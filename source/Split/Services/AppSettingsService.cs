using Split.Data;
using Split.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace Split.Services
{
    public class AppSettingsService : IAppSettingsService
    {
		private readonly ILogService _logService;
		private readonly IUserService _userService;

		public AppSettingsService(
			ILogService logService, 
			IUserService userService)
		{
			_logService = logService;
			_userService = userService;
		}

		private string _defaultCurrency = "USD";
		public string DefaultCurrency
		{
			get
			{
				var savedDefault = Preferences.Get("default_currency", string.Empty);

				return savedDefault == string.Empty? _defaultCurrency : savedDefault;
			}
			set => Preferences.Set("default_currency", value);
		}

		public async Task<User> GetAppUserAsync()
		{
			try
			{
				var savedAppUserGUID = Preferences.Get("user_guid", Guid.Empty.ToString());

				var users = await _userService.GetUsersAsync();
				var user = users.FirstOrDefault(u => u.GUID.ToString() == savedAppUserGUID);

				return user;
			}
			catch(Exception ex)
			{
				return null;
			}
		}

		public async Task<User> AddAppUserAsync(User user)
		{
			try
			{
				user.UserId = 1;
				Preferences.Set("user_id", user.UserId);
				Preferences.Set("user_guid", user.GUID.ToString());

				var addedUser = await _userService.AddUserAsync(user);

				return addedUser; 
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		//private int SaveChanges() => _uow.Complete();

		//public async Task<IEnumerable<AppSettings>> GetAppSettingsAsync() => null;//await _uow.AppSettings.GetAllAsync();

		//public async Task<AppSettings> GetAppSettingsAsync(int id) => null; //await _uow.AppSettings.GetAsync(id);

		//public async Task<AppSettings> AddAppSettingsAsync(AppSettings setting)
		//{
		//	//await _uow.AppSettings.AddAsync(setting);

		//	var savedChanges = SaveChanges();

		//	return savedChanges > 0 ? setting : null;
		//}

		//public async Task<bool> DeleteAppSettingsAsync(AppSettings setting)
		//{
		//	await _uow.Events.RemoveAsync(setting.AppSettingsId);
		//	var savedChanges = SaveChanges();

		//	return savedChanges > 0;
		//}

		//public async Task<AppSettings> UpdateAppSettingsAsync(AppSettings newEvent)
		//{
		//	SaveChanges();

		//	return newEvent;
		//}

	}
}
