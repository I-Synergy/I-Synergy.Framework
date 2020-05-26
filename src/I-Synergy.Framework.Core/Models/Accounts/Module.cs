using ISynergy.Framework.Core.Data;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.Core.Models.Accounts
{
    public class Module : ModelBase
    {
        public Module()
        {
            ModuleId = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the Module_Id property value.
        /// </summary>
        [Required]
        [JsonProperty]
        public Guid ModuleId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Code property value.
        /// </summary>
        [Required]
        [StringLength(32)]
        [JsonProperty]
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Description property value.
        /// </summary>
        [Required]
        [StringLength(128)]
        [JsonProperty]
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
