using Prism.Services;
using Split.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
    public class ErrorManagementService : IErrorManagementService
    {
        private readonly ILogService _logService;
        private readonly IPageDialogService _pageDialogService;
        //private readonly IBugReportingService _bugReportingService;

        public ErrorManagementService(
            ILogService logService,
            IPageDialogService pageDialogService)
        {
            _logService = logService;
            _pageDialogService = pageDialogService;
            //_bugReportingService = bugReportingService;
        }

        public void HandleError(Exception ex)
        {
            _logService.Error(ex, ex.Message);
            //_bugReportingService.LogError(ex.ToString());
            //Crashes.TrackError(ex);

            // only handle/log errors when the app is online
            //if (!_connectivityService.IsNetworkAvailable) return;

            if (Secrets.AppId == "com.researchinmobile.split")
            {
                _pageDialogService.DisplayAlertAsync("Error", ex.ToString(), "OK")
                    .FireAndForgetSafeAsync(HandleErrorInternal);
            }
            else
            {
                DisplayBugReportAlert().FireAndForgetSafeAsync(HandleErrorInternal);
            }
        }

        private void HandleErrorInternal(Exception ex)
        {
            _pageDialogService.DisplayAlertAsync("Error", ex.ToString(), "OK").FireAndForgetSafeAsync(HandleErrorInternal);
        }

        private async Task DisplayBugReportAlert()
        {
            bool result = await _pageDialogService.DisplayAlertAsync("Oops, an error has occurred", "An unexpected error has occured. Would you like to report a bug?", "Yes", "No");
            //if (result) _bugReportingService.ReportBug();
        }
    }
}
