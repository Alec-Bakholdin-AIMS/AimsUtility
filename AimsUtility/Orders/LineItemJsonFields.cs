using Newtonsoft.Json;

namespace DataTableUtil.Orders
{
    public class LineItemJsonFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string styleColorID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string warehouseID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? commissionPercentage;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? price;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? lineItemDiscountPercentage;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lineItemDiscountReasonID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lineNote;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Prepack prepack;
    }

    public class Prepack
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prepackID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? prepackQuantity;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? pieces;
    }
}