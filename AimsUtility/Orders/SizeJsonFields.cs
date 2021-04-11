using Newtonsoft.Json;

namespace DataTableUtil.Orders
{
    public class SizeJsonFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sizeName;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? orderQuantity;
    }
}