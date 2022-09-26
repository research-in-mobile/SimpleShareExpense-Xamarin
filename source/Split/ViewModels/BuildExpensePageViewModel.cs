using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Split.Models;
using Split.Helpers;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Split.Services;
using Split.Data;
using Microsoft.EntityFrameworkCore;

namespace Split.ViewModels
{
	public class BuildExpensePageViewModel : BaseViewModel
	{
		private readonly IExpenseService _expenseService;

		protected static event EventHandler CalculateCost;
		protected static void OnCalculateCost(object sender, EventArgs e) => CalculateCost?.Invoke(sender, e);

		private int _eventRef;
		protected Event Event;

		private double _deltaSplitCost;
		public double DeltaSplitCost
		{
			get => _deltaSplitCost;
			set => SetProperty(ref _deltaSplitCost, Math.Round(value, 2));
		}

		private double _cost;
		public double Cost
		{
			get => _cost;
			set
			{
				SetProperty(ref _cost, Math.Round(value, 2));
				OnCalculateCost(this, new EventArgs());
				RaisePropertyChanged(nameof(ConvertedCost));
			}
		}

		private int _assignedUserIndex;
		public int AssignedUserIndex
		{
			get => _assignedUserIndex;
			set
			{
				if (value >= 0)
				{
					_assignedUserIndex = value;
					RaisePropertyChanged();

					var SelectedEntity = Entities[value];

					Expense.UserRef = SelectedEntity.User.UserId;
					Expense.AssignedUser = SelectedEntity.User;
				}
			}
		}

		private Expense _expense = new Expense();
		public Expense Expense
		{
			get => _expense;
			set => SetProperty(ref _expense, value);
		}

		private ObservableCollection<CostAllocatedUser> _entities = new ObservableCollection<CostAllocatedUser>();
		public ObservableCollection<CostAllocatedUser> Entities
		{
			get => _entities;
			set => SetProperty(ref _entities, value);
		}

		public DateTime ExpenseDate
		{
			get => Expense.ExpenseDate;
			set
			{
				if (value != null)
				{
					Expense.ExpenseDate = value;
					RaisePropertyChanged();
				}
			}
		}

		public double ConvertaionRate
		{
			get => Expense.ConvertaionRate;
			set
			{
				if (Expense.ConvertaionRate != value)
				{
					Expense.ConvertaionRate = value;
					RaisePropertyChanged();
				}
			}
		}

		public double ConvertedCost
		{
			get
			{
				return ConvertaionRate * Cost;
			}
		}


		public int CostSplitCount
		{
			get => Entities.Count(e => e.IsSplitWith);
		}

		public DelegateCommand DoneCommand { get; set; }
		public DelegateCommand DeleteExpenseCommand { get; set; }

		public BuildExpensePageViewModel(
			IBaseViewModelServiceProvider baseServiceProvider,
			IExpenseService expenseService)
			: base(baseServiceProvider)
		{
			_expenseService = expenseService;

			Initialize();
		}

		public void Initialize()
		{ 
			DoneCommand = new DelegateCommand(SaveAndGoBack);
			DeleteExpenseCommand = new DelegateCommand(DeleteAndGoBack);

			CalculateCost += (s, e) =>
			{
				Entities = ExpenseHelper.CheckAndUpdateCostAllocationWeight(Entities);
				Entities = ExpenseHelper.CalculateUserCosts(Entities, Cost);
				Entities = ExpenseHelper.SplitErrorCorrection(Entities, Cost);
			};
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);

			using (Busy())
			{

				var hasEventId = parameters.TryGetValue("EventId", out _eventRef);
				var hasExpense = parameters.TryGetValue("Expense", out Expense expense);
				var hasGuests = parameters.TryGetValue("Guests", out IEnumerable<User> guests);

				hasEventId = hasEventId && _eventRef != 0;
				hasExpense = hasExpense && expense != null && (expense?.ExpenseId != 0);
				hasGuests = hasGuests && guests != null && (guests?.Count() > 0);

				IsNew = hasExpense ? false : true;

				if (IsNew)
				{
					Expense = new Expense();
					ExpenseDate = DateTime.Now;

					if (hasGuests)
					{
						foreach (var guest in guests)
						{
							var guestCA = new CostAllocatedUser(guest, OnCalculateCost);
							//guestCA.ExternallyStateChanged += OnPageStateChanged;

							Entities.Add(guestCA);
						}

						AssignedUserIndex = 0;
					}
				}
				else
				{
					Expense = expense;
					Cost = expense.Cost;
					ExpenseDate = expense.ExpenseDate != default ? expense.ExpenseDate : DateTime.Now;

					foreach (var guest in Expense.CostAllocations)
					{
						var guestCA = new CostAllocatedUser(guest, OnCalculateCost);
						//guestCA.ExternallyStateChanged += OnPageStateChanged;

						Entities.Add(guestCA);
					}

					AssignedUserIndex = Entities.IndexOf(Entities.First(e => e.User.UserId == expense.UserRef));
				}

				if (ConvertaionRate <= 0)
					ConvertaionRate = 1;
			}

			PropertyChanged += OnPageStateChanged;
		}

		protected override async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = null)
		{
			parameters = new NavigationParameters();
			parameters.Add("EventId", _eventRef);
			parameters.Add("Expense", Expense);

			await base.GoBackAsync(parameters);
		}

		protected void OnPageStateChanged(object sender, EventArgs e)
		{
			HasChanged = true;
		}

		private async void SaveAndGoBack()
		{
			if (await SaveExpense())
				await GoBackAsync();
		}

		private async void DeleteAndGoBack()
		{
			if (IsNew)
				await GoBackAsync();

			if (await _expenseService.DeleteExpenseAsync(Expense))
				await GoBackAsync();
		}

		private async Task<bool> SaveExpense()
		{
			if (!CanSaveExpense())
				return false;

			using (Busy())
			{
				if (IsNew)
				{
					HasChanged = await CreateNewExpense() != null ? false : true;
				}
				else
				{
					HasChanged = await UpdateExpense() != null ? false : true;
				}

				HasChanged = false;
			}

			return true;
		}

		private bool CanSaveExpense()
		{
			if (String.IsNullOrEmpty(Expense?.Title))
				return false;

			return true;
		}

		private async Task<Expense> UpdateExpense()
		{
			Expense.Cost = Cost;

			for (int i = 0; i < Entities.Count(); i++)
			{
				Expense.CostAllocations.ElementAt(i).AllocatedCost = Entities.ElementAt(i).AllocatedCost;
				Expense.CostAllocations.ElementAt(i).AllocationWeight = Entities.ElementAt(i).AllocationWeight;
				Expense.CostAllocations.ElementAt(i).AllocationPercentage = Entities.ElementAt(i).AllocationPercentage;
				Expense.CostAllocations.ElementAt(i).IsSplitWith = Entities.ElementAt(i).IsSplitWith;
				Expense.CostAllocations.ElementAt(i).UserRef = Entities.ElementAt(i).User.UserId;
			}

			return await _expenseService.UpdateExpenseAsync(Expense);
		}

		private async Task<Expense> CreateNewExpense()
		{
			Expense.EventRef = _eventRef;
			Expense.Cost = Cost;
			Expense.CostAllocations = new List<CostAllocation>();

			foreach (var entity in Entities)
			{
				var ca = new CostAllocation()
				{
					AllocatedCost = entity.AllocatedCost,
					AllocationWeight = entity.AllocationWeight,
					AllocationPercentage = entity.AllocationPercentage,
					IsSplitWith = entity.IsSplitWith,
					UserRef = entity.User.UserId
				};

				Expense.CostAllocations.Add(ca);
			}

			return await _expenseService.AddExpenseAsync(Expense);
		}

	}
}
