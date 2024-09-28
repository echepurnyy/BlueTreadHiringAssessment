using System.Text.Json.Serialization;

namespace BlueTreadHiringAssessment.Classes
{
    public class Venue
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("address")]
        public Dictionary<string,string> Address { get; set; }
        [JsonPropertyName("city")]
        public Dictionary<string, string> City { get; set; }
        [JsonPropertyName("state")]
        public Dictionary<string, string> State { get; set; }
        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }
    }
}
