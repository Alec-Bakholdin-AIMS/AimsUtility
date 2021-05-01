using System;
using System.Reflection;
using System.Linq;
using AimsUtility.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using AimsUtility.Orders;


namespace AimsUtilityConsole
{
    class Program
    {
        private static string DemoBearer = GetFieldFromJson("DemoBearerToken", "credentials.json");
        private static string JobID = GetFieldFromJson("JobID", "credentials.json");

        static async Task Main(string[] args)
        {
            var apiClient = new ApiClient(DemoBearer);
            apiClient.LoggingFunction = Console.WriteLine;
            var styles = await apiClient.AimsODataFeed("https://apiwest.aims360.rest/StyleColors/v1.1/StyleColors");
            styles.ForEach(s => Console.WriteLine((string)s["style"]));
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
