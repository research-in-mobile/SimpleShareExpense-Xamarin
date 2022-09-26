using Microsoft.EntityFrameworkCore;
using Split.Entities;
using Split.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
	public class AppDbContext : DbContext
	{
		private string _dbPath;

		//public virtual DbSet<AppSettings> AppSettings { get; set; }
		public virtual DbSet<Event> Events { get; set; }
		public virtual DbSet<Expense> Expenses { get; set; }
		public virtual DbSet<CostAllocation> CostAllocation { get; set; }

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<EventUser> EventUsers { get; set; }

		public virtual DbSet<Contact> Contacts { get; set; }
		public virtual DbSet<UserEventExpense> UserEventExpense { get; set; }
		public virtual DbSet<Transaction> Payments { get; set; }

		public AppDbContext()
		{
			_dbPath = LocalStorageHelper.GetDBLocation();

			//this.Database.EnsureDeleted();
			this.Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Filename={_dbPath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<AppSettings>()
			//	.HasKey(p => p.AppSettingsId);

			modelBuilder.Entity<Event>()
				.HasKey(p => p.EventId);

			modelBuilder.Entity<Expense>()
				.HasKey(p => p.ExpenseId);

			modelBuilder.Entity<Event>()
				.Property(p => p.EventId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<Expense>()
				.Property(p => p.ExpenseId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<User>()
				.HasKey(p => p.UserId);

			modelBuilder.Entity<User>()
				.Property(p => p.UserId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<Contact>()
				.HasKey(p => p.ContactId);

			modelBuilder.Entity<UserEventExpense>()
				.HasKey(p => p.UserEventExpenseId);

			modelBuilder.Entity<Transaction>()
				.HasKey(p => p.TransactionId);

			modelBuilder.Entity<CostAllocation>()
				.HasKey(p => p.CostAllocationId);

			modelBuilder.Entity<EventUser>()
				.HasKey(p => new { p.EventRef, p.UserRef });


			modelBuilder.Entity<UserEventExpense>()
				.HasOne(p => p.User)
				.WithMany(p => p.UserEventExpenses)
				.HasForeignKey(p => p.UserRef);

			modelBuilder.Entity<Transaction>()
				.HasOne(p => p.UserEventExpense)
				.WithMany(p => p.Transactions)
				.HasForeignKey(p => p.UserEventExpenseRef);

			modelBuilder.Entity<Transaction>()
				.HasOne(e => e.PayableUser)
				.WithMany(p => p.Receivables)
				.HasForeignKey(p => p.PayableUserRef);

			modelBuilder.Entity<User>()
				.HasOne(p => p.Contact)
				.WithOne(p => p.User)
				.HasForeignKey<Contact>(p => p.UserRef);

			//modelBuilder.Entity<User>()
			//	.HasOne(p => p.AppSettings)
			//	.WithOne(p => p.User)
			//	.HasForeignKey<AppSettings>(p => p.UserRef);

			modelBuilder.Entity<EventUser>()
				.HasOne(p => p.Event)
				.WithMany(p => p.EventUsers)
				.HasForeignKey(p => p.EventRef);

			modelBuilder.Entity<EventUser>()
				.HasOne(p => p.User)
				.WithMany(p => p.EventUsers)
				.HasForeignKey(p => p.UserRef);

			modelBuilder.Entity<Expense>()
				.HasOne(p => p.Event)
				.WithMany(p => p.Expenses)
				.HasForeignKey(p => p.EventRef);

			modelBuilder.Entity<Expense>()
				.HasOne(p => p.AssignedUser)
				.WithMany(p => p.UserExpenses)
				.HasForeignKey(p => p.UserRef);

			modelBuilder.Entity<Expense>()
				.HasMany(p => p.CostAllocations)
				.WithOne(p => p.Expense)
				.HasForeignKey(p => p.ExpenseRef);

			modelBuilder.Entity<CostAllocation>()
				.HasOne(p => p.User)
				.WithMany(p => p.CostAllocations)
				.HasForeignKey(p => p.UserRef);


#if MOCKDEBUG

			modelBuilder.Entity<Contact>()
				.HasData(
					new Contact
					{
						ContactId = 1,
						UserRef = 1,
						Name = "Ajay",
						Email = "ajay@sivanesu.com",
						Phone = "6477848922",
						AddressLine1 = "69 Clarion Crescent, Markham, Ontario, L3S3M5, Canada."
					}
				);

			modelBuilder.Entity<UserEventExpense>()
				.HasData(
					new UserEventExpense
					{
						UserEventExpenseId = 1,
						UserRef = 1,
						Cost = 100,
						Spend = 50,
						Balance = -50,
						IsPaid = false,

					}
				);

			modelBuilder.Entity<Payment>()
				.HasData(
					new Payment
					{
						PaymentId = 1,
						PayableUserRef = 1,
						UserEventExpenseRef = 1,
						Amount = 35
					},
					new Payment
					{
						PaymentId = 2,
						PayableUserRef = 2,
						UserEventExpenseRef = 1,
						Amount = 15
					}

				);

			modelBuilder.Entity<User>()
				.HasData(
					new User
					{
						UserId = 1,
						Name = "Ajay",
						Type = UserEntityType.Business
					},
					new User
					{
						UserId = 2,
						Name = "Glani",
						Type = UserEntityType.Person
					}

				);

			modelBuilder.Entity<Event>()
				.HasData(
					new Event
					{
						EventId = 1,
						Title = "Test Event",
						StartDate = DateTime.UtcNow,
						EndDate = DateTime.UtcNow
					}

				);

			modelBuilder.Entity<EventUser>()
				.HasData(
					new EventUser
					{
						EventRef = 1,
						UserRef = 1
					}

				);

			modelBuilder.Entity<Expense>()
				.HasData(
					new Expense
					{
						ExpenseId = 1,
						Title = "Test Expense 35",
						UserRef = 1,
						EventRef = 1,
						CurrencyType = "CAD",
						Cost = 35,
						IsPaid = true
					},
					new Expense
					{
						ExpenseId = 2,
						Title = "Test Expense 15",
						UserRef = 1,
						EventRef = 1,
						CurrencyType = "CAD",
						Cost = 15,
						IsPaid = true
					}
				);

			modelBuilder.Entity<CostAllocation>()
				.HasData(
					new CostAllocation
					{
						CostAllocationId = 1,
						IsSplitWith = true,
						AllocatedCost = 25,
						AllocationPercentage = 50,
						AllocationWeight = 1,
						ExpenseRef = 1,
						UserRef = 1
					},
					new CostAllocation
					{
						CostAllocationId = 2,
						IsSplitWith = true,
						AllocatedCost = 25,
						AllocationPercentage = 50,
						AllocationWeight = 1,
						ExpenseRef = 1,
						UserRef = 2
					}
				);
#endif


		}

	}
}