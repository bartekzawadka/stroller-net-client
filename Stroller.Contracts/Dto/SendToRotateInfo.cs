using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public class SendToRotateInfo : CapturingInfo
    {
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
    }
}
