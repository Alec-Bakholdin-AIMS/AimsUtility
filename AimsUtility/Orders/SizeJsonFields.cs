using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    public class SizeJsonFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sizeName;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? orderQuantity;
    }
}