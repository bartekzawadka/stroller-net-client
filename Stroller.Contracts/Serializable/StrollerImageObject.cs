using System;
using Newtonsoft.Json;

namespace Stroller.Contracts.Serializable
{
    public class StrollerImageObject
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "thumnail")]
        public string Thumbnail { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "chunks")]
        public StrollerChunkItem[] Chunks { get; set; }
    }
}
