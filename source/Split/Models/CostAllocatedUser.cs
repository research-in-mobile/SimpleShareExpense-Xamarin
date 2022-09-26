using Prism.Mvvm;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Split.Models
{
	public class CostAllocatedUser : BindableBase
	{
		protected Action<object, EventArgs> CalculateCost;
		public event EventHandler ExternallyStateChanged;

		public CostAllocatedUser() { }

		public CostAllocatedUser(CostAllocation costAllocation, Action<object, EventArgs> calculateCost = null)
		{
			CalculateCost = calculateCost;

			if (costAllocation != null)
				CopyEntity(costAllocation);
		}

		public CostAllocatedUser(User user, Action<object, EventArgs> calculateCost = null)
		{
			User = user;
			CalculateCost = calculateCost;
		}

		private bool _isSplitWith = true;
		public bool IsSplitWith
		{
			get => _isSplitWith;
			set
			{
				SetProperty(ref _isSplitWith, value);
				AllocatedWeightCheck();
				CalculateCost(this, new EventArgs());

				ExternallyStateChanged?.Invoke(this, new EventArgs());
			}
		}

		private double _allocatedCost;
		public double AllocatedCost
		{
			get => _allocatedCost;
			set
			{
				SetProperty(ref _allocatedCost, value);
			}
		}

		private double _allocationPercentage;
		public double AllocationPercentage
		{
			get => _allocationPercentage;
			set
			{
				SetProperty(ref _allocationPercentage, value);
			}
		}

		private int _allocationWeight = 1;
		public int AllocationWeight
		{
			get => _allocationWeight;
			set
			{
				if (value >= 0)
					SetProperty(ref _allocationWeight, value);
			}
		}

		private void AllocatedWeightCheck()
		{
			if (IsSplitWith)
			{
				if (AllocationWeight < 1)
					AllocationWeight = 1;
			}
			else
			{
				AllocationWeight = 0;
			}
		}

		public User User { get; private set; }

		public CostAllocatedUser CopyEntity(CostAllocation costAllocation)
		{
			IsSplitWith = costAllocation.IsSplitWith;
			AllocatedCost = costAllocation.AllocatedCost;
			AllocationPercentage = costAllocation.AllocationPercentage;
			AllocationWeight = costAllocation.AllocationWeight;
			User = costAllocation.User;

			return this;
		}

		public CostAllocation ConvertToEntity(CostAllocation costAllocation)
		{
			costAllocation.IsSplitWith = IsSplitWith;
			costAllocation.AllocatedCost = AllocatedCost;
			costAllocation.AllocationPercentage = AllocationPercentage;
			costAllocation.AllocationWeight = AllocationWeight;

			return costAllocation;
		}
	}
}
