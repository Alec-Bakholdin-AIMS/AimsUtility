using System.Data;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using AimsUtility.Api;


namespace AimsUtility.DataTables{
    public class MetadataObject{

        // * * * * * * * * * * Public Variables * * * * * * * * * *
        public string RowStr;
        public string[] RowArr;

        public string OutputColumn;
        public Type DataType;
        public bool Skip;

        public string InputFormula;
        public (string col, Type type)[] DependentColumns;

        public bool IsApiCall;
        public string ApiCall;
        public string ApiPath;
        public ApiClient Api;

        public string PostMappingStr;
        public Dictionary<string, string> PostMapping;


        /// <summary>
        /// Primary constructor for the metadata objects,
        /// used to help with data table connection
        /// </summary>
        /// <param name="RowStr">String of the metadata representing the data to be inserted into this object. Columns are separated by backticks (`)</param>
        /// <param name="Api">Object that contains methods for api calls that this object might need to make</param>
        public MetadataObject(string RowStr, ApiClient Api)
        {
            this.Api = Api;
            this.RowStr = RowStr;
            this.RowArr = RowStr.Split("`");

            // get raw data from metadata
            this.OutputColumn   = RowArr[0];
            this.Skip           = RowArr[1] == "TRUE";
            this.InputFormula   = RowArr[2];
            this.DataType       = ConvertStrToDataType(RowArr[3]);
            this.PostMappingStr = RowArr[4];

            // process that information into better data types
            this.DependentColumns = ParseInputFormula();

            this.PostMapping = DeserializeMappingString(this.PostMappingStr);
        }


        
        /// <summary>
        /// Only valid if the datatype is string.
        /// Deserializes the mapping string present
        /// in the metadata into a dictionary
        /// </summary>
        private Dictionary<string, string> DeserializeMappingString(string MappingStr)
        {
            var dict = new Dictionary<string, string>();

            if(this.DataType != typeof(string) && MappingStr != "")
                throw new Exception("Cannot use mapping when the type is not string");

            if(MappingStr == "")
                return dict;

            // get rows
            var rows = MappingStr.Split(";");
            foreach(string row in rows)
            {
                // split each row into key/value pairs
                var pair = row.Split(":");
                if(pair.Length != 2)
                    throw new Exception($"Invalid number of entries in mapping row of {MappingStr}");

                // insert key/value pair
                dict[pair[0]] = pair[1];                
            }

            return dict;
        }




        /// <summary>
        /// Parses the input formula and determines which input
        /// columns I will need in the future to substitute
        /// </summary> 
        private (string, Type)[] ParseInputFormula()
        {
            var regexStr = @"(?:{(API):([^\}\|]*)(?:\|([^}]*)|)}\(([^\)]+)\))|(?:{(I):([^\}\|]*)(?:\|([^}]*)|)})";
            var regex = new Regex(regexStr);


            var matches = regex.Matches(this.InputFormula);
            if(matches.Count > 1)
                throw new Exception("We're not quite ready for more than one match yet");
            if(matches.Count == 0) // no input values
            {
                this.IsApiCall = false;
                return new (string, Type)[]{(this.InputFormula, typeof(string))};
            }

            // determine what type of entityt his is and parse accordingly
            var match = matches[0];
            var type = match.Groups[1].Value;
            switch(type)
            {
                //input entity
                case "I":
                    this.IsApiCall = false;
                    var inputType = ConvertStrToDataType(match.Groups[7].Value);
                    return new (string, Type)[]{(match.Groups[6].Value, inputType)};

                // api entity
                case "API":
                    this.IsApiCall = true;
                    this.ApiCall = match.Groups[2].Value;
                    this.ApiPath = match.Groups[4].Value;

                    var subApiRegexStr = @"\[I:([^\|\]]*)(?:\|([^\]]*)|)\]";
                    var subApiRegex = new Regex(subApiRegexStr);
                    var subApiMatches = subApiRegex.Matches(this.ApiCall);

                    return subApiMatches.Select(m => (m.Groups[1].Value, ConvertStrToDataType(m.Groups[2].Value))).ToArray();

                // what?
                default:
                    throw new Exception($"Unsupported type found in {match.Groups[0].Value}");
            }
        }

        
        /// <summary>
        /// Converts the string into the corresponding data type.
        /// Empty string "" becomes string
        /// </summary>
        /// <param name="dataTypeStr">The type as a string, fetched from the metadata string in the constructor</param>
        private Type ConvertStrToDataType(string dataTypeStr)
        {
            if(dataTypeStr == "string" || dataTypeStr == "")
                return typeof(string);
            if(dataTypeStr == "int")
                return typeof(Int32);
            if(dataTypeStr == "float" || dataTypeStr == "double")
                return typeof(float);
            if(dataTypeStr == "date")
                return typeof(DateTime);

            throw new Exception($"Unsupported type {dataTypeStr}");
        }


        /// <summary>
        /// Takes the input values and plugs them into the formula, 
        /// returning the resulting value.
        /// </summary>
        /// <param name="inputFieldValues">The values from the input to substitute into the API</param>
        /// <returns>The singular value from the API specified by the input formula of this object</returns>
        public async Task<JToken> GetApiValue(string[] inputFieldValues)
        {
            if(inputFieldValues.Length != DependentColumns.Length)
                throw new Exception($"Input fields length did not match the number of dependent columns for input formula {this.InputFormula}.");

            if(!IsApiCall)
                throw new Exception($"The metadata object for output column {OutputColumn} does not involve an API call. Input formula: {this.InputFormula}");

            var url = this.ApiCall;
            // substitute the values
            for(int i = 0; i < inputFieldValues.Length; i++)
            {
                var dependentField = this.DependentColumns[i];
                var searchFor = "[I:" + dependentField.col + "]";
                url = url.Replace(searchFor, inputFieldValues[i]);
            }

            // get response from API
            var restResponse = await this.Api.GetAsync(url);

            if(!restResponse.IsSuccessful)
                throw new HttpRequestException($"Api request for {OutputColumn} was not successful. Response: {restResponse.StatusCode}, {restResponse.Content}. Call: {url}");
            
            // get data from 
            var responseJson = (JObject)JsonConvert.DeserializeObject(restResponse.Content);
            var target = responseJson.SelectToken(this.ApiPath);

            return target;
        }

        /// <summary>
        /// Uses the PostMapping dictionary to fetch the mapped
        /// value of the value defined in 'input'. Returns the 
        /// value if no mapping exists
        /// </summary>
        /// <param name="input">The value to search the PostMapping array for</param>
        /// <returns></returns>
        public object GetMappedData(object input)
        {
            // Mapping non-strings is currently not supported by this library 
            if(input.GetType() != typeof(string))
                return input;

            if(PostMapping != null && PostMapping.ContainsKey((string)input))
                return PostMapping[(string)input];

            return (string)input;
        }
    }
}