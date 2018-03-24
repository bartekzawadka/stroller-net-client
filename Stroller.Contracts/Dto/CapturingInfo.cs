using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public abstract class CapturingInfo
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
