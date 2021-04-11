using AimsUtility.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using RestSharp;
using System.Threading.Tasks;

namespace AimsUtilityUnitTests
{
    [TestClass]
    public class ApiTests
    {
        private static string DemoBearer = GetFieldFromJson("DemoBearerToken", "credentials.json");
        
        [TestMethod]
        public async Task GenericApiCall()
        {
            var restClient = new RestClient("https://apieast.aims360.rest/codes/v1.0/currencies?$filter=currencyCode eq 'AUD'");
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", DemoBearer);

            var apiClient = new ApiClient(DemoBearer);
            var response = await apiClient.ExecuteAsync(restClient, restRequest);

            var expected = "{\"@odata.context\":\"https://apieast1.aims360.rest/v1.0/$metadata#Currencies\",\"value\":[{\"currencyID\":\"3bb08470-6a63-ea11-a94c-000d3a13013b\",\"currencyCode\":\"AUD\",\"currencyName\":\"Australian dollar\",\"currencySymbol\":\"$\",\"currencyEnabled\":false}]}";
            Assert.AreEqual(response.Content, expected);
        }

        /// <summary>
        /// Gets the field value from a json that is found
        /// at a local filepath, relative of course
        /// </summary>
        /// <param name="fieldName">The field of the json to fetch (currently only works at one level deep, cannot do nested objects)</param>
        /// <param name="filepath">The filepath to the json field</param>
        /// <returns></returns>
        private static string GetFieldFromJson(string fieldName, string filepath)
        {
            var contents = File.ReadAllText(filepath);
            var json = (JObject)JsonConvert.DeserializeObject(contents);
            return (string)json[fieldName];
        }
    }
}