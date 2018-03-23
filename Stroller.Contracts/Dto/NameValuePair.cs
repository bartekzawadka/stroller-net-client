using Newtonsoft.Json;

namespace Stroller.Contracts.Dto
{
    public class NameValuePair<T>
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public T Value { get; set; }

        public NameValuePair(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
