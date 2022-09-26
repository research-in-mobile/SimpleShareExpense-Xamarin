using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected readonly AppDbContext Context;
		protected readonly DbSet<TEntity> DbSet;

		public Repository(AppDbContext context)
		{
			Context = context;
			DbSet = Context.Set<TEntity>();
		}


		public TEntity Get(int id)
		{
			return DbSet.Find(id);
		}

		public async Task<TEntity> GetAsync(int id)
		{
			try
			{
				var item = await DbSet
							.FindAsync(id)
							.ConfigureAwait(false);

				return item;
			}
			catch (NullReferenceException ex)
			{
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		public IEnumerable<TEntity> GetAll()
		{
			return DbSet.ToList();
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync(bool forceRefresh = false)
		{
			var allItems = await DbSet
							.ToListAsync()
							.ConfigureAwait(false);

			return allItems;
		}


		public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
		{
			return DbSet.Where(predicate);
		}

		public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return DbSet.SingleOrDefault(predicate);
		}


		public void Add(TEntity entity)
		{
			var result = DbSet.Add(entity);
		}

		public async Task<bool> AddAsync(TEntity entity)
		{
			await DbSet
					.AddAsync(entity)
					.ConfigureAwait(false);

			return true;
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			DbSet.AddRange(entities);
		}

		public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
		{
			await DbSet
					.AddRangeAsync(entities)
					.ConfigureAwait(false);

			return true;
		}


		public void Remove(TEntity entity)
		{
			DbSet.Remove(entity);
		}

		public async Task<bool> RemoveAsync(int id)
		{
			var itemToRemove = await DbSet.FindAsync(id);

			if (itemToRemove != null)
			{
				DbSet.Remove(itemToRemove);
			}

			return true;
		}

		public void RemoveRange(IEnumerable<TEntity> entities)
		{
			DbSet.RemoveRange(entities);
		}


	}

}
