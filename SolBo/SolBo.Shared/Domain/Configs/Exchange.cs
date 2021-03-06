﻿using System.Text.Json.Serialization;

namespace SolBo.Shared.Domain.Configs
{
    public class Exchange
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        [JsonIgnore]
        public bool IsInTestMode
            => string.IsNullOrWhiteSpace(ApiKey) || string.IsNullOrWhiteSpace(ApiSecret);
    }
}