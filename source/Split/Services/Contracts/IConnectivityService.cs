using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Services
{
    public interface IConnectivityService
    {
        event ConnectivityService.NetworkConnectivityEventHandler NetworkConnectivityUpdate;

        bool IsNetworkAvailable { get; }
        void ToggleLocalOnlyMode();
        bool NetworkConnectionIsAvailable();
        void ListenForConnectivityChanges();
        void StopListeningForConnectivityChanges();
    }
}
