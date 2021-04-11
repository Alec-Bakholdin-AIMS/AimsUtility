using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    /// <summary>
    /// Object used to store information about sizes in each line item
    /// </summary>
    public class Size : SizeJsonFields
    {
        /// <summary>
        /// The line item this size belongs to
        /// </summary>
        [JsonIgnore]
        public LineItem ParentLineItem;

        /// <summary>
        /// Basic constructor that allows specification of the Size Name
        /// </summary>
        /// <param name="SizeName">The size name to set</param>
        public Size(string SizeName)
        {
            this.sizeName = SizeName;
        }
    }
}