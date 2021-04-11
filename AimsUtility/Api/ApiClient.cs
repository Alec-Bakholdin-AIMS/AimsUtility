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

namespace AimsUtility.Api
{
    /// <summary>
    /// A client to be used for calling the AIMS Api
    /// </summary>
    public class ApiClient
    {
        
        /// <summary>
        /// Bearer token to be attached to every Api call
        /// </summary>
        public string Bearer;
        private Semaphore _semaphore;
        private static Dictionary<string, IRestResponse> ApiCache = new Dictionary<string, IRestResponse>();


        /// <summary>
        /// Basic constructor that allows the user to pass a bearer token
        /// </summary>
        /// <param name="Bearer"></param>
        public ApiClient(string Bearer)
        {
            this.Bearer = Bearer;
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
            _semaphore = new Semaphore(0, MaxNumberOfThreads);
        }


        /// <summary>
        /// Reruns the aqua job and gets the job publish link after waiting for completion.
        /// </summary>
        /// <param name="BaseUrl">This should be either https://apieast.aims360.rest or https://apiwest.aims360.rest</param>
        /// <param name="JobID">The job to rerun and get the publish link for</param>
        /// <returns>The publish link. We need to return this in case it's a public link</returns>
        public async Task<string> RerunAndWaitForJobPublishLink(string BaseUrl, string JobID)
        {
            // rerun the job
            var rerunResponse = await this.PostAsync($"{BaseUrl}/jobsmanagement/v1.0/backgroundjob/{JobID}/rerun");
            if(!rerunResponse.IsSuccessful)
                throw new Exception($"Error rerunning aqua job {JobID}: {rerunResponse.Content}");

            // loop until the job is complete
            string status = null;
            string publishLink = null;
            do{
                // choose a random time between 3 and 7 seconds to sleep for to avoid overlap in parallel calls
                int seed = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeMilliseconds() % Int32.MaxValue);
                var randms = new Random(seed).Next(3000, 7000);
                Thread.Sleep(randms);

                // make the api call for the status of the job
                var statusResponse = await this.GetAsync($"{BaseUrl}/jobsmanagement/v1.0/backgroundjob/{JobID}", 3, false);
                if(!statusResponse.IsSuccessful)
                    throw new Exception($"Error fetching job status for {JobID}: {statusResponse.Content}");

                // get the status of the job
                var responseJson = (JObject)JsonConvert.DeserializeObject(statusResponse.Content);
                status = (string)responseJson["jobStatus"];
                publishLink = (string)responseJson["publishLink"];
            }while(status != "Completed");

            return publishLink;
        }


        /// <summary>
        /// Gets data from the publish link (attaches a bearer token in case it's private) and 
        /// converts the resulting CSV into a data table
        /// </summary>
        /// <param name="PublishLink">The publish link from which we will get the data</param>
        /// <returns>A data table representing the data from the publish link</returns>
        public async Task<DataTable> GetAquaCsvData(string PublishLink)
        {
            // publish links should never be cached responses
            var publishLinkResponse = await this.GetAsync(PublishLink, 3, false);
            if(!publishLinkResponse.IsSuccessful)
                throw new Exception("Retrieval of data from publish link was unsuccessful: " + PublishLink);

            // convert to CSV
            var tableFromCsv = DataTableUtillity.ParseFromCsv(publishLinkResponse.Content);
            return tableFromCsv;
        }

        
        /// <summary>
        /// Calls the API generically using a semaphore and
        /// has some basic caching. If the url matches a
        /// previous call EXACTLY, it bypasses the API call
        /// and simply returns the previous result.
        /// </summary>
        /// <param name="url">The complete url to retrieve the data from</param>
        /// <param name="MaxNumberOfIterations">The number of retries to make of the API call before returning</param>
        /// <param name="useCaching">Determines whether or not to return cached results if the url matches previous calls. This is particularly useful if you're polling the API for a change in status</param>
        public async Task<IRestResponse> GetAsync(string url, int MaxNumberOfIterations = 3, bool useCaching = true)
        {   
            // check to see if we've used this call before
            if(ApiCache.ContainsKey(url) && useCaching)
                return ApiCache[url];

            // make API call using the generic api call, which uses semaphores
            var restClient = new RestClient(url);
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", Bearer);
            var restResponse = await ExecuteAsync(restClient, restRequest, MaxNumberOfIterations);

            ApiCache[url] = restResponse;
            return restResponse;
        }

        /// <summary>
        /// Calls the API generically using a semaphore. Adds the bearer
        /// token to the call.
        /// </summary>
        /// <param name="url">The complete url to retrieve the data from</param>
        /// <param name="MaxNumberOfIterations">The number of retries to make of the API call before returning</param>
        public async Task<IRestResponse> PostAsync(string url, int MaxNumberOfIterations = 3)
        {
            var restClient = new RestClient(url);
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Authorization", Bearer);

            return await this.ExecuteAsync(restClient, restRequest, MaxNumberOfIterations);
        }

        /// <summary>
        /// The most generic api call you could ever make:
        /// you provide the client and request, this just handles retries.
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="restRequest"></param>
        /// <param name="MaxNumberOfIterations"></param>
        /// <returns></returns>
        public async Task<IRestResponse> ExecuteAsync(RestClient restClient, RestRequest restRequest, int MaxNumberOfIterations = 3)
        {
            IRestResponse restResponse = null;
            // main loop for retries
            for(int i = 0; i < MaxNumberOfIterations; i++)
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
            }

            return restResponse;
        }
    }
}