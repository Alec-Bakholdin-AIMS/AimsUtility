using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AimsUtility.Orders;
using Newtonsoft.Json;

namespace AimsUtilityUnitTests
{
    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        public void Order_InsertByPathLengthOne()
        {
            var order = new Order();

            // path length one
            order.InsertValueByPath("customerID", "test");
            Assert.AreEqual(order.customerID, "test");
        }

        [TestMethod]
        public void Order_InsertByPathLengthThree()
        {
            var order = new Order();
            
            // path length 3
            order.InsertValueByPath("billTo/address/city", "Marlboro");
            Assert.AreEqual(order.billTo.address.city, "Marlboro");

            // test non-string element
            order.InsertValueByPath("salesRepCommissions/salesRep1Commission/commissionPercentage", 12.0);
            Assert.AreEqual(order.salesRepCommissions.salesRep1Commission.commissionPercentage, 12.0);
        }

        [TestMethod]
        public void Order_FullStackTest()
        {
            var order = new Order();
            order.AddLineItem(new LineItem("testing"));
            order["testing"].AddSize(new Size("SM"){orderQuantity = 10});
            var str = JsonConvert.SerializeObject(order);

            Assert.AreEqual("{\"lineItems\":[{\"sizes\":[{\"sizeName\":\"SM\",\"orderQuantity\":10}],\"styleColorID\":\"testing\"}]}", str);
        }

        [TestMethod]
        public void Order_TestDateFormats()
        {
            var order = new Order();
            var dt = new DateTime(2020, 10, 1);
            order.entered = dt;
            order.complete = dt;
            order.start = dt;
            order.InsertValueByPath("salesRepCommissions/salesRep1Commission/currencyExchangeRate/exchangeRateDate", dt);
            order.InsertValueByPath("salesRepCommissions/salesRep2Commission/currencyExchangeRate/exchangeRateDate", dt);
            var str = JsonConvert.SerializeObject(order);
            
            var expected = "{\"salesRepCommissions\":{\"salesRep1Commission\":{\"currencyExchangeRate\":{\"exchangeRateDate\":\"2020-10-01\"}},\"salesRep2Commission\":{\"currencyExchangeRate\":{\"exchangeRateDate\":\"2020-10-01\"}}},\"lineItems\":[],\"entered\":\"2020-10-01\",\"start\":\"2020-10-01\",\"complete\":\"2020-10-01\"}";
            Assert.AreEqual(expected, str);
        }
    }
}