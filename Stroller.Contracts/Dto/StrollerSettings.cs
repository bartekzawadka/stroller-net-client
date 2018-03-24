using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public class StrollerSettings
    {
        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }

        [JsonProperty(PropertyName = "stepAngle")]
        public decimal StepAngle { get; set; }

        [JsonProperty(PropertyName = "cameras")]
        public string[] Cameras { get; set; }

        [JsonProperty(PropertyName = "camera")]
        public string Camera { get; set; }

        [JsonProperty(PropertyName = "directions")]
        public NameValuePair<string>[] Directions { get; set; }

        [JsonProperty(PropertyName = "isLargeImagesMode")]
        public bool IsLargeImagesMode { get; set; }
    }
}
