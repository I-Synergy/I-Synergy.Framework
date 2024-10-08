﻿using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class NetworkUtility.
/// </summary>
public static class NetworkUtility
{
    /// <summary>
    /// Determines whether [is internet connection available].
    /// </summary>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public static Task<bool> IsInternetConnectionAvailable()
    {
        var ping = new Ping();
        var reply = ping.Send("8.8.8.8", 1000, new byte[32], new PingOptions());

        if (reply.Status == IPStatus.Success)
            return Task.FromResult(true);

        return Task.FromResult(false);
    }

    /// <summary>
    /// Determines whether [is valid ip] [the specified test address].
    /// </summary>
    /// <param name="TestAddress">The test address.</param>
    /// <returns><c>true</c> if [is valid ip] [the specified test address]; otherwise, <c>false</c>.</returns>
    public static bool IsValidIP(string TestAddress)
    {
        return Regex.Match(TestAddress, "^(0[0-7]{10,11}|0(x|X)[0-9a-fA-F]{8}|(\\b4\\d{8}[0-5]\\b|\\b[1-3]?\\d{8}\\d?\\b)|((2[0-5][0-5]|1\\d{2}|[1-9]\\d?)|(0(x|X)[0-9a-fA-F]{2})|(0[0-7]{3}))(\\.((2[0-5][0-5]|1\\d{2}|\\d\\d?)|(0(x|X)[0-9a-fA-F]{2})|(0[0-7]{3}))){3})$", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Success;
    }

    /// <summary>
    /// The domain reg ex
    /// </summary>
    private const string DomainRegEx = "^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\\.(xn--)?([a-z0-9]{1,61}|[a-z0-9-]{1,30}\\.[a-z]{2,})$";

    /// <summary>
    /// Determines whether [is valid domain] [the specified string domain].
    /// </summary>
    /// <param name="strDomain">The string domain.</param>
    /// <returns><c>true</c> if [is valid domain] [the specified string domain]; otherwise, <c>false</c>.</returns>
    public static bool IsValidDomain(string strDomain)
    {
        return Regex.Match(strDomain, DomainRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)).Success;
    }

    /// <summary>
    /// The URL reg ex
    /// </summary>
    private const string URLRegEx = "#([a-z]([a-z]|\\d|\\+|-|\\.)*):(\\/\\/(((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:)*@)?((\\[(|(v[\\da-f]{1,}\\.(([a-z]|\\d|-|\\.|_|~)|[!\\$&'\\(\\)\\*\\+,;=]|:)+))\\])|((\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5]))|(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=])*)(:\\d*)?)(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*|(\\/((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)+(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*)?)|((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)+(\\/(([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*)|((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)){0})(\\?((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|[\\xE000-\\xF8FF]|\\/|\\?)*)?(\\#((([a-z]|\\d|-|\\.|_|~|[\\x00A0-\\xD7FF\\xF900-\\xFDCF\\xFDF0-\\xFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|\\/|\\?)*)?#iS";

    /// <summary>
    /// Determines whether [is valid URL] [the specified string URL].
    /// </summary>
    /// <param name="strURL">The string URL.</param>
    /// <returns><c>true</c> if [is valid URL] [the specified string URL]; otherwise, <c>false</c>.</returns>
    public static bool IsValidURL(Uri strURL)
    {
        return Regex.Match(strURL.AbsoluteUri, URLRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)).Success;
    }

    /// <summary>
    /// The mail reg ex
    /// </summary>
    private const string MailRegEx = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

    /// <summary>
    /// Determines whether [is valid e mail] [the specified string mail address].
    /// </summary>
    /// <param name="strMailAddress">The string mail address.</param>
    /// <returns><c>true</c> if [is valid e mail] [the specified string mail address]; otherwise, <c>false</c>.</returns>
    public static bool IsValidEMail(string strMailAddress)
    {
        return Regex.Match(strMailAddress, MailRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)).Success;
    }

    /// <summary>
    /// Determines whether [is URL reachable] [the specified URL].
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The method.</param>
    /// <returns><c>true</c> if [is URL reachable] [the specified URL]; otherwise, <c>false</c>.</returns>
    public static async Task<bool> IsUrlReachableAsync(Uri url, string method = "GET")
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(1);
            using var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}