using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlueTreadHiringAssessment.Classes
{
    public class Event: ICloneable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get;set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Date { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images {  get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
