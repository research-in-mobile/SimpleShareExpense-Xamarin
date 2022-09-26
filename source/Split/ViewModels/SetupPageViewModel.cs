using Prism.Commands;
using Prism.Mvvm;
using Split.Data;
using Split.Entities;
using Split.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace Split.ViewModels
{
    public class SetupPageViewModel : BaseViewModel
    {
        private readonly IAppSettingsService _appSettingsService;
        private AppSettings _appSettings = new AppSettings();

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _defaultCurrency = "CAD";
        public string DefaultCurrency
        {
            get => _defaultCurrency;
            set => SetProperty(ref _defaultCurrency, value);
        }

        public DelegateCommand CompleteCommand { get; set; }

        public SetupPageViewModel(IBaseViewModelServiceProvider baseServiceProvider,
            IAppSettingsService appSettingsService)
            : base(baseServiceProvider)
        {
            Title = "Simply Share";

            _appSettingsService = appSettingsService;

            CompleteCommand = new DelegateCommand(async () =>
            {
                await _appSettingsService.AddAppUserAsync(new User(_name));

                Preferences.Set("default_currency", DefaultCurrency);

                await NavigationService.NavigateAsync("/NavigationPage/HubPage");
            });
        }
    }
}
