namespace ISynergy.Framework.Wopi.Models
{
    /// <summary>
    /// Represented the valid WOPI actions from WOPI Discovery
    /// </summary>
    public class WopiAction
    {
        public string app { get; set; }
        public string favIconUrl { get; set; }
        public bool checkLicense { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
        public string progid { get; set; }
        public string requires { get; set; }
        public bool? @default { get; set; }
        public string urlsrc { get; set; }
    }
}
