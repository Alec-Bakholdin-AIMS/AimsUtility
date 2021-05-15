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

        /// <summary>
        /// Line Item Discount Percentage
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? lineItemDiscountPercentage;
        /// <summary>
        /// Line Item Discount Reason ID (found through API)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lineItemDiscountReasonID;
        /// <summary>
        /// Line Note
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lineNote;
        /// <summary>
        /// Prepack Object
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Prepack prepack;
    }

    /// <summary>
    /// Prepack Class
    /// </summary>
    public class Prepack
    {
        /// <summary>
        /// Prepack ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prepackID;
        /// <summary>
        /// Prepack Quantity
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? prepackQuantity;
        /// <summary>
        /// # Pieces in Prepack
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? pieces;
    }
}