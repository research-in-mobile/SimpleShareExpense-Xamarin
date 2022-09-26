using Split.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
	public interface IRepository<TEntity> where TEntity : class
	{
		TEntity Get(int id);
		Task<TEntity> GetAsync(int id);
		IEnumerable<TEntity> GetAll();
		Task<IEnumerable<TEntity>> GetAllAsync(bool forceRefresh = false);

		IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
		TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

		void Add(TEntity entity);
		void AddRange(IEnumerable<TEntity> entities);
		Task<bool> AddAsync(TEntity item);
		Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

		void Remove(TEntity entity);
		Task<bool> RemoveAsync(int id);
		void RemoveRange(IEnumerable<TEntity> entities);


	}
}
