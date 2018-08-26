namespace ISynergy.Base
{
    public abstract class ConfigBase
    {
        /// <summary>
        /// Gets or sets the url_api property value.
        /// </summary>
        public static string url_api
        {
            get
            {
#if DEBUG_REMOTE
                return @"https://app-test.i-synergy.nl/api/";
#elif DEBUG
                return @"http://localhost.:5000/api/";
#else
                return @"https://app.i-synergy.nl/api/";
#endif
            }
        }

        /// <summary>
        /// Gets or sets the url_token property value.
        /// </summary>
        public static string url_token
        {
            get
            {
#if DEBUG_REMOTE
                return @"https://app-test.i-synergy.nl/oauth/token";
#elif DEBUG
                return @"http://localhost.:5000/oauth/token";
#else
                return @"https://app.i-synergy.nl/oauth/token";
#endif
            }
        }

        /// <summary>
        /// Gets or sets the url_account property value.
        /// </summary>
        public static string url_account
        {
            get
            {
#if DEBUG_REMOTE
                return @"https://app-test.i-synergy.nl/account/";
#elif DEBUG
                return @"http://localhost.:5000/account/";
#else
                return @"https://app.i-synergy.nl/account/";
#endif
            }
        }

        /// <summary>
        /// Gets or sets the url_web property value.
        /// </summary>
        public static string url_web
        {
            get
            {
#if DEBUG_REMOTE
                return @"http://test.i-synergy.nl/";
#elif DEBUG
                return @"http://localhost.:5000/";
#else
                return @"http://www.i-synergy.nl/";
#endif
            }
        }
    }
}