using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Web
{
    public class MvcRoute
    {
        /// <summary>
        /// The characters used to split the document path into parts
        /// </summary>
        protected static readonly char[] SplitChar = new char[] { '/' };

        /// <summary>
        /// Gets or Sets the Controller
        /// </summary>
        public string Controller;

        /// <summary>
        /// Gets or Sets the Action Method
        /// </summary>
        public string Action;

        /// <summary>
        /// Gets or Sets the additional paramenters for the Action
        /// </summary>
        public string[] Params;

        /// <summary>
        /// Creates a new instance of MvcRoute
        /// </summary>
        /// <param name="DocumentPath"></param>
        public MvcRoute(string DocumentPath)
        {
            // Get the document path into an array
            string[] parts = DocumentPath.Split(SplitChar, StringSplitOptions.RemoveEmptyEntries);

            // Fetch our Controller
            Controller = (parts.Length > 0) ? parts[0] : "index";

            // Fetch our Action
            Action = (parts.Length > 1) ? parts[1] : "index";

            // Fetch our parameters if we have any
            Params = (parts.Length > 2) ? parts.Skip(2).ToArray() : new string[0];
        }
    }
}
