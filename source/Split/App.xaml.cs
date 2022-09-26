using Prism;
using Prism.Ioc;

using Split.Data;
using Split.Helpers;
using Split.Services;
using Split.ViewModels;
using Split.Views;
using System;
using System.IO;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Split.Entities;
using System.Linq;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Split
{
	public partial class App
	{
		public static UnitOfWork UoW;
		public static User AppUser { get; set; }

		public App() : this(null) { }

		public App(IPlatformInitializer initializer) : base(initializer) { }

		protected override async void OnInitialized()
		{
			InitializeComponent();

			AppCenter.Start("android=" + Secrets.AppCenterKey_Android + ";" +
							"ios=" + Secrets.AppCenterKey_iOS + ";" +
							"uwp=" + Secrets.AppCenterKey_UWP + ";",
				typeof(Analytics), typeof(Crashes));

			var userId = Preferences.Get("user_id", -1);

			if (userId == -1)
			{
				await NavigationService.NavigateAsync("SetupPage");
			}
			else
			{
				await NavigationService.NavigateAsync("NavigationPage/HubPage");
			}
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			RegisterServices(containerRegistry);

			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<HubPage, HubPageViewModel>();
			containerRegistry.RegisterForNavigation<BuildEventPage, BuildEventPageViewModel>();
			containerRegistry.RegisterForNavigation<EventPage, EventPageViewModel>();
			containerRegistry.RegisterForNavigation<BuildExpensePage, BuildExpensePageViewModel>();
			containerRegistry.RegisterForNavigation<EventSummaryPage, EventSummaryPageViewModel>();
			containerRegistry.RegisterForNavigation<SetupPage, SetupPageViewModel>();
		}

		protected void RegisterServices(IContainerRegistry containerRegistry)
		{
			UoW = new UnitOfWork(new AppDbContext());
			containerRegistry.RegisterInstance<IUnitOfWork>(UoW);

			containerRegistry.Register<IBaseViewModelServiceProvider, BaseViewModelServiceProvider>();
			containerRegistry.Register<ICacheProvider, CacheProvider>();
			containerRegistry.Register<IErrorManagementService, ErrorManagementService>();


			containerRegistry.RegisterSingleton<IEventService, EventService>();
			containerRegistry.RegisterSingleton<IExpenseService, ExpenseService>();
			containerRegistry.RegisterSingleton<IUserService, UserService>();
			containerRegistry.RegisterSingleton<IAppSettingsService, AppSettingsService>();
			containerRegistry.RegisterSingleton<ILogService, LogService>();
			//containerRegistry.RegisterSingleton<IRESTService, RESTService>();
			//containerRegistry.RegisterSingleton<IConnectivityService, ConnectivityService>();
		}

	}
}
