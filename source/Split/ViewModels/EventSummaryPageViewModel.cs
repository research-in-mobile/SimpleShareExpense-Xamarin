using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Split.Entities;
using Split.Helpers;
using Split.Models;
using Split.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Split.ViewModels
{
	public class EventSummaryPageViewModel : BaseViewModel
	{
		private readonly IEventService _eventService;
		private readonly IExpenseService _expenseService;

		private User _host;

		private Event _event;
		public Event Event
		{
			get => _event;
			set => SetProperty(ref _event, value);
		}

		public TimeSpan EventSpan
		{
			get
			{
				if (Event != null)
					return Event.EndDate - Event.StartDate;

				return default(TimeSpan);
			}
		}

		private double _eventCost;
		public double EventCost
		{
			get => _eventCost;
			set => SetProperty(ref _eventCost, value);
		}

		private double _costPaid;
		public double CostPaid
		{
			get => _costPaid;
			set => SetProperty(ref _costPaid, value);
		}

		private double _numberOfGuests;
		public double NumberOfGuests
		{
			get => _numberOfGuests;
			set => SetProperty(ref _numberOfGuests, value);
		}

		private ObservableCollection<UserExpenseAggregate> _userEventExpenses;
		public ObservableCollection<UserExpenseAggregate> UserEventExpenses
		{
			get => _userEventExpenses;
			set => SetProperty(ref _userEventExpenses, value);
		}

		private Dictionary<string, TransactionStrategy> _transactionStrategies;
		public List<string> TransactionStrategies => _transactionStrategies?.Keys.ToList();

		private TransactionStrategy _transactionStrategySelected;
		private int _selectedTransactionStrategyIndex;
		public int SelectedTransactionStrategyIndex
		{
			get => _selectedTransactionStrategyIndex;
			set
			{
				if (value >= 0)
				{
					_selectedTransactionStrategyIndex = value;
					RaisePropertyChanged();

					_transactionStrategies.TryGetValue(TransactionStrategies[value], out _transactionStrategySelected);
					Event.TransactionStrategy = _transactionStrategySelected;
				}
			}
		}


		public EventSummaryPageViewModel(
			IBaseViewModelServiceProvider baseServiceProvider,
			IEventService eventService,
			IExpenseService expenseService)
			: base(baseServiceProvider)
		{
			_eventService = eventService;
			_expenseService = expenseService;
		}

		protected override async Task RefreshAsync()
		{
			using (Busy())
			{
				await base.RefreshAsync();

			}
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);

			using (Busy())
			{
				var hasEvent = parameters.TryGetValue("Event", out Event evnt);
				var hasEventId = parameters.TryGetValue("EventId", out int eventId);

				if (hasEvent)
				{
					Event = evnt;

					Load();
				}
				else if (hasEventId)
				{
					Event = await _eventService.GetEventAsync(eventId);

					Load();
				}
				else
				{
					await GoBackAsync();
				}

				await RefreshAsync();
			}
		}

		protected override async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = null)
		{
			parameters = new NavigationParameters();
			parameters.Add("EventId", Event.EventId);
			parameters.Add("EventId", Event);

			await base.GoBackAsync(parameters);
		}

		private void Load()
		{
			using (Busy())
			{

				Title = Event?.Title;
				_host = Event.Hosts?.First();

				EventCost = _eventService.TotalExpense(Event);
				NumberOfGuests = _eventService.NumberOfGuests(Event);

				if (_transactionStrategies == null)
				{
					_transactionStrategies = _eventService?.GetTransactionStrategies();
					RaisePropertyChanged(nameof(TransactionStrategies));
				}

				var key = _transactionStrategies.FirstOrDefault(x => x.Value == Event.TransactionStrategy).Key;
				SelectedTransactionStrategyIndex = TransactionStrategies.IndexOf(key);
				RaisePropertyChanged(nameof(EventSpan));
			}
		}

		private ObservableCollection<UserExpenseAggregate> CalculateTransactions(TransactionStrategy transactionStrategy)
		{
			switch (transactionStrategy)
			{
				case TransactionStrategy.HMT:
					return new ObservableCollection<UserExpenseAggregate>(EventHelper.CalculateHostManagedTransaction(Event));

				case TransactionStrategy.BET:
					return new ObservableCollection<UserExpenseAggregate>(EventHelper.CalculateByExpenseTransaction(Event));

				case TransactionStrategy.MT:
					return new ObservableCollection<UserExpenseAggregate>(EventHelper.CalculateMinimumTransaction(Event));

				default:
					return new ObservableCollection<UserExpenseAggregate>(EventHelper.CalculateHostManagedTransaction(Event));

			}
		}


	}
}
