using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Services
{
    public class BaseViewModelServiceProvider : IBaseViewModelServiceProvider
    {
        public INavigationService NavigationService { get; }
        public IErrorManagementService ErrorManagementService { get; }
        public ILogService LogService { get; }


        public BaseViewModelServiceProvider(
            INavigationService navigationService,
            IErrorManagementService errorManagementService,
            ILogService logService)
        {
            ErrorManagementService = errorManagementService;
            LogService = logService;
            NavigationService = navigationService;
        }
    }
}
