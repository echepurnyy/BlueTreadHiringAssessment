﻿using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlueTreadHiringAssessment.Classes
{
    public class Event
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get;set; }

        [JsonPropertyName("externalLinks")]
        public string ExternalLinks { get; set; }
        [JsonPropertyName("images")]
        public List<Image> Images {  get; set; }

    }
}
