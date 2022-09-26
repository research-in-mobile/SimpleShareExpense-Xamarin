using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
	public interface IUserService
	{
		Task<IEnumerable<User>> GetUsersAsync();
		Task<IEnumerable<User>> GetUsersFromEventAsync(int eventId);
		Task<User> GetUserAsync(int userId);

		Task<User> AddUserAsync(User user);
		Task<bool> DeleteUserAsync(User user);
		Task<bool> ForceDeleteUserAsync(User user);
		Task<User> UpdateUserAsync(User user);

	}
}