using System;

namespace ISynergy.Framework.Wopi.Models
{
    public class BreadcrumbProperties : IBreadcrumbProperties
    {
        public string BreadcrumbBrandName { get; set; }
        public Uri BreadcrumbBrandUrl { get; set; }
        public string BreadcrumbDocName { get; set; }
        public string BreadcrumbFolderName { get; set; }
        public Uri BreadcrumbFolderUrl { get; set; }
    }
}
