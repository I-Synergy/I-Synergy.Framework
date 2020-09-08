using ISynergy.Framework.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.Wopi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CheckFileInfoResponse : WopiResponse, 
        IFileProperties, 
        IWopiHostCapabilities,
        IFileUrlProperties,
        IBreadcrumbProperties,
        IPostMessageProperties,
        IWopiHostProperties,
        IUserMetadata,
        IUserPermissions
    {
        private class FileUrlProperties : IFileUrlProperties
        {
            public Uri CloseUrl { get; set; }
            public Uri DownloadUrl { get; set; }
            public Uri FileSharingUrl { get; set; }
            public Uri HostEditUrl { get; set; }
            public Uri HostViewUrl { get; set; }
            public Uri SignoutUrl { get; set; }

        }

        private class FileProperties : IFileProperties
        {
            public string BaseFileName { get; set; }
            public string FileExtension { get; set; }
            public int FileNameMaxLength { get; set; }
            public string LastModifiedTime { get; set; }
            public string OwnerId { get; set; }
            public string SHA256 { get; set; }
            public long Size { get; set; }
            public string UniqueContentId { get; set; }
            public string UserId { get; set; }
            public string Version { get; set; }

        }

        private class UserMetadata : IUserMetadata
        {
            public bool IsEduUser { get; set; }

            public bool LicenseCheckForEditIsEnabled { get; set; }
          
            public string UserFriendlyName { get; set; }

            public string UserInfo { get; set; }

        }

        private class UserPermissions : IUserPermissions
        {
            public bool DisablePrint { get; set; }
            public bool DisableTranslation { get; set; }
            public bool ReadOnly { get; set; }
            public bool RestrictedWebViewOnly { get; set; }
            public bool UserCanAttend { get; set; }
            public bool UserCanNotWriteRelative { get; set; }
            public bool UserCanPresent { get; set; }
            public bool UserCanRename { get; set; }
            public bool UserCanWrite { get; set; }
            public bool WebEditingDisabled { get; set; }

        }

        private IFileProperties _fileProperties;
        private IWopiHostCapabilities _wopiHostCapabilities;
        private IFileUrlProperties _fileUrlProperties;
        private IBreadcrumbProperties _breadcrumbProperties;
        private IPostMessageProperties _postMessageProperties;
        private IWopiHostProperties _wopiHostProperties;
        private IUserMetadata _userMetadata;
        private IUserPermissions _userPermissions;
        
        internal CheckFileInfoResponse()
        {
            _wopiHostCapabilities = WopiConfiguration.WopiHostCapabilities.Clone();
            _wopiHostProperties = WopiConfiguration.WopiHostProperties.Clone();
            _postMessageProperties = WopiConfiguration.PostMessageProperties.Clone();
            _breadcrumbProperties = new BreadcrumbProperties();
            _fileProperties = new FileProperties();
            _fileUrlProperties = new FileUrlProperties();
            _userMetadata = new UserMetadata()
            {
                IsEduUser = false
            };
            _userPermissions = new UserPermissions()
            {
                DisablePrint = false,
                DisableTranslation = false,
                ReadOnly = false,
                RestrictedWebViewOnly = false,
                UserCanAttend = false,
                UserCanNotWriteRelative = false,
                UserCanPresent = false,
                UserCanRename = true,
                UserCanWrite = true,  
                WebEditingDisabled = false
                
            };
        }


        #region File Properties

        [JsonProperty]
        public string BaseFileName
        {
            get
            {
                return _fileProperties.BaseFileName;
            }

            set
            {
                _fileProperties.BaseFileName = value;
            }
        }

        [JsonProperty]
        public string OwnerId
        {
            get
            {
                return _fileProperties.OwnerId;
            }

            set
            {
                _fileProperties.OwnerId = value;
            }
        }

        [JsonProperty]
        public long Size
        {
            get
            {
                return _fileProperties.Size;
            }

            set
            {
                _fileProperties.Size = value;
            }
        }

        [JsonProperty]
        public string UserId
        {
            get
            {
                return _fileProperties.UserId;
            }

            set
            {
                _fileProperties.UserId = value;
            }
        }

        [JsonProperty]
        public string Version
        {
            get
            {
                return _fileProperties.Version;
            }

            set
            {
                _fileProperties.Version = value;
            }
        }

        [JsonProperty]
        public string FileExtension
        {
            get
            {
                return _fileProperties.FileExtension;
            }

            set
            {
                _fileProperties.FileExtension = value;
            }
        }


        [JsonProperty]
        public string LastModifiedTime
        {
            get
            {
                return _fileProperties.LastModifiedTime;
            }

            set
            {
                _fileProperties.LastModifiedTime = value;
            }
        }


        [JsonProperty]
        public string SHA256
        {
            get
            {
                return _fileProperties.SHA256;
            }

            set
            {
                _fileProperties.SHA256 = value;
            }
        }

        [JsonProperty]
        public string UniqueContentId
        {
            get
            {
                return _fileProperties.UniqueContentId;
            }

            set
            {
                _fileProperties.UniqueContentId = value;
            }
        }

        #endregion

        #region Wopi Host Properties

        [JsonProperty]
        public bool AllowExternalMarketplace
        {
            get
            {
                return _wopiHostProperties.AllowExternalMarketplace;
            }

            set
            {
                _wopiHostProperties.AllowExternalMarketplace = value;
            }
        }

        [JsonProperty]
        public bool CloseButtonClosesWindow
        {
            get
            {
                return _wopiHostProperties.CloseButtonClosesWindow;
            }

            set
            {
                _wopiHostProperties.CloseButtonClosesWindow = value;
            }
        }

        [JsonProperty]
        public int FileNameMaxLength
        {
            get
            {
                return _wopiHostProperties.FileNameMaxLength;
            }

            set
            {
                _wopiHostProperties.FileNameMaxLength = value;
            }
        }

        #endregion


        #region Wopi Host Capabilities

        [JsonProperty]
        public bool SupportsCobalt
        {
            get
            {
                return _wopiHostCapabilities.SupportsCobalt;
            }

            set
            {
                _wopiHostCapabilities.SupportsCobalt = value;
            }
        }

        [JsonProperty]
        public bool SupportsContainers
        {
            get
            {
                return _wopiHostCapabilities.SupportsContainers;
            }

            set
            {
                _wopiHostCapabilities.SupportsContainers = value;
            }
        }

        [JsonProperty]
        public bool SupportsDeleteFile
        {
            get
            {
                return _wopiHostCapabilities.SupportsDeleteFile;
            }

            set
            {
                _wopiHostCapabilities.SupportsDeleteFile = value;
            }
        }

        [JsonProperty]
        public bool SupportsEcosystem
        {
            get
            {
                return _wopiHostCapabilities.SupportsEcosystem;
            }

            set
            {
                _wopiHostCapabilities.SupportsEcosystem = value;
            }
        }

        [JsonProperty]
        public bool SupportsExtendedLockLength
        {
            get
            {
                return _wopiHostCapabilities.SupportsExtendedLockLength;
            }

            set
            {
                _wopiHostCapabilities.SupportsExtendedLockLength = value;
            }
        }

        [JsonProperty]
        public bool SupportsFolders
        {
            get
            {
                return _wopiHostCapabilities.SupportsFolders;
            }

            set
            {
                _wopiHostCapabilities.SupportsFolders = value;
            }
        }

        [JsonProperty]
        public bool SupportsGetLock
        {
            get
            {
                return _wopiHostCapabilities.SupportsGetLock;
            }

            set
            {
                _wopiHostCapabilities.SupportsGetLock = value;
            }
        }

        [JsonProperty]
        public bool SupportsLocks
        {
            get
            {
                return _wopiHostCapabilities.SupportsLocks;
            }

            set
            {
                _wopiHostCapabilities.SupportsLocks = value;
            }
        }

        [JsonProperty]
        public bool SupportsRename
        {
            get
            {
                return _wopiHostCapabilities.SupportsRename;
            }

            set
            {
                _wopiHostCapabilities.SupportsRename = value;
            }
        }

        [JsonProperty]
        public bool SupportsUpdate
        {
            get
            {
                return _wopiHostCapabilities.SupportsUpdate;
            }

            set
            {
                _wopiHostCapabilities.SupportsUpdate = value;
            }
        }

        [JsonProperty]
        public bool SupportsUserInfo
        {
            get
            {
                return _wopiHostCapabilities.SupportsUserInfo;
            }

            set
            {
                _wopiHostCapabilities.SupportsUserInfo = value;
            }
        }
        #endregion

        #region User Metadata Properties
        [JsonProperty]
        public bool IsEduUser
        {
            get
            {
                return _userMetadata.IsEduUser;
            }

            set
            {
                _userMetadata.IsEduUser = value;
            }
        }

        [JsonProperty]
        public bool LicenseCheckForEditIsEnabled
        {
            get
            {
                return _userMetadata.LicenseCheckForEditIsEnabled;
            }

            set
            {
                _userMetadata.LicenseCheckForEditIsEnabled = value;
            }
        }

        [JsonProperty]
        public string UserFriendlyName
        {
            get
            {
                return _userMetadata.UserFriendlyName;
            }

            set
            {
                _userMetadata.UserFriendlyName = value;
            }
        }

        [JsonProperty]
        public string UserInfo
        {
            get
            {
                return _userMetadata.UserInfo;
            }

            set
            {
                _userMetadata.UserInfo = value;
            }
        }


        #endregion

        #region User/File Permissions Properties

        [JsonProperty]
        public bool ReadOnly
        {
            get
            {
                return _userPermissions.ReadOnly;
            }

            set
            {
                _userPermissions.ReadOnly = value;
            }
        }

        [JsonProperty]
        public bool RestrictedWebViewOnly
        {
            get
            {
                return _userPermissions.RestrictedWebViewOnly;
            }

            set
            {
                _userPermissions.RestrictedWebViewOnly = value;
            }
        }

        [JsonProperty]
        public bool UserCanAttend
        {
            get
            {
                return _userPermissions.UserCanAttend;
            }

            set
            {
                _userPermissions.UserCanAttend = value;
            }
        }

        [JsonProperty]
        public bool UserCanNotWriteRelative
        {
            get
            {
                return _userPermissions.UserCanNotWriteRelative;
            }

            set
            {
                _userPermissions.UserCanNotWriteRelative = value;
            }
        }

        [JsonProperty]
        public bool UserCanPresent
        {
            get
            {
                return _userPermissions.UserCanPresent;
            }

            set
            {
                _userPermissions.UserCanPresent = value;
            }
        }

        [JsonProperty]
        public bool UserCanRename
        {
            get
            {
                return _userPermissions.UserCanRename;
            }

            set
            {
                _userPermissions.UserCanRename = value;
            }
        }

        [JsonProperty]
        public bool UserCanWrite
        {
            get
            {
                return _userPermissions.UserCanWrite;
            }

            set
            {
                _userPermissions.UserCanWrite = value;
            }
        }

        [JsonProperty]
        public bool WebEditingDisabled
        {
            get
            {
                return _userPermissions.WebEditingDisabled;
            }

            set
            {
                _userPermissions.WebEditingDisabled = value;
            }
        }

        [JsonProperty]
        public bool DisablePrint
        {
            get
            {
                return _userPermissions.DisablePrint;
            }

            set
            {
                _userPermissions.DisablePrint = value;
            }
        }

        [JsonProperty]
        public bool DisableTranslation
        {
            get
            {
                return _userPermissions.DisableTranslation;
            }

            set
            {
                _userPermissions.DisableTranslation = value;
            }
        }



        #endregion

        #region File URL Properties

        [JsonProperty]
        public Uri CloseUrl
        {
            get
            {
                return _fileUrlProperties.CloseUrl;
            }

            set
            {
                _fileUrlProperties.CloseUrl = value;
            }
        }

        [JsonProperty]
        public Uri DownloadUrl
        {
            get
            {
                return _fileUrlProperties.DownloadUrl;
            }

            set
            {
                _fileUrlProperties.DownloadUrl = value;
            }
        }

        // Not currently supported in WopiFramework
        //[JsonProperty]
        public Uri FileSharingUrl
        {
            get
            {
                return _fileUrlProperties.FileSharingUrl;
            }

            set
            {
                _fileUrlProperties.FileSharingUrl = value;
            }
        }

        [JsonProperty]
        public Uri HostEditUrl
        {
            get
            {
                return _fileUrlProperties.HostEditUrl;
            }

            set
            {
                _fileUrlProperties.HostEditUrl = value;
            }
        }

        [JsonProperty]
        public Uri HostViewUrl
        {
            get
            {
                return _fileUrlProperties.HostViewUrl;
            }

            set
            {
                _fileUrlProperties.HostViewUrl = value;
            }
        }

        [JsonProperty]
        public Uri SignoutUrl
        {
            get
            {
                return _fileUrlProperties.SignoutUrl;
            }

            set
            {
                _fileUrlProperties.SignoutUrl = value;
            }
        }
        #endregion

        #region PostMessageProperties
        [JsonProperty]
        public bool ClosePostMessage
        {
            get
            {
                return _postMessageProperties.ClosePostMessage;
            }

            set
            {
                _postMessageProperties.ClosePostMessage = value;
            }
        }

        [JsonProperty]
        public bool EditModePostMessage
        {
            get
            {
                return _postMessageProperties.EditModePostMessage;
            }

            set
            {
                _postMessageProperties.EditModePostMessage = value;
            }
        }

        [JsonProperty]
        public bool EditNotificationPostMessage
        {
            get
            {
                return _postMessageProperties.EditNotificationPostMessage;
            }

            set
            {
                _postMessageProperties.EditNotificationPostMessage = value;
            }
        }

        // Not currently supported in WopiFramework
        //[JsonProperty]
        public bool FileSharingPostMessage
        {
            get
            {
                return _postMessageProperties.FileSharingPostMessage;
            }

            set
            {
                _postMessageProperties.FileSharingPostMessage = value;
            }
        }

        [JsonProperty]
        public string PostMessageOrigin
        {
            get
            {
                return _postMessageProperties.PostMessageOrigin;
            }

            set
            {
                _postMessageProperties.PostMessageOrigin = value;
            }
        }

        #endregion

        #region BreadcrumbProperties        

        [JsonProperty]
        public string BreadcrumbBrandName
        {
            get
            {
                return _breadcrumbProperties.BreadcrumbBrandName;
            }

            set
            {
                _breadcrumbProperties.BreadcrumbBrandName = value;
            }
        }

        [JsonProperty]
        public Uri BreadcrumbBrandUrl
        {
            get
            {
                return _breadcrumbProperties.BreadcrumbBrandUrl;
            }

            set
            {
                _breadcrumbProperties.BreadcrumbBrandUrl = value;
            }
        }

        [JsonProperty]
        public string BreadcrumbDocName
        {
            get
            {
                return _breadcrumbProperties.BreadcrumbDocName;
            }

            set
            {
                _breadcrumbProperties.BreadcrumbDocName = value;
            }
        }

        [JsonProperty]
        public string BreadcrumbFolderName
        {
            get
            {
                return _breadcrumbProperties.BreadcrumbFolderName;
            }

            set
            {
                _breadcrumbProperties.BreadcrumbFolderName = value;
            }
        }

        [JsonProperty]
        public Uri BreadcrumbFolderUrl
        {
            get
            {
                return _breadcrumbProperties.BreadcrumbFolderUrl;
            }

            set
            {
                _breadcrumbProperties.BreadcrumbFolderUrl = value;
            }
        }



      



        #endregion




        public override HttpResponseMessage ToHttpResponse()
        {
            var httpResponseMessage = base.ToHttpResponse();
            // Only serialize reponse on success
            if (StatusCode == HttpStatusCode.OK)
            {
                string jsonString = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                httpResponseMessage.Content = new StringContent(jsonString);
            }
            return httpResponseMessage;
        }

      
    }
}
