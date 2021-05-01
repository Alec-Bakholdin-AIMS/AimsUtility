namespace AimsUtility.Api
{
    /// <summary>
    /// Essentially a container containing parameters for paged Rest API
    /// responses. This is relevant for AIMS360's OData Feed as well as
    /// LogicBroker's paged orders page, so far.
    /// </summary>
    public class ApiFeedParameters
    {
        /// <summary>
        /// The path with the base url and filters needed for the feed.
        /// With regards to OData, this would contain things like $filter
        /// and $select, but not $count or $skip
        /// </summary>
        public string BasePath;


        /// <summary>
        /// This should be set ahead of time, and is required to calculate
        /// the number of pages so the feed can be efficiently consumed.
        /// </summary>
        public int PageSize;


        /// <summary>
        /// Generally, feeds will 
        /// </summary>
        public string CountLocation;
    }
}