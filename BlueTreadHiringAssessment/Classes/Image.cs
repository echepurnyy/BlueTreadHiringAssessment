using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace BlueTreadHiringAssessment.Classes
{
    public class Image
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("ratio")]
        public string Ratio{ get;set; }
        [JsonPropertyName("width")]
        public int Width{ get; set; }
        [JsonPropertyName("height")]
        public int Height {  get; set; }
        [JsonPropertyName("attribution")]
        public string Attribution {  get; set; }
    }
}
