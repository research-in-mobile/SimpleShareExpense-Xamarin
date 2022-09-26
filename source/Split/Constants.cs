using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace Split
{
    public class Constants
    {
        public const string DefaultCurrency = "CAD";

        public static TimeSpan DefaultCacheExpiration => TimeSpan.FromDays(7);

        public static string NLogConfigPath
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return "NLog.config";
                    case Device.Android:
                        return "Assets/NLog.config";
                    case Device.UWP:
                        return "Logs/NLog.config";
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static readonly HttpStatusCode[] HandledHttpStatusCodes =
        {
            HttpStatusCode.BadRequest,          // 400
            HttpStatusCode.RequestTimeout,      // 408
            HttpStatusCode.BadGateway,          // 502
            HttpStatusCode.ServiceUnavailable,  // 503
            HttpStatusCode.GatewayTimeout       // 504
        };

    }
}
