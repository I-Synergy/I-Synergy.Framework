using System;

namespace ISynergy.Framework.Wopi.Models
{
    internal interface IBreadcrumbProperties
    {
        string BreadcrumbBrandName { get; set; }
        Uri BreadcrumbBrandUrl { get; set; }
        string BreadcrumbDocName { get; set; }
        string BreadcrumbFolderName { get; set; }
        Uri BreadcrumbFolderUrl { get; set; }
    }
}
