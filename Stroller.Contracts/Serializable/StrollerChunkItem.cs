using Newtonsoft.Json;

namespace Stroller.Contracts.Serializable
{
    public class StrollerChunkItem
    {
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
    }
}
