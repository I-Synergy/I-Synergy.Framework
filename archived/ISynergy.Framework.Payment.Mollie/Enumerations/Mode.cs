﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// Enum Mode
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Mode
    {
        /// <summary>
        /// The live
        /// </summary>
        [EnumMember(Value = "live")]
        Live,
        /// <summary>
        /// The test
        /// </summary>
        [EnumMember(Value = "test")]
        Test
    }
}
