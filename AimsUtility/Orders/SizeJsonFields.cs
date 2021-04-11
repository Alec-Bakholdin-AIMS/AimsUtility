using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    /// <summary>
    /// The fields that are needed for the size object,
    /// stored in lineItems, that holds each size information
    /// </summary>
    public class SizeJsonFields
    {
        /// <summary>
        /// Size Name (e.g. 'SM')
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sizeName;
        /// <summary>
        /// Order Quantity. Note: If prepackID and prepackQuantity is specified, then orderQuantity is not required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? orderQuantity;
    }
}