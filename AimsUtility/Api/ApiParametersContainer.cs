

namespace AimsUtility.Api
{
    /// <summary>
    /// An object that's passed to methods in the ApiClient
    /// class to change certain behaviors.
    /// </summary>
    public class ApiParametersContainer
    {
        /// <summary>
        /// Determines whether to use caching or not.
        /// Default value is true.
        /// </summary>
        public bool UseCaching = true;

        /// <summary>
        /// Determines how many iterations to try before giving up.
        /// Setting MaxNumIterations to null will mean infinite tries
        /// until a success is met (this is not recommended).
        /// Default value is 3
        /// </summary>
        public int? MaxNumIterations = 3;

        /// <summary>
        /// used in certain calls to log what's going on in parallel calls.
        /// Default value is null.
        /// </summary>
        public string JobName = null;

        /// <summary>
        /// Default constructor sets the values of the 
        /// parameters to their listed default values
        /// </summary>
        public ApiParametersContainer()
        {

        }
    }
}