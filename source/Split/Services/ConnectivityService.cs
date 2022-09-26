using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Split.Services
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly ILogService _logger;
        private bool _listeningForConnectivityChanges;

        public ConnectivityService(ILogService logger)
        {
            _logger = logger;
        }

        public void ListenForConnectivityChanges()
        {
            if (_listeningForConnectivityChanges) return;

            _listeningForConnectivityChanges = true;
            Connectivity.ConnectivityChanged += NetworkConnectivityChanged;

            _logger.Info($"Network Connection Changed: {(IsNetworkAvailable ? "Connected" : "Disconnected")}");
            NetworkConnectivityUpdate?.Invoke(this, new NetworkInfoArguments(IsNetworkAvailable));
        }

        public void StopListeningForConnectivityChanges()
        {
            Connectivity.ConnectivityChanged -= NetworkConnectivityChanged;
            _listeningForConnectivityChanges = false;
        }

        public delegate void NetworkConnectivityEventHandler(object sender, NetworkInfoArguments args);

        public event NetworkConnectivityEventHandler NetworkConnectivityUpdate;

        public void ToggleLocalOnlyMode() =>
            NetworkConnectivityUpdate?.Invoke(this, new NetworkInfoArguments(IsNetworkAvailable));

        /// <summary>
        /// Handles the change in network connectivity event and publishes the network availability to subscriber 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NetworkConnectivityChanged(object sender, ConnectivityChangedEventArgs args)
        {
            var isNetworkAvailable = args.NetworkAccess == NetworkAccess.Internet ||
                                     args.NetworkAccess == NetworkAccess.ConstrainedInternet;
            _logger.Info($"Network Connection Changed: {(isNetworkAvailable ? "Connected" : "Disconnected")}");

            NetworkConnectivityUpdate?.Invoke(this, new NetworkInfoArguments(isNetworkAvailable));
        }

        /// <summary>
        /// IsNetworkAvailable can be used to manually check the network connectivity
        /// </summary>
        public bool IsNetworkAvailable => Connectivity.NetworkAccess == NetworkAccess.Internet ||
                                          Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet;

        public bool NetworkConnectionIsAvailable() => IsNetworkAvailable;
    }

    public class NetworkInfoArguments : System.EventArgs
    {
        public bool IsNetworkAvailable { get; }

        public NetworkInfoArguments(bool isNetworkAvailable)
        {
            IsNetworkAvailable = isNetworkAvailable;
        }
    }
}
