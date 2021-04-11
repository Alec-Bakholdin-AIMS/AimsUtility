using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataTableUtil.Orders
{
    // order/root of everything here
    public class OrderJsonFields
    {
        // * * * * * * * * * * Primitives * * * * * * * * * *        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customerID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string divisionID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string seasonID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string termID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string shipviaID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string thirdPartyOrderID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? dropShipOrder;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string orderStatusReasonID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string specialInstructionID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string userCodeID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string vendor;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string packingRuleType;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? freightCollect;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? cod;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customerPurchaseOrder;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string buyer;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phone;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fax;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string email;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string note1;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string note2;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? entered;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? start;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? complete;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? priority;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string department;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? residentialAddress;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? shipCompleteOnly;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string notes;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string shippingInstructions;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string packingNotes;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? valueAddedServiceNotes;


        // * * * * * * * * * * Objects(Non-Primitives) * * * * * * * * * *
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BillTo billTo;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Store store;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Discounts discounts;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommissions salesRepCommissions;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CreditCard creditCard;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Factor factor;
    }


    // * * * * * * * * * * address-related objects * * * * * * * * * *


    public class BillTo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string billtoStoreID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Address address;
    }

    public class Store
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string storeStoreID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Address address;
    }

    public class Address
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address2;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address3;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zipCode;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryID;
    }


    // * * * * * * * * * * Discount-related objects * * * * * * * * * *
    

    public class Discounts
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LineItemDiscount lineItemDiscount;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DiscretionaryDiscount discretionaryDiscount;
    }

    public class LineItemDiscount
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string discountType;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? netDiscountPercentage;
    }

    public class DiscretionaryDiscount
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? discountPercentage;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string discountReasonID;
    }


    // * * * * * * * * * * Sales rep related objects * * * * * * * * * *


    public class SalesRepCommissions
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommission salesRep1Commission;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommission salesRep2Commission;
    }

    public class SalesRepCommission
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string salesRepID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? commissionPercentage;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CurrencyExchangeRate currencyExchangeRate;
    }

    public class CurrencyExchangeRate
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? exchangeRate;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? exchangeRateDate;
    }


    // * * * * * * * * * * credit card object * * * * * * * * * *


    public class CreditCard
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string creditCardID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardType;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardNumber;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardName;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string billingAddress;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zipcode;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expirationMonth;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expirationYear;
    }


    // * * * * * * * * * * factor object * * * * * * * * * *


    public class Factor
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string factorID;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CreditDecision creditDecision;
    }

    public class CreditDecision
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string decisionDate;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float amount;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string codeComment;
    }

    // * * * * * * * * * * Field converters * * * * * * * * * *
    public class AimsDateTimeConverter : IsoDateTimeConverter
    {
        public AimsDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

}