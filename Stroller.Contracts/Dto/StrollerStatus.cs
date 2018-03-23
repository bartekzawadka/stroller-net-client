using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public class StrollerStatus
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
