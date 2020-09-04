using Newtonsoft.Json;

namespace Skrbot.WebApi.Models
{
    public class ChannelDataMessage
    {
        [JsonProperty(PropertyName = "message")]
        public LocationMessage Message { get; set; }
    }
}