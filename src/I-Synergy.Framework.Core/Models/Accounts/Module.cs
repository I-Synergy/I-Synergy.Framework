using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Models.Accounts
{
    public class Module : ModelBase
    {
        public Module()
        {
            Module_Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the Module_Id property value.
        /// </summary>
        [Required]
        [JsonProperty]
        public Guid Module_Id
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
