using Prism.Mvvm;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Split.Models
{
	public class UserExpenseAggregate : BindableBase
	{
		public UserExpenseAggregate()
		{
		}

		private User _user;
		public User User
		{
			get => _user;
			set => SetProperty(ref _user, value);
		}

		private ObservableCollection<Transaction> _payables = new ObservableCollection<Transaction>();
		public ObservableCollection<Transaction> Payables
		{
			get => _payables;
			set => SetProperty(ref _payables, value);
		}

		private ObservableCollection<Transaction> _receivables = new ObservableCollection<Transaction>();
		public ObservableCollection<Transaction> Receivables
		{
			get => _receivables;
			set => SetProperty(ref _receivables, value);
		}

		private double _spend;
		public double Spend
		{
			get => _spend;
			set => SetProperty(ref _spend, value);
		}

		private double _cost;
		public double Cost
		{
			get => _cost;
			set => SetProperty(ref _cost, value);
		}

		public double Balance
		{
			get => Math.Round(Cost - Spend, 2);
		}

		public bool IsPayable
		{
			get => Balance > 0;
		}

		private bool _isResolved;
		public bool IsResolved
		{
			get => _isResolved;
			set => SetProperty(ref _isResolved, value);
		}

		public bool TransactionsCostSpendBalanceCheck
		{
			get => TransactionsBalanceCheck();
		}

		protected bool TransactionsReceivablesCheck()
		{
			double receivableTotal = 0;

			foreach (var receivable in Receivables)
			{
				receivableTotal += receivable.Amount;
			}

			return Spend == receivableTotal;
		}

		protected bool TransactionsPayablesCheck()
		{
			double payableTotal = 0;

			foreach (var payable in Payables)
			{
				payableTotal += payable.Amount;
			}

			return Cost == payableTotal;
		}

		protected bool TransactionsBalanceCheck()
		{
			double receivableTotal = 0;
			double payableTotal = 0;

			foreach (var receivable in Receivables)
			{
				receivableTotal += receivable.Amount;
			}


			foreach (var payable in Payables)
			{
				payableTotal += payable.Amount;
			}

			var balance = Math.Round(payableTotal - receivableTotal, 2);

			return Balance == balance && 
					Cost == payableTotal &&
					Spend == receivableTotal;
		}
	}
}
