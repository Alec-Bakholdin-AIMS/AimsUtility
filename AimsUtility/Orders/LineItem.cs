using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    /// <summary>
    /// A class representing a single entry in the lineItems JArray in the AIMS order json
    /// </summary>
    public class LineItem : LineItemJsonFields
    {
        /// <summary>
        /// Variable that keeps track of the parent order.
        /// </summary>
        [JsonIgnore]
        public Order ParentOrder;

        /// <summary>
        /// A read-only list containing all the Size objects that belong to this line item
        /// </summary>
        [JsonProperty(PropertyName = "sizes")]
        public IList<Size> Sizes;
        private List<Size> SizeList;
        private Dictionary<string, Size> SizeDict;

        /// <summary>
        /// Base constructor that only initializes the sizes list.
        /// </summary>
        public LineItem()
        {
            SizeList = new List<Size>();
            Sizes = SizeList.AsReadOnly();
            SizeDict = new Dictionary<string, Size>();
        }

        /// <summary>
        /// Constructor that initializes the sizes list and also sets the styleColorID of this object.
        /// </summary>
        /// <param name="StyleColorID">The styleColorID for this line item</param>
        public LineItem(string StyleColorID)
        {
            this.styleColorID = StyleColorID;
            SizeList = new List<Size>();
            Sizes = SizeList.AsReadOnly();
            SizeDict = new Dictionary<string, Size>();
        }

        /// <summary>
        /// Adds a new line item to the order. Throws an exception
        /// if there's already a line item that has the same sizeName
        /// or if the sizeName is empty.
        /// </summary>
        /// <param name="SizeObj">The LineItem object to add to the order. Its parent order will be set to this Order object.</param>
        /// <remarks>Also adds the new line item to the internal SizeDict</remarks>
        public void AddSize(Size SizeObj)
        {
            if(SizeObj.ParentLineItem != null && SizeObj.ParentLineItem != this)
                throw new Exception("Item does not belong to this order");
            if(SizeObj.sizeName == null)
                throw new Exception("sizeName cannot be null");
            if(SizeDict.ContainsKey(SizeObj.sizeName))
                throw new Exception("This order already contains a line item with the sizeName " + SizeObj.sizeName);
            
            // insert into the dictionary and list
            SizeList.Add(SizeObj);
            SizeDict[SizeObj.sizeName] = SizeObj;
            SizeObj.ParentLineItem = this;
        }

        /// <summary>
        /// Gets the size with the corresponding sizeName.
        /// </summary>
        /// <param name="sizeName">The sizeName to search for</param>
        /// <returns>The corresponding Size object. Null if none found.</returns>
        public Size GetSize(string sizeName)
        {
            if(SizeDict.ContainsKey(sizeName))
                return SizeDict[sizeName];

            return null;
        }

        /// <summary>
        /// Gets the size at the given index.
        /// </summary>
        /// <param name="Index">The target index</param>
        /// <returns>The size at the index</returns>
        public Size GetSize(int Index)
        {
            return SizeList[Index];
        }

        /// <summary>
        /// Removes a Size object from the order
        /// </summary>
        /// <param name="SizeObj">The Size to remove.</param>
        public void RemoveSize(Size SizeObj)
        {
            if(SizeObj.ParentLineItem != this)
                throw new Exception("LineItem object does not belong to this order");
            SizeList.Remove(SizeObj);
            SizeDict.Remove(SizeObj.sizeName);
        }

        /// <summary>
        /// Remove the Index'th size in the LineItem list
        /// </summary>
        /// <param name="Index">The index where we'll find the size to remove</param>
        public void RemoveSize(int Index)
        {
            var size = SizeList[Index];
            SizeList.RemoveAt(Index);
            SizeDict.Remove(size.sizeName);
        }


        /// <summary>
        /// Removes the line item with the corresponding sizeName from the order
        /// </summary>
        /// <param name="sizeName">The sizeName in question</param>
        public void RemoveSize(string sizeName)
        {
            var lineItem = SizeDict[sizeName];
            SizeList.Remove(lineItem);
            SizeDict.Remove(sizeName);
        }


        // * * * * * * * * * * Indexers * * * * * * * * * *
        /// <summary>
        /// Reference a size by its position
        /// </summary>
        /// <value>A size object at the position i in the Sizes list</value>
        public Size this[int i]
        {
            get => SizeList[i];
        }

        /// <summary>
        /// Reference a size by name
        /// </summary>
        /// <value>A Size object with the sizeName 'sizeName'</value>
        public Size this[string sizeName]
        {
            get => this.GetSize(sizeName);
        }
    }
}