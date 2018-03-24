using Newtonsoft.Json;
using Stroller.Contracts.Enums;

namespace Stroller.Contracts.Dto
{
    public class CapturingProgressInfo : CapturingInfo
    {
        [JsonProperty(PropertyName = "status")]
        public AcquisitionStatusType Status { get; set; }

        [JsonProperty(PropertyName = "progress")]
        public double Progress { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long? ImageId { get; set; }
    }
}
