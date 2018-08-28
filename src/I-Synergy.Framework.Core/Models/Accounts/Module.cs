using ISynergy.Models.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISynergy.Models.Accounts
{
    public class Module : BaseModel
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
