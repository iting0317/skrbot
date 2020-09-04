using Newtonsoft.Json;

namespace Skrbot.WebApi.Models
{
    public class ChannelData
    {
        [JsonProperty(PropertyName = "payload")]
        public ChannelDataMessage Payload { get; set; }
    }
}