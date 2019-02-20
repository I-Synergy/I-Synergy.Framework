using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISynergy.Handlers
{
    public static class Network
    {
        public static Task<bool> IsInternetConnectionAvailable()
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("8.8.8.8", 1000, new byte[32], new PingOptions());

            if(reply.Status == IPStatus.Success)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public static bool IsValidIP(string TestAddress)
        {
            return Regex.Match(TestAddress, "^(0[0-7]{10,11}|0(x|X)[0-9a-fA-F]{8}|(\\b4\\d{8}[0-5]\\b|\\b[1-3]?\\d{8}\\d?\\b)|((2[0-5][0-5]|1\\d{2}|[1-9]\\d?)|(0(x|X)[0-9a-fA-F]{2})|(0[0-7]{3}))(\\.((2[0-5][0-5]|1\\d{2}|\\d\\d?)|(0(x|X)[0-9a-fA-F]{2})|(0[0-7]{3}))){3})$").Success;
        }

        private const string DomainRegEx = "^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\\.(xn--)?([a-z0-9]{1,61}|[a-z0-9-]{1,30}\\.[a-z]{2,})$";

        public static bool IsValidDomain(string strDomain)
        {
            return Regex.Match(strDomain, DomainRegEx).Success;
        }

        private const string URLRegEx = "#([a-z]([a-z]|\\d|\\+|-|\\.)*):(\\/\\/(((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:)*@)?((\\[(|(v[\\da-f]{1,}\\.(([a-z]|\\d|-|\\.|_|~)|[!\\$&'\\(\\)\\*\\+,;=]|:)+))\\])|((\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5]))|(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=])*)(:\\d*)?)(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*|(\\/((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)+(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*)?)|((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)+(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*)|((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)){0})(\\?((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|[\\xE000-\\xF8FF]|\\/|\\?)*)?(\\#((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|\\/|\\?)*)?#iS";

        public static bool IsValidURL(string strURL)
        {
            return Regex.Match(strURL, URLRegEx).Success;
        }

        private const string MailRegEx = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

        public static bool IsValidEMail(string strMailAddress)
        {
            return Regex.Match(strMailAddress, MailRegEx).Success;
        }

        public static string ValidateUrl(string strUri)
        {
            if (!strUri.ToLower().StartsWith("http://"))
                strUri = "http://" + strUri;
            return strUri;
        }
    }
}