using System;
using System.IO;
using System.Data;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AimsUtility.DataTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

// TODO: OData feed and conversion to data table (possibly generic using strings/object arrays in some way)

namespace AimsUtility.Api
{
    /// <summary>
    /// A client to be used for calling the AIMS Api
    /// </summary>
    public class ApiClient
    {
        
        /// <summary>
        /// Bearer token to be attached to every Api call.
        /// </summary>
        public string Bearer;
        private Semaphore _semaphore;
        private Dictionary<string, IRestResponse> ApiCache;

        /// <summary>
        /// A void function that takes a string which the API can use to print logging information to.
        /// </summary>
        public Action<string> LoggingFunction;



        /// <summary>
        /// Basic constructor that allows the user to pass a bearer token. Sets
        /// the maximum number of concurrend threads to 50, the default
        /// </summary>
        /// <param name="Bearer"></param>
        public ApiClient(string Bearer)
        {
            this.Bearer = Bearer;
            _semaphore = new Semaphore(50, 50);
            ApiCache = new Dictionary<string, IRestResponse>();
        }


        /// <summary>
        /// Allows the option to specify how many threads can make an API call
        /// at once. If not included, there is no limit
        /// </summary>
        /// <param name="Bearer"></param>
        /// <param name="MaxNumberOfThreads"></param>
        public ApiClient(string Bearer, int MaxNumberOfThreads)
        {
            this.Bearer = Bearer;
            _semaphore = new Semaphore(MaxNumberOfThreads, MaxNumberOfThreads);
            ApiCache = new Dictionary<string, IRestResponse>();
        }


        /// <summary>
        /// Reruns the aqua job and gets the job publish link after waiting for completion.
        /// </summary>
        /// <param name="BaseUrl">This should be either https://apieast.aims360.rest or https://apiwest.aims360.rest</param>
        /// <param name="JobID">The job to rerun and get the publish link for</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns>The publish link. We need to return this in case it's a public link</returns>
        public async Task<string> RerunAndWaitForJobPublishLink(string BaseUrl, string JobID, ApiParametersContainer ApiParameters = null)
        {
            await RerunJob(BaseUrl, JobID, ApiParameters);
            var publishLink = await WaitForPublishLink(BaseUrl, JobID, ApiParameters);

            return publishLink;
        }

        /// <summary>
        /// Reruns the job assocated with the jobID parameter.
        /// </summary>
        /// <param name="BaseUrl">This should be either https://apieast.aims360.rest or https://apiwest.aims360.rest</param>
        /// <param name="JobID">The job to rerun and get the publish link for</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        public async Task RerunJob(string BaseUrl, string JobID, ApiParametersContainer ApiParameters = null)
        {
            var JobNameString = ApiParameters?.JobName != null ? ApiParameters?.JobName + ": " : "";

            // rerun the job
            LoggingFunction?.Invoke(JobNameString + "Rerunning job");
            var rerunResponse = await this.PostAsync($"{BaseUrl}/jobsmanagement/v1.0/backgroundjob/{JobID}/rerun");
            if(!rerunResponse.IsSuccessful)
                throw new Exception($"Error rerunning aqua job {JobID}: {rerunResponse.Content}");
            LoggingFunction?.Invoke(JobNameString + "Successfully reran job");
        }

        /// <summary>
        /// Waits for a job to finish, returns the publish link when it does.
        /// </summary>
        /// <param name="BaseUrl">This should be either https://apieast.aims360.rest or https://apiwest.aims360.rest</param>
        /// <param name="JobID">The job to rerun and get the publish link for</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns>The publish link. We need to return this in case it's a public link</returns>
        public async Task<string> WaitForPublishLink(string BaseUrl, string JobID, ApiParametersContainer ApiParameters = null)
        {
            var JobNameString = ApiParameters?.JobName != null ? ApiParameters?.JobName + ": " : "";

            // loop until the job is complete
            string status = null;
            string publishLink = null;
            int i = 1;
            do{
                // choose a random time between 3 and 7 seconds to sleep for to avoid overlap in parallel calls
                int seed = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeMilliseconds() % Int32.MaxValue);
                var randms = new Random(seed).Next(3000, 7000);
                Thread.Sleep(randms);


                // make the api call for the status of the job
                var statusResponse = await this.GetAsync($"{BaseUrl}/jobsmanagement/v1.0/backgroundjob/{JobID}", ApiParameters);
                if(!statusResponse.IsSuccessful)
                    throw new Exception($"Error fetching job status for {JobID}: {statusResponse.Content}");

                // tell the user which iteration we're on
                String iterationString = ApiParameters?.MaxNumIterations == null ? $"{i}" : $"{i}/{(ApiParameters?.MaxNumIterations ?? -1)}";

                // get the status of the job
                var responseJson = (JObject)JsonConvert.DeserializeObject(statusResponse.Content);
                status = (string)responseJson["jobStatus"];
                if(status != "Completed" && status != "Failed")
                    status = (string)responseJson["jobStatusText"];
                publishLink = (string)responseJson["publishLink"];

                LoggingFunction?.Invoke(JobNameString + $"Iteration {iterationString}. Status: {status}.");

                i++;
            // finish when the job is complete or if we've tried enough times (infinite if MaxNumIterations=null)
            }while((status != "Completed" && status != "Failed") || (ApiParameters?.MaxNumIterations != null && i > (ApiParameters?.MaxNumIterations ?? -1)));

            // edge case when we can't wait this long
            if(status != "Completed")
                throw new Exception($"{JobNameString}Job {JobID} was not successfully completed");

            return publishLink;
        }


        /// <summary>
        /// Gets data from the publish link (attaches a bearer token in case it's private) and 
        /// converts the resulting CSV into a data table. If no parameters are included, 
        /// </summary>
        /// <param name="PublishLink">The publish link from which we will get the data</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns>A data table representing the data from the publish link</returns>
        public async Task<DataTable> GetAquaCsvData(string PublishLink, ApiParametersContainer ApiParameters = null)
        {
            LoggingFunction?.Invoke($"Fetching CSV data from {PublishLink}");

            // publish links should never be cached responses unless specified
            var publishLinkResponse = await this.GetAsync(PublishLink, (ApiParameters ?? new ApiParametersContainer(){UseCaching = false}));
            if(!publishLinkResponse.IsSuccessful)
                throw new Exception("Retrieval of data from publish link was unsuccessful: " + PublishLink);

            // convert to CSV
            var jobName = ApiParameters?.JobName == null ? "" : $"{ApiParameters?.JobName}: ";
            LoggingFunction?.Invoke($"{jobName}Converting CSV data to DataTable");
            var tableFromCsv = DataTableUtility.ParseFromCsv(publishLinkResponse.Content);
            LoggingFunction?.Invoke($"{jobName}Successfully converted CSV data to DataTable");
            return tableFromCsv;
        }

                
        /// <summary>
        /// Gets the data from the url in an OData feed and also loops through each
        /// other page that OData returns in a parallel fashion.
        /// </summary>
        /// <param name="Url">The url of the first page. Should not contain pagesize, $count or $skip</param>
        /// <param name="pagesize">The number of entries per page. This should be 250 by default, but it might change based on other specifications</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns>A list of JObjects representing the feed.</returns>
        public async Task<List<JObject>> AimsODataFeed(string Url, ApiParametersContainer ApiParameters = null, int pagesize=250)
        {
            // get first page
            LoggingFunction?.Invoke("Retrieving first page of feed");
            var firstPageUrl = Url + (Url.Contains("?") ? "&" : "?") + $"$count=true&pagesize={pagesize}";
            var firstPageResponse = await this.GetAsync(firstPageUrl, ApiParameters);
            if(!firstPageResponse.IsSuccessful) throw new Exception($"Error retrieving response from {firstPageUrl}: {firstPageResponse.Content}");

            // get count and data from first page
            var feedData = new List<JObject>();
            var firstPageJson = (JObject)JsonConvert.DeserializeObject(firstPageResponse.Content);
            var count = (int)firstPageJson["@odata.count"];
            var numPages = count/pagesize + 1;
            LoggingFunction?.Invoke($"Successfully retrieved first page. Number of entries: {count}. Number of pages: {numPages}");
            ((JArray)firstPageJson["value"]).ToList().ForEach(j => feedData.Add((JObject)j));

            // get the pages in an asynchronous manner
            var taskList = new List<Task<IRestResponse>>();
            for(int i = 1; i < numPages; i++)
            {
                var ithPageUrl = Url + (Url.Contains("?") ? "&" : "?") + $"pagesize={pagesize}&$skip={i*pagesize}";
                taskList.Add(GetAsync(ithPageUrl, ApiParameters));
                LoggingFunction?.Invoke($"Initialized fetch of page {i + 1}/{numPages}");
            }

            // await the page tasks
            for(int i = 1; i < numPages; i++)
            {
                // await the response and remove it from the list
                var ithPageResponseIndex = Task.WaitAny(taskList.ToArray());
                var ithPageResponseTask = taskList[ithPageResponseIndex];
                taskList.Remove(ithPageResponseTask);
                var ithPageResponse = await ithPageResponseTask;
                if(!ithPageResponse.IsSuccessful) throw new Exception($"Error retrieving page from API: {ithPageResponse.Content}");
                
                // if successful, we can add the data to the feedData list
                var ithPageResponseJson = (JObject)JsonConvert.DeserializeObject(ithPageResponse.Content);
                ((JArray)ithPageResponseJson["value"]).ToList().ForEach(j => feedData.Add((JObject)j));
                LoggingFunction?.Invoke($"Successfully retrieved page {i + 1}/{numPages}");

            }

            return feedData;
        }


        /// <summary>
        /// Uses Newtonsoft.Json's SelectToken function to find a token in path.
        /// This token path will be absolute, so make sure you construct your url
        /// in such a way that it unambiguously returns 1 output. Remember that this
        /// uses caching.
        /// </summary>
        /// <param name="Url">The exact url to call from the API</param>
        /// <param name="JTokenPath">The exact static path to find the JToken at</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns>The JToken at the path, or null if not present</returns>
        public async Task<JToken> GetTokenAsync(String Url, String JTokenPath, ApiParametersContainer ApiParameters = null)
        {
            // get the response from the API
            var response = await this.GetAsync(Url, ApiParameters);
            if(!response.IsSuccessful)
                throw new Exception($"Could not retrieve data from {Url}: {response.Content}");

            // parse the json and return the token
            var responseJson = (JObject)JsonConvert.DeserializeObject(response.Content);
            var token = responseJson.SelectToken(JTokenPath);

            return token;
        }

        /// <summary>
        /// Calls the API generically using a semaphore and
        /// has some basic caching. If the url matches a
        /// previous call EXACTLY, it bypasses the API call
        /// and simply returns the previous result.
        /// </summary>
        /// <param name="url">The complete url to retrieve the data from</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        public async Task<IRestResponse> GetAsync(string url, ApiParametersContainer ApiParameters = null)
        {   
            // check to see if we've used this call before
            if((ApiParameters?.UseCaching ?? false) && ApiCache.ContainsKey(url))
                return ApiCache[url];

            // make API call using the generic api call, which uses semaphores
            var restClient = new RestClient(url);
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", Bearer);
            var restResponse = await ExecuteAsync(restClient, restRequest, ApiParameters);

            lock(ApiCache){
                ApiCache[url] = restResponse;
            }
            return restResponse;
        }

        /// <summary>
        /// Calls the API generically using a semaphore. Adds the bearer
        /// token to the call.
        /// </summary>
        /// <param name="Url">The complete url to retrieve the data from</param>
        /// <param name="JsonPayload">The body (json format) to send with the POST request</param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        public async Task<IRestResponse> PostAsync(string Url, string JsonPayload = null, ApiParametersContainer ApiParameters = null)
        {
            var restClient = new RestClient(Url);
            var restRequest = new RestRequest(Method.POST);
            if(JsonPayload != null)
                restRequest.AddJsonBody(JsonPayload);
            restRequest.AddHeader("Authorization", Bearer);

            return await this.ExecuteAsync(restClient, restRequest, ApiParameters);
        }

        /// <summary>
        /// The most generic api call you could ever make:
        /// you provide the client and request, this just handles retries.
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="restRequest"></param>
        /// <param name="ApiParameters">Changes the behavior of the Api calls</param>
        /// <returns></returns>
        public async Task<IRestResponse> ExecuteAsync(RestClient restClient, RestRequest restRequest, ApiParametersContainer ApiParameters = null)
        {
            IRestResponse restResponse = null;
            // main loop for retries
            for(int i = 0; i < (ApiParameters?.MaxNumIterations ?? 3); i++)
            {
                _semaphore?.WaitOne(); // allow up to 5 threads to call aims at a time, so as to not overload the servers
                restResponse = await restClient.ExecuteAsync(restRequest);
                _semaphore?.Release();

                if(restResponse.IsSuccessful)
                    return restResponse;

                // exponential backoff
                else
                {
                    var timeInSeconds = (int)Math.Pow(2, i);
                    Thread.Sleep(timeInSeconds*1000);
                }

                // make this an infinite infinite loop if maxnumiterations is set to null
                if(ApiParameters != null && ApiParameters.MaxNumIterations == null)
                    i--;
            }

            return restResponse;
        }
    }
}