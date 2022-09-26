using Split.Entities;
using Split.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Split.Helpers
{
	public static class ExpenseHelper
	{
		public static ObservableCollection<CostAllocatedUser> CalculateUserCosts(ObservableCollection<CostAllocatedUser> entities, double cost)
		{
			double entitySplitCount = GetSplitByQunatity(entities);

			if (entitySplitCount == 0)
				return entities;

			foreach (var entity in entities)
			{
				entity.AllocationPercentage = entity.AllocationWeight / entitySplitCount;

				if (entity.IsSplitWith)
				{

					entity.AllocatedCost = Math.Round(cost * entity.AllocationPercentage, 2);
				}
				else
				{
					entity.AllocatedCost = 0;
				}
			}

			return entities;
		}
		public static IList<CostAllocation> CalculateUserCosts(IList<CostAllocation> entities, double cost)
		{
			double entitySplitCount = GetSplitByQunatity(entities);

			foreach (var entity in entities)
			{
				entity.AllocationPercentage = entity.AllocationWeight / entitySplitCount;

				if (entity.IsSplitWith)
				{

					entity.AllocatedCost = Math.Round(cost * entity.AllocationPercentage, 2);
				}
				else
				{
					entity.AllocatedCost = 0;
				}
			}

			return entities;
		}

		public static int GetSplitByQunatity(ICollection<CostAllocatedUser> entities)
		{
			int count = 0;

			foreach (var entity in entities)
			{
				if (entity.IsSplitWith)
				{
					count += entity.AllocationWeight;
				}
			}

			return count;
		}
		public static int GetSplitByQunatity(IEnumerable<CostAllocation> entities)
		{
			int count = 0;

			foreach (var entity in entities)
			{
				if (entity.IsSplitWith)
				{
					count += entity.AllocationWeight;
				}
			}

			return count;
		}

		public static ObservableCollection<CostAllocatedUser> CheckAndUpdateCostAllocationWeight(ObservableCollection<CostAllocatedUser> entities)
		{
			foreach (var entity in entities)
			{
				if (entity.IsSplitWith && entity.AllocationWeight < 1)
				{
					entity.AllocationWeight = 1;
				}
			}

			return entities;
		}
		public static IEnumerable<CostAllocation> CheckAndUpdateCostAllocationWeight(IEnumerable<CostAllocation> entities)
		{
			foreach (var entity in entities)
			{
				if (entity.IsSplitWith && entity.AllocationWeight < 1)
				{
					entity.AllocationWeight = 1;
				}
			}

			return entities;
		}

		public static ObservableCollection<CostAllocatedUser> SplitErrorCorrection(ObservableCollection<CostAllocatedUser> entities, double cost)
		{
			var splitBy = GetSplitByQunatity(entities);

			if (splitBy < 1)
				return entities;

			var deltaSplitCost = cost - entities.Sum(e => e.AllocatedCost);

			if (deltaSplitCost == 0)
				return entities;

			if (deltaSplitCost < 0)
			{
				var entity = entities.First(e => e.IsSplitWith);
				entity.AllocatedCost += deltaSplitCost;
			}
			else
			{
				var outterBound = Math.Max(splitBy, 1);
				Random randomGen = new Random();
				var randomIndex = randomGen.Next(1, outterBound);

				var entity = entities.OrderBy(e => e.IsSplitWith)
								.ToList();

				entity[randomIndex].AllocatedCost += deltaSplitCost;
			}

			return entities;
		}
		public static IList<CostAllocation> SplitErrorCorrection(IList<CostAllocation> entities, double cost)
		{
			var splitBy = GetSplitByQunatity(entities);

			if (splitBy < 1)
				return entities;

			var deltaSplitCost = cost - entities.Sum(e => e.AllocatedCost);

			if (deltaSplitCost == 0)
				return entities;

			if (deltaSplitCost < 0)
			{
				var entity = entities.First(e => e.IsSplitWith);
				entity.AllocatedCost += deltaSplitCost;
			}
			else
			{
				var outterBound = Math.Max(splitBy, 1);
				Random randomGen = new Random();
				var randomIndex = randomGen.Next(1, outterBound);

				var entity = entities.OrderBy(e => e.IsSplitWith)
								.ToList();

				entity[randomIndex].AllocatedCost += deltaSplitCost;
			}

			return entities;
		}

		public static IEnumerable<UserExpenseAggregate> GetUserExpenseAggregate(IEnumerable<IEnumerable<CostAllocation>> eventCostAllocations)
		{
			var result = new List<UserExpenseAggregate>();
			bool initial = true;
			int count = 0;

			foreach (var EventCAS in eventCostAllocations)
			{
				foreach (var expenseCAS in EventCAS)
				{
					if (initial)
					{
						var uee = new UserExpenseAggregate();
						uee.User = expenseCAS.User;
						uee.Cost = expenseCAS.AllocatedCost;

						result.Add(uee);
					}
					else
					{
						result[count].Cost += expenseCAS.AllocatedCost;

						count++;
					}
				}

				initial = false;
				count = 0;
			}

			return result;
		}

		public static IEnumerable<UserExpenseAggregate> GetUserExpenseAggregate(ICollection<User> guests)
		{
			var result = new List<UserExpenseAggregate>();

			foreach (var guest in guests)
			{
				var uee = new UserExpenseAggregate();
				uee.User = guest;

				result.Add(uee);
			}

			return result;
		}

		public static ICollection<Expense> UpdateCostAllocations(IEnumerable<User> newUsers, ICollection<Expense> expenses)
		{
			if (newUsers == null || expenses == null)
				return null;

			var oldUsers = expenses.Select(ex => ex.CostAllocations.Select(ca => ca.User));
			var numberOfNewUsers = newUsers.Count() - oldUsers?.ElementAt(0)?.Count();
			numberOfNewUsers = numberOfNewUsers ?? 0;

			if (numberOfNewUsers > 0)
			{
				for (int i = 0; i < newUsers.Count(); i++)
				{
					if (i < (newUsers.Count() - numberOfNewUsers))
					{
						var oldUser = oldUsers?.ElementAt(0)?.ElementAt(i);
						var hasOldUser = newUsers?.Contains(oldUser);

						if (hasOldUser == false)
						{
							IEnumerable<CostAllocation> costAllocations;

							expenses.ForEach(ex =>
							{
								costAllocations = ex.CostAllocations
													.Where(ca => ca.User == oldUser);

								foreach (var ca in costAllocations)
								{
									ex.CostAllocations.Remove(ca);
								}
							});
						}
					}

					var newUser = newUsers?.ElementAt(i);
					var hasNewUser = oldUsers?.ElementAt(0).Contains(newUser);

					if (hasNewUser == false)
					{
						expenses.ForEach(ex =>
						{
							ex.CostAllocations.Add(new CostAllocation()
							{
								User = newUser,
								UserRef = newUser.UserId,
								IsSplitWith = true,
								AllocationWeight = 1,
								Expense = ex,
								ExpenseRef = ex.ExpenseId
							});
						});

					}

				}
			}
			else
			{
				for (int i = 0; i < oldUsers.ElementAt(0).Count(); i++)
				{
					var oldUser = oldUsers?.ElementAt(0)?.ElementAt(i);
					var hasOldUser = newUsers?.Contains(oldUser);

					if (hasOldUser == false)
					{
						//IList<CostAllocation> costAllocations;

						expenses.ForEach(ex =>
						{
							ex.CostAllocations = ex.CostAllocations.Where(x => x.User != oldUser).ToList();

							//ex.CostAllocations.RemoveAll(x => x.User != oldUser);

							//costAllocations = ex.CostAllocations
							//					.Where(ca => ca.User == oldUser);

							//foreach (var ca in costAllocations)
							//{
							//	ex.CostAllocations.Remove(ca);
							//}
						});

					}

					if (i < (oldUsers.Count() + numberOfNewUsers))
					{
						var newUser = newUsers?.ElementAt(i);
						var hasNewUser = oldUsers?.ElementAt(0).Contains(newUser);

						if (hasNewUser == false)
						{
							expenses.ForEach(ex =>
							{
								ex.CostAllocations.Add(new CostAllocation()
								{
									User = newUser,
									UserRef = newUser.UserId,
									IsSplitWith = true,
									AllocationWeight = 1,
									Expense = ex,
									ExpenseRef = ex.ExpenseId
								});
							});

						}
					}

				}
			}

			expenses.ForEach(ex =>
			{
				ex.CostAllocations = ex.CostAllocations.OrderBy(ca => ca.User.Name).ToList();
				ex.CostAllocations = CalculateUserCosts(ex.CostAllocations, ex.Cost);
				ex.CostAllocations = SplitErrorCorrection(ex.CostAllocations, ex.Cost);

			});

			return expenses;
		}

		public static IEnumerable<Transaction> CalculateTransactions(Expense expense)
		{
			var transactions = new List<Transaction>();

			foreach (var costAllocation in expense.CostAllocations)
			{

				var transaction = new Transaction();

				transaction.Amount = costAllocation.AllocatedCost;
				transaction.Expense = expense;
				transaction.ExpenseRef = expense.ExpenseId;

				transaction.PayableUser = costAllocation.User;
				transaction.PayableUserRef = costAllocation.UserRef;

				transaction.ReceivableUser = expense.AssignedUser;
				transaction.ReceivableUserRef = expense.UserRef;

				if (transaction.PayableUserRef == transaction.ReceivableUserRef)
				{
					transaction.IsComplete = true;
					transaction.IsAutoComplete = true;
				}

				transactions.Add(transaction);
			}

			return transactions;
		}

		public static double TransactionsCostBalanceCheck(IList<Transaction> transactions, double cost)
		{
			double balance = cost;

			foreach (var transaction in transactions)
			{
				balance -= transaction.Amount;
			}

			return balance;
		}


	}
}