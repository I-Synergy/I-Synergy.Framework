﻿using Newtonsoft.Json;

namespace NugetUnlister.Common.Models
{
    public class NugetResponse
    {
        [JsonProperty("versions")]
        public string[] Versions { get; set; }
    }
}