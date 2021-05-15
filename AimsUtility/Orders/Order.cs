using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AimsUtility.Orders
{
    /// <summary>
    /// Base Order Class
    /// </summary>
    public class Order : OrderJsonFields
    {
        /// <summary>
        /// Read-Only list of LineItem objects in this order.
        /// </summary>
        [JsonProperty(PropertyName = "lineItems")]
        public IList<LineItem> LineItems; // read-only list so users don't use unsupported methods of adding line items
        private List<LineItem> LineItemList; // read-write list of line items
        private Dictionary<string, LineItem> LineItemDict; // dictionary to search line item by styleColorID

        /// <summary>
        /// base constructor initalizes lineItems
        /// </summary>
        public Order()
        {
            LineItemList = new List<LineItem>();
            LineItems = LineItemList.AsReadOnly();
            LineItemDict = new Dictionary<string, LineItem>();
        }

        /// <summary>
        /// Adds a new line item to the order. Throws an exception
        /// if there's already a line item that has the same styleColorID
        /// or if the styleColorID is empty.
        /// </summary>
        /// <param name="LineItemObj">The LineItem object to add to the order. Its parent order will be set to this Order object.</param>
        /// <remarks>Also adds the new line item to the internal lineItemDict</remarks>
        public void AddLineItem(LineItem LineItemObj)
        {
            if(LineItemObj.ParentOrder != null && LineItemObj.ParentOrder != this)
                throw new Exception("Item does not belong to this order");
            if(LineItemObj.styleColorID == null)
                throw new Exception("styleColorID cannot be null");
            if(LineItemDict.ContainsKey(LineItemObj.styleColorID))
                throw new Exception("This order already contains a line item with the styleColorID " + LineItemObj.styleColorID);
            
            // insert into the dictionary and list
            LineItemList.Add(LineItemObj);
            LineItemDict[LineItemObj.styleColorID] = LineItemObj;
            LineItemObj.ParentOrder = this;
        }












        // * * * * * * * * * * Line Item Manipulation * * * * * * * * * *

        /// <summary>
        /// Gets the line item with the corresponding styleColorID.
        /// </summary>
        /// <param name="StyleColorID">The styleColorID to search for</param>
        /// <returns>The corresponding LineItem object. Null if none found.</returns>
        public LineItem GetLineItem(string StyleColorID)
        {
            if(LineItemDict.ContainsKey(StyleColorID))
                return LineItemDict[StyleColorID];

            return null;
        }

        /// <summary>
        /// Gets the line item at the given index.
        /// </summary>
        /// <param name="Index">The target index</param>
        /// <returns>The line item at the index</returns>
        public LineItem GetLineItem(int Index)
        {
            return LineItemList[Index];
        }

        /// <summary>
        /// Removes a LineItem object from the order
        /// </summary>
        /// <param name="LineItemObj">The LineItem to remove.</param>
        public void RemoveLineItem(LineItem LineItemObj)
        {
            if(LineItemObj.ParentOrder != this)
                throw new Exception("LineItem object does not belong to this order");
            LineItemList.Remove(LineItemObj);
            LineItemDict.Remove(LineItemObj.styleColorID);
        }

        /// <summary>
        /// Remove the Index'th line item in the LineItem list
        /// </summary>
        /// <param name="Index">The index where we'll find the line item to remove</param>
        public void RemoveLineItem(int Index)
        {
            var lineItem = LineItemList[Index];
            LineItemList.RemoveAt(Index);
            LineItemDict.Remove(lineItem.styleColorID);
        }


        /// <summary>
        /// Removes the line item with the corresponding StyleColorID from the order
        /// </summary>
        /// <param name="StyleColorID">The StyleColorID in question</param>
        public void RemoveLineItem(string StyleColorID)
        {
            var lineItem = LineItemDict[StyleColorID];
            LineItemList.Remove(lineItem);
            LineItemDict.Remove(StyleColorID);
        }

         
















        

        // * * * * * * * * * * Indexers and General Overrides * * * * * * * * * *

        /// <summary>
        /// Overrides the default ToString() method by returning the json
        /// representation of this Order object.
        /// </summary>
        /// <returns>A json string that can be POSTed to AIMS's API</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>The line item at the specified index</value>
        public LineItem this[int i]
        {
            get => LineItemList[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>The line item with the specified styleColorID</value>
        public LineItem this[string styleColorID]
        {
            get => this.GetLineItem(styleColorID);
        }
    }

}