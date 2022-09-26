using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Split.Data;
using Split.Entities;
using Split.Helpers;
using Split.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Split.ViewModels
{
	public class HubPageViewModel : BaseViewModel
	{
		private readonly IEventService _eventService;
		private readonly IPageDialogService _pageDialogService;
		private readonly IAppSettingsService _appSettingsService;

		private User _appUser;
		public User AppUser
		{
			get => _appUser;
			set => SetProperty(ref _appUser, value);
		}

		private ObservableCollection<Event> _events;
		public ObservableCollection<Event> Events
		{
			get => _events;
			set => SetProperty(ref _events, value);
		}

		public DelegateCommand AddEventCommand { get; set; }
		public DelegateCommand<object> EventSelectedCommand { get; set; }
		public DelegateCommand<object> DeleteEventCommand { get; set; }

		public DelegateCommand SigninCommand { get; set; }

		public HubPageViewModel(
			IBaseViewModelServiceProvider baseServiceProvider,
			IPageDialogService pageDialogService,
			IEventService eventService,
			IAppSettingsService appSettingsService)
			: base(baseServiceProvider)
		{
			Title = "Hub";

			_eventService = eventService;
			_pageDialogService = pageDialogService;
			_appSettingsService = appSettingsService;

			Task.Run(async () =>
			{
				var events = await _eventService?.GetEventsAsync();
				Events = new ObservableCollection<Event>(events);
			});

			EventSelectedCommand = new DelegateCommand<object>(async (evnt) =>
			{
				if (IsBusy)
					return;

				using (Busy())
				{
					var parameters = new NavigationParameters();
					parameters.Add("Event", evnt);

					await NavigationService.NavigateAsync("EventPage", parameters, useModalNavigation: false);
				}
			});

			AddEventCommand = new DelegateCommand(async () =>
			{
				if (IsBusy)
					return;

				using (Busy())
				{
					var result = await NavigationService.NavigateAsync("BuildEventPage", useModalNavigation: true);
				}
			});

			DeleteEventCommand = new DelegateCommand<object>(async (obj) =>
			{
				if (IsBusy)
					return;

				using (Busy())
				{
					var evnt = obj as Event;
					var isDeleted = await _eventService.DeleteEventAsync(evnt);

					if(isDeleted)
						Events.Remove(evnt);
				}
			});

			SigninCommand = new DelegateCommand(async () =>
			{
				if (IsBusy)
					return;

				using (Busy())
				{
					await _pageDialogService.DisplayAlertAsync("Coming Soon", "Sorry, This feature is not available at the moment.", "Close");
				}
			});

			EventHelper.DataContextChanged += async (s, e) => await LoadAsync();

			LogService.Info("App Loaded");
		}

		public override void Destroy()
		{
			base.Destroy();

			EventHelper.DataContextChanged -= async (s, e) => await LoadAsync();
		}

		public override async void OnNavigatedTo(INavigationParameters parameters)
		{
			base.OnNavigatedTo(parameters);

			await LoadAsync();
		}

		private async Task LoadAsync()
		{
			var events = await _eventService?.GetEventsAsync();
			Events = new ObservableCollection<Event>(events);

			AppUser = await _appSettingsService?.GetAppUserAsync();
		}
	}
}
