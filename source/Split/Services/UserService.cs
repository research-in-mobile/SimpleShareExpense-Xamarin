using Split.Data;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _uow;
		private readonly ILogService _logService;


		public UserService(IUnitOfWork uow,
						ILogService logService)
		{
			_uow = uow;
			_logService = logService;
		}

		private int SaveChanges() => _uow.Complete();

		public async Task<IEnumerable<User>> GetUsersAsync() => await _uow.Users.GetAllAsync();

		public async Task<User> GetUserAsync(int userId) => await _uow.Users.GetAsync(userId);

		public async Task<IEnumerable<User>> GetUsersFromEventAsync(int eventId)
		{
			var evnt = await _uow.Events.GetAsync(eventId);

			return evnt.Guests;
		}

		public async Task<User> AddUserAsync(User user)
		{
			await _uow.Users.AddAsync(user);

			var savedChanges = SaveChanges();

			return savedChanges > 0 ? user : null;
		}

		public async Task<bool> ForceDeleteUserAsync(User user)
		{
			await _uow.Users.RemoveAsync(user.UserId);

			var savedChanges = SaveChanges();

			return savedChanges > 0;
		}

		public async Task<bool> DeleteUserAsync(User user)
		{
			//TODO: Check and remove only if user is not in saved contacts

			await _uow.Users.RemoveAsync(user.UserId);

			var savedChanges = SaveChanges();

			return savedChanges > 0;
		}

		public async Task<User> UpdateUserAsync(User user)
		{
			var updatedUser = await _uow.Users.GetAsync(user.UserId);

			updatedUser = user;

			var savedChanges = SaveChanges();

			return savedChanges > 0 ? updatedUser : null;
		}

		
	}
}
