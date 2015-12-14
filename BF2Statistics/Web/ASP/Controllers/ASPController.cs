using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// Provides base functionality to ASP related controllers
    /// </summary>
    public abstract class ASPController
    {
        /// <summary>
        /// Gets or Sets the Stats Database connection
        /// </summary>
        /// <remarks>Default is NULL. This variable must be set before using</remarks>
        protected StatsDatabase Database;

        /// <summary>
        /// Gets the HttpClient object that made this request
        /// </summary>
        protected readonly HttpClient Client;

        /// <summary>
        /// 
        /// </summary>
        protected HttpRequest Request
        {
            get { return Client.Request;  }
        }

        /// <summary>
        /// Our response object
        /// </summary>
        protected ASPResponse Response;


        public ASPController(HttpClient Client)
        {
            this.Client = Client;
            this.Response = Client.Response as ASPResponse;
        }

        /// <summary>
        /// Processes the request, and sends a resonse back to the client
        /// </summary>
        public abstract void HandleRequest();
    }
}
