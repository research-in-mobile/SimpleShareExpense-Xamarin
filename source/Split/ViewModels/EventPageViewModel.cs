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
using System.Linq;
using System.Threading.Tasks;

namespace Split.ViewModels
{
	public class EventPageViewModel : BaseViewModel
	{
		private readonly IEventService _eventService;
		private readonly IExpenseService _expenseService;


		private Event _event;
		public Event Event
		{
			get => _event;
			set => SetProperty(ref _event, value);
		}

		private double _totalExpense;
		public double TotalExpense
		{
			get => _totalExpense;
			set => SetProperty(ref _totalExpense, value);
		}

		private double _numberOfGuests;
		public double NumberOfGuests
		{
			get => _numberOfGuests;
			set => SetProperty(ref _numberOfGuests, value);
		}

		private ObservableCollection<Expense> _expenses;
		public ObservableCollection<Expense> Expenses
		{
			get => _expenses;
			set => SetProperty(ref _expenses, value);
		}

		private ObservableCollection<CostAllocation> _guests;
		public ObservableCollection<CostAllocation> Guests
		{
			get => _guests;
			set => SetProperty(ref _guests, value);
		}

		private ObservableCollection<UserExpenseAggregate> _userExpenses;
		public ObservableCollection<UserExpenseAggregate> UserExpenses
		{
			get => _userExpenses;
			set => SetProperty(ref _userExpenses, value);
		}

		public DelegateCommand InviteEntityCommand { get; set; }
		public DelegateCommand AddExpenseCommand { get; set; }
		public DelegateCommand<object> EditExpenseCommand { get; set; }
		public DelegateCommand<object> DeleteExpenseCommand { get; set; }
		public DelegateCommand EventSummaryCommand { get; set; }
		public DelegateCommand EditEventCommand { get; set; }
		public DelegateCommand EventSummaryeCommand { get; set; }

		//TODO: Calculate payment breakdown per sure 
		public EventPageViewModel(
			IBaseViewModelServiceProvider baseServiceProvider,
			IEventService eventService,
			IExpenseService expenseService)
			: base(baseServiceProvider)
		{
			_eventService = eventService;
			_expenseService = expenseService;


			AddExpenseCommand = new DelegateCommand(async () => await NavigateToBuilExpensePageAsync(null)).ObservesProperty(() => IsBusy);
			DeleteExpenseCommand = new DelegateCommand<object>(async (obj) => await DeleteExpenseAsync(obj)).ObservesProperty(() => IsBusy);
			EditExpenseCommand = new DelegateCommand<object>(async (obj) => await NavigateToBuilExpensePageAsync((Expense)obj)).ObservesProperty(() => IsBusy);
			EditEventCommand = new DelegateCommand(async () => await NavigateToBuildEventPageAsync(Event)).ObservesProperty(() => IsBusy);
			EventSummaryCommand = new DelegateCommand(async () => await NavigateToEventSummaryPageAsync());
		}

		protected override async Task RefreshAsync()
		{
			using (Busy())
			{
				await base.RefreshAsync();

				Title = Event?.Title;

				TotalExpense = _eventService.TotalExpense(Event);
				NumberOfGuests = _eventService.NumberOfGuests(Event);

				var userEventExpenses = EventHelper.CalculateByExpenseTransaction(Event);

				if (userEventExpenses != null)
					UserExpenses = new ObservableCollection<UserExpenseAggregate>(userEventExpenses);

			}
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			IsBusy = true;

			base.OnNavigatedTo(parameters);


			var hasEvent = parameters.TryGetValue("Event", out Event evnt);
			var hasEventId = parameters.TryGetValue("EventId", out int eventId);

			if (hasEvent)
			{
				Event = evnt;
			}
			else if (hasEventId)
			{
				Event = await _eventService.GetEventAsync(eventId);
			}
			else
			{
				await GoBackAsync();
			}

			Expenses = Event.Expenses == null ? new ObservableCollection<Expense>() : new ObservableCollection<Expense>(Event.Expenses);			

			await RefreshAsync();

			IsBusy = false;
		}

		#region Private Methods 
		private async Task DeleteExpenseAsync(object obj)
		{
			if (IsBusy)
				return;

			using (Busy())
			{
				var expense = obj as Expense;
				await _expenseService.DeleteExpenseAsync(expense);

				Expenses.Remove(expense);
				await RefreshAsync();
			}
		}

		private async Task NavigateToBuildEventPageAsync(Event evnt)
		{
			if (IsBusy)
				return;

			using (Busy())
			{
				var parameters = new NavigationParameters();
				parameters.Add("Event", evnt);

				await NavigationService.NavigateAsync("BuildEventPage", parameters, useModalNavigation: true);
			}
		}

		private async Task NavigateToBuilExpensePageAsync(Expense expense = null)
		{
			if (IsBusy)
				return;

			using (Busy())
			{
				var parameters = new NavigationParameters();
				parameters.Add("EventId", Event.EventId);
				parameters.Add("Guests", Event.Guests);
				parameters.Add("Expense", expense);

				await NavigationService.NavigateAsync("BuildExpensePage", parameters, useModalNavigation: false);
			}

		}

		private async Task NavigateToEventSummaryPageAsync()
		{
			if (IsBusy)
				return;

			using (Busy())
			{
				var parameters = new NavigationParameters();
				parameters.Add("EventId", Event.EventId);
				parameters.Add("Event", Event);

				await NavigationService.NavigateAsync("EventSummaryPage", parameters, useModalNavigation: true);
			}
		}
		#endregion

	}
}
