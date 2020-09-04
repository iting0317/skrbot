using Newtonsoft.Json;

namespace Skrbot.WebApi.Models
{
    public class LocationMessage
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        public LocationMessage()
        {
            Type = "location";
        }
    }
}