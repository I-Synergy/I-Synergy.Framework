using System.Collections.Generic;

namespace ISynergy.Framework.Wopi
{
    public class Constants
    {
        //WOPI protocol constants
        public const string WOPI_BASE_PATH = @"/wopi/";
        public const string WOPI_CHILDREN_PATH = @"/children";
        public const string WOPI_CONTENTS_PATH = @"/contents";
        public const string WOPI_FILES_PATH = @"files/";
        public const string WOPI_FOLDERS_PATH = @"folders/";

        public const string HttpClientDefault = "Default";

        public const string WopiProof = "WopiProof";
        public const string DiscoveryData = "DiscoveryData";
    }

    public class WopiUrlPlaceholders
    {
        public const string BusinessUser = "BUSINESS_USER";
        public const string DataCalculationLanguageCode = "DC_LLCC";
        public const string DisableAsync = "DISABLE_ASYNC";
        public const string DisableBroadcast = "DISABLE_BROADCAST";
        public const string DisableChat = "DISABLE_CHAT";
        public const string Embedded = "EMBEDDED";
        public const string FullScreen = "FULLSCREEN";
        public const string PerfStats = "PERFSTATS";
        public const string Recording = "RECORDING";
        public const string ThemeId = "THEME_ID";
        public const string UiLanguageCode = "UI_LLCC";

        public const string BUSINESS_USER = "<IsLicensedUser=BUSINESS_USER&>";
        public const string DC_LLCC = "<rs=DC_LLCC&>";
        public const string DISABLE_ASYNC = "<na=DISABLE_ASYNC&>";
        public const string DISABLE_CHAT = "<dchat=DISABLE_CHAT&>";
        public const string DISABLE_BROADCAST = "<vp=DISABLE_BROADCAST&>";
        public const string EMBDDED = "<e=EMBEDDED&>";
        public const string FULLSCREEN = "<fs=FULLSCREEN&>";
        public const string PERFSTATS = "<showpagestats=PERFSTATS&>";
        public const string RECORDING = "<rec=RECORDING&>";
        public const string THEME_ID = "<thm=THEME_ID&>";
        public const string UI_LLCC = "<ui=UI_LLCC&>";
        public const string VALIDATOR_TEST_CATEGORY = "<testcategory=VALIDATOR_TEST_CATEGORY>";

        public static List<string> Placeholders = new List<string>() { BUSINESS_USER,
            DC_LLCC, DISABLE_ASYNC, DISABLE_CHAT, DISABLE_BROADCAST,
            EMBDDED, FULLSCREEN, PERFSTATS, RECORDING, THEME_ID, UI_LLCC,
            VALIDATOR_TEST_CATEGORY};

        /// <summary>
        /// Sets a specific WOPI URL placeholder with the correct value
        /// Most of these are hard-coded in this WOPI implementation
        /// </summary>
        public static string GetPlaceholderValue(string placeholder)
        {
            var ph = placeholder.Substring(1, placeholder.IndexOf("="));
            string result = "";
            switch (placeholder)
            {
                case BUSINESS_USER:
                    result = ph + "1";
                    break;
                case DC_LLCC:
                case UI_LLCC:
                    result = ph + "1033";
                    break;
                case DISABLE_ASYNC:
                case DISABLE_BROADCAST:
                case EMBDDED:
                case FULLSCREEN:
                case RECORDING:
                case THEME_ID:
                    // These are all broadcast related actions
                    result = ph + "true";
                    break;
                case DISABLE_CHAT:
                    result = ph + "false";
                    break;
                case PERFSTATS:
                    result = ""; // No documentation
                    break;
                case VALIDATOR_TEST_CATEGORY:
                    result = ph + "OfficeOnline"; //This value can be set to All, OfficeOnline or OfficeNativeClient to activate tests specific to Office Online and Office for iOS. If omitted, the default value is All.  
                    break;
                default:
                    result = "";
                    break;
            }

            return result;
        }
    }
}
