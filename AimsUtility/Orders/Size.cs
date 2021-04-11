using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    public class Size : SizeJsonFields
    {
        [JsonIgnore]
        public LineItem ParentLineItem;

        public Size(string SizeName)
        {
            this.sizeName = SizeName;
        }
    }
}