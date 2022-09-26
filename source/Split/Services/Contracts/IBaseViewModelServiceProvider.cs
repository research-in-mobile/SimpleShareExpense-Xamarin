using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Services
{
    public interface IBaseViewModelServiceProvider
    {
        INavigationService NavigationService { get; }
        IErrorManagementService ErrorManagementService { get; }
        ILogService LogService { get; }
    }
}
