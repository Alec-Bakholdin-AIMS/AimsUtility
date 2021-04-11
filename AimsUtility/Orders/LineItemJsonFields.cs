using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    /// <summary>
    /// Line Item Json Fields
    /// </summary>
    public class LineItemJsonFields
    {
        /// <summary>
        /// Style Color ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string styleColorID;
        /// <summary>
        /// Warehouse ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string warehouseID;
        /// <summary>
        /// Commission Percentage
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? commissionPercentage;
        /// <summary>
        /// Price
        /// </summary>
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