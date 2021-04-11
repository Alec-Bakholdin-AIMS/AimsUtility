using System;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AimsUtility.Api
{
    public class ApiClient
    {
        
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