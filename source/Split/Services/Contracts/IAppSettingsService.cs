using Split.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Split.Services
{
    public interface IAppSettingsService
    {
        string DefaultCurrency { get; set; }
		Task<User> GetAppUserAsync();
		Task<User> AddAppUserAsync(User user);

		//Task<IEnumerable<AppSettings>> GetAppSettingsAsync();
		//Task<AppSettings> GetAppSettingsAsync(int id);
		//Task<AppSettings> AddAppSettingsAsync(AppSettings setting);
		//Task<bool> DeleteAppSettingsAsync(AppSettings setting);
		//Task<AppSettings> UpdateAppSettingsAsync(AppSettings setting);
	}
}