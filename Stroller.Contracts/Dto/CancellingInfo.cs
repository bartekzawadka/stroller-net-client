using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public class CancellingInfo : CapturingInfo
    {
        [JsonProperty(PropertyName = "force")]
        public bool Force { get; set; }
    }
}
