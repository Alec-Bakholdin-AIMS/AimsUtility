using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AimsUtility.Orders
{
    /// <summary>
    /// Fields that belong to the Order
    /// </summary>
    public class OrderJsonFields
    {
        // * * * * * * * * * * Primitives * * * * * * * * * *       
        /// <summary>
        /// Customer ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customerID;
        /// <summary>
        /// Division ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string divisionID;
        /// <summary>
        /// Season ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string seasonID;
        /// <summary>
        /// Term ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string termID;
        /// <summary>
        /// Shipvia ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string shipviaID;
        /// <summary>
        /// ThirdParty Order ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string thirdPartyOrderID;
        /// <summary>
        /// DropShipOrder: true/false - indicates whether to consider order as DropShip Order
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? dropShipOrder;
        /// <summary>
        /// OrderStatus Reason ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string orderStatusReasonID;
        /// <summary>
        /// Order Status Allowed values are Open/Hold
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status;
        /// <summary>
        /// Special Instruction ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string specialInstructionID;
        /// <summary>
        /// UserCode ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string userCodeID;
        /// <summary>
        /// Vendor order reference number given by your customer.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string vendor;
        /// <summary>
        /// Packing Rule Type. If nothing is specified default will be selected.
        /// Allowed values are: Default/Pack by Bulk/Pack by Prepack
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string packingRuleType;
        /// <summary>
        /// Freight Collect. Default value is false.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? freightCollect;
        /// <summary>
        /// Cash on Delivery. Default value is false.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? cod;
        /// <summary>
        /// Customer Purchase Order
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string customerPurchaseOrder;
        /// <summary>
        /// Buyer
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string buyer;
        /// <summary>
        /// Phone
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phone;
        /// <summary>
        /// Fax
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fax;
        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string email;
        /// <summary>
        /// Note 1
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string note1;
        /// <summary>
        /// Note 2
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string note2;
        /// <summary>
        /// Entered Date (yyyy-MM-dd).
        /// If nothing is specified, the current date will be considered.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? entered;
        /// <summary>
        /// Start Date (yyyy-MM-dd)
        /// If nothing is specified, the current date will be considered.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? start;
        /// <summary>
        /// Complete Date (yyyy-MM-dd)
        /// If nothing is specified, the current date will be considered.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? complete;
        /// <summary>
        /// optional. Default is 5?
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? priority;
        /// <summary>
        /// Department
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string department;
        /// <summary>
        /// Residential Address
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? residentialAddress;
        /// <summary>
        /// Ship Complete Only
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? shipCompleteOnly;
        /// <summary>
        /// Notes
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string notes;
        /// <summary>
        /// Shipping Instructions
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string shippingInstructions;
        /// <summary>
        /// Packing Notes
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string packingNotes;
        /// <summary>
        /// Value Added Service Notes
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? valueAddedServiceNotes;


        // * * * * * * * * * * Objects(Non-Primitives) * * * * * * * * * *
        /// <summary>
        /// Factor
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Factor factor;
        /// <summary>
        /// If billing address is an existing store, then specify the corresponding storeID
        /// for the billToStoreID property. If billToStoreID is specified, then the address
        /// properties will be ignored. If billing address is one-time address, then
        /// specify the address in the address properties.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BillTo billTo;
        /// <summary>
        /// If shipping store address is an existing store, then specify the corresponding
        /// storeID for the customerStoreID property. If customerStoreID is specified, then
        /// the address properties will be ignored. If shipping address is one-time address,
        /// then specify the address in the address properties.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Store store;
        /// <summary>
        /// Valid vaues for LineItemDiscountType are NET/LINEITEMDISCOUNT
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Discounts discounts;
        /// <summary>
        /// If the default salesrep commission details are configured in 
        /// the customer master then, this section can be ignored.
        /// Otherwise at least salesrep1 commission details must be specified.
        /// </summary>        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommissions salesRepCommissions;
        /// <summary>
        /// To use the existing credit card, specify the corresponding 
        /// creditcardID for the creditCardID Property. Otherwise,
        /// specify the card details in the appropriate fields.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CreditCard creditCard;
    }


    // * * * * * * * * * * address-related objects * * * * * * * * * *

    /// <summary>
    /// Bill To
    /// </summary>
    public class BillTo
    {
        /// <summary>
        /// If billing address is an existing store, then specify the corresponding storeID
        /// for the billToStoreID property. If billToStoreID is specified, then the address
        /// properties will be ignored. If billing address is one-time address, then
        /// specify the address in the address properties.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string billtoStoreID;
        /// <summary>
        /// Bill To address
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Address address;
    }

    /// <summary>
    /// Store
    /// </summary>
    public class Store
    {
        /// <summary>
        /// If shipping store address is an existing store, then specify the corresponding
        /// storeID for the customerStoreID property. If customerStoreID is specified, then
        /// the address properties will be ignored. If shipping address is one-time address,
        /// then specify the address in the address properties.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string storeStoreID;
        /// <summary>
        /// Store address
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Address address;
    }

    /// <summary>
    /// Address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name;
        /// <summary>
        /// Address 1
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1;
        /// <summary>
        /// Address 2
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address2;
        /// <summary>
        /// Address 3
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address3;
        /// <summary>
        /// City
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city;
        /// <summary>
        /// State
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state;
        /// <summary>
        /// Zip Code
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zipCode;
        /// <summary>
        /// Country ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string countryID;
    }


    // * * * * * * * * * * Discount-related objects * * * * * * * * * *
    

    /// <summary>
    /// Discounts
    /// </summary>
    public class Discounts
    {
        /// <summary>
        /// Line Item Discount
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LineItemDiscount lineItemDiscount;
        /// <summary>
        /// Discretionary Discount
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DiscretionaryDiscount discretionaryDiscount;
    }

    /// <summary>
    /// Line Item Discount
    /// </summary>
    public class LineItemDiscount
    {
        /// <summary>
        /// Disount Type. Valid values: NET/LINEITEMDISCOUNT
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string discountType;
        /// <summary>
        /// Net Discount Percentage
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? netDiscountPercentage;
    }

    /// <summary>
    /// Discretionary Discount
    /// </summary>
    public class DiscretionaryDiscount
    {
        /// <summary>
        /// Discount Percentage
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? discountPercentage;
        /// <summary>
        /// Discount Reason ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string discountReasonID;
    }


    // * * * * * * * * * * Sales rep related objects * * * * * * * * * *


    /// <summary>
    /// Sales Rep Commissions
    /// </summary>
    public class SalesRepCommissions
    {
        /// <summary>
        /// Sales Rep 1 Commissions. Only necessary if the customer
        /// has no sales rep on file.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommission salesRep1Commission;
        /// <summary>
        /// Sales Rep 2 Commissions. Only necessary if the customer
        /// has no sales rep on file.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SalesRepCommission salesRep2Commission;
    }

    /// <summary>
    /// Sales Rep Commissions
    /// </summary>
    public class SalesRepCommission
    {
        /// <summary>
        /// Sales Rep ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string salesRepID;
        /// <summary>
        /// Commission Percentage
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? commissionPercentage;
        /// <summary>
        /// Currency Exchange Rate
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CurrencyExchangeRate currencyExchangeRate;
    }

    /// <summary>
    /// Currency Exchange Rate
    /// </summary>
    public class CurrencyExchangeRate
    {
        /// <summary>
        /// Exchange Rate
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? exchangeRate;
        /// <summary>
        /// Exchange Rate Date
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(AimsDateTimeConverter))]
        public DateTime? exchangeRateDate;
    }


    // * * * * * * * * * * credit card object * * * * * * * * * *


    /// <summary>
    /// Credit Card Details
    /// </summary>
    public class CreditCard
    {
        /// <summary>
        /// Credit Card ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string creditCardID;
        /// <summary>
        /// Card Type. Acceptable values are: Mastercard, AMEX, Visa (?)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardType;
        /// <summary>
        /// Card Number. Must match the cardtype above
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardNumber;
        /// <summary>
        /// Name on card
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cardName;
        /// <summary>
        /// Billing Address
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string billingAddress;
        /// <summary>
        /// Zip Code
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zipcode;
        /// <summary>
        /// Expiration Month
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expirationMonth;
        /// <summary>
        /// Expiration Year
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expirationYear;
    }


    // * * * * * * * * * * factor object * * * * * * * * * *


    /// <summary>
    /// Factor
    /// </summary>
    public class Factor
    {
        /// <summary>
        /// Factor ID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string factorID;
        /// <summary>
        /// Credit Decision
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CreditDecision creditDecision;
    }

    /// <summary>
    /// Credit Decision
    /// </summary>
    public class CreditDecision
    {
        /// <summary>
        /// Decision Date
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string decisionDate;
        /// <summary>
        /// Amount
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float amount;
        /// <summary>
        /// Code Comment
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string codeComment;
    }

    // * * * * * * * * * * Field converters * * * * * * * * * *

    /// <summary>
    /// Used in Json Tags to convert time to proper AIMS date format (yyyy-MM-dd)
    /// </summary>
    public class AimsDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Base constructor sets DateTimeFormat to yyyy-MM-dd
        /// </summary>
        public AimsDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

}