using Newtonsoft.Json;

namespace DataTableUtil.Orders
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