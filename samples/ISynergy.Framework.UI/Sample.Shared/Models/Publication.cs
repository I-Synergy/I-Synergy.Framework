using ISynergy.Framework.Core.Collections;
using Sample.Enumerations;
using System;

namespace Sample.Models
{
    public class Publication : TreeNode<Guid, PublicationItem>, IDisposable
    {
        /// <summary>
        /// Gets or sets the PublicationId property value.
        /// </summary>
        public Guid PublicationId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
        public string Version
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the LanguageId property value.
        /// </summary>
        public int LanguageId
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Author property value.
        /// </summary>
        public string Author
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Publication()
            : base()
        {
            PublicationId = Guid.NewGuid();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="version">Document version</param>
        /// <param name="languageId">Defauls to English (2)</param>
        public Publication(string description, string version, int languageId)
            : this()
        {
            Version = version;
            LanguageId = languageId;
            Data = new PublicationItem(PublicationId, description, PublicationItemTypes.Chapter);
        }
    }
}
