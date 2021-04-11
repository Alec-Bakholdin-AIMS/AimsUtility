using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace DataTableUtil.DataTables{
    public class DataTableConnectorApi{
        // * * * * * * * * * * Public variables * * * * * * * * * *
        public string BearerToken;



        // * * * * * * * * * * Private variables * * * * * * * * * *
        private int MaxNumThreads;
        private Semaphore _semaphore;
        private static Dictionary<string, IRestResponse> ApiCache = new Dictionary<string, IRestResponse>();


        /**
         * <summary>
         * Initialize the API for the data table connector. Specify the
         * bearer token to use as well as the maximum number of threads 
         * that are allowed to use this API client at once (semaphore behavior).
         * </summary>
         * <param name="BearerToken">The bearer token to use in authorization</param>
         * <param name="MaxNumThreads">Maximum number of threads that can be waiting for the API to complete. Default is 40</param>
         */
        public DataTableConnectorApi(string BearerToken, int MaxNumThreads = 50)
        {
            this.BearerToken = BearerToken;
            this.MaxNumThreads = MaxNumThreads;
            this._semaphore =  new Semaphore(MaxNumThreads, MaxNumThreads);
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
        public async Task<IRestResponse> GetFromApi(string url, int MaxNumberOfIterations = 3, bool useCaching = true)
        {   
            // check to see if we've used this call before
            if(ApiCache.ContainsKey(url) && useCaching)
                return ApiCache[url];

            // make API call using the generic api call, which uses semaphores
            var restClient = new RestClient(url);
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Authorization", BearerToken);
            var restResponse = await CallApiGeneric(restClient, restRequest, MaxNumberOfIterations);

            ApiCache[url] = restResponse;
            return restResponse;
        }



        /// <summary>
        /// The most generic api call you could ever make
        /// you provide the client and request, this just handles
        /// retries.
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="restRequest"></param>
        /// <param name="MaxNumberOfIterations"></param>
        /// <returns></returns>
        public async Task<IRestResponse> CallApiGeneric(RestClient restClient, RestRequest restRequest, int MaxNumberOfIterations = 3)
        {
            IRestResponse restResponse = null;
            // main loop for retries
            for(int i = 0; i < MaxNumberOfIterations; i++)
            {
                _semaphore.WaitOne(); // allow up to 5 threads to call aims at a time, so as to not overload the servers
                restResponse = await restClient.ExecuteAsync(restRequest);
                _semaphore.Release();

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