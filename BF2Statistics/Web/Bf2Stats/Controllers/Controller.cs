using System;
using System.IO;
using System.Text.RegularExpressions;
using BF2Statistics.Database;
using RazorEngine;
using RazorEngine.Templating;

namespace BF2Statistics.Web.Bf2Stats
{
    public abstract class Controller
    {
        /// <summary>
        /// Gets the HttpClient object that made this request
        /// </summary>
        protected readonly HttpClient Client;

        /// <summary>
        /// Our cache file object if we need it. This will be NULL until
        /// </summary>
        protected FileInfo CacheFile;

        public Controller(HttpClient Client)
        {
            this.Client = Client;
        }

        /// <summary>
        /// Processes the request, and sends a resonse back to the client
        /// </summary>
        public abstract void HandleRequest(MvcRoute Route);

        /// <summary>
        /// Returns whether the template name provided has been compiled
        /// with the Razor Engine
        /// </summary>
        /// <param name="Name">The name of the template</param>
        /// <returns></returns>
        protected bool EnsureTemplate(string Name)
        {
            return Engine.Razor.IsTemplateCached(Name + ".cshtml", HttpServer.ModelType);
        }

        /// <summary>
        /// Returns whether or not the Cache file is expired
        /// </summary>
        /// <param name="CacheFileName">The Cache file name</param>
        /// <param name="CacheTime">The expire time of the cache file in Minutes</param>
        /// <returns></returns>
        protected bool CacheFileExpired(string CacheFileName, int CacheTime)
        {
            // If cache is diabled, force the Controller to process
            if (!Program.Config.BF2S_CacheEnabled) return true;

            // Make sure we have loaded this file
            if (CacheFile == null)
                CacheFile = new FileInfo(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache", CacheFileName + ".html"));

            // Cant be expired if we dont exist, so we force the Controller to process
            if (!CacheFile.Exists) return true;

            // Compare dates
            return (DateTime.Now.CompareTo(CacheFile.LastWriteTime.AddMinutes(CacheTime)) > 0);
        }

        /// <summary>
        /// Runs the specified Template through the RazorEngine and sends the contents back to the
        /// client in the Response
        /// </summary>
        /// <param name="TemplateName">The name of the cshtml template file we are parsing</param>
        /// <param name="ModelType">The Model type used in the template file</param>
        /// <param name="Model">The Model used in the template file</param>
        /// <param name="CacheFileName">The name of the Cached file if we are using one. Leave blank for no caching.</param>
        protected void SendTemplateResponse(string TemplateName, Type ModelType, object Model, string CacheFileName = "")
        {
            // Send Response
            HttpResponse Response = Client.Response;
            Response.ContentType = "text/html";
            Response.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Response.AddHeader("Expires", "0");
            Response.ResponseBody.Append(Engine.Razor.Run(TemplateName + ".cshtml", ModelType, Model));
            Response.Send();

            // Process cache
            if (Program.Config.BF2S_CacheEnabled && !String.IsNullOrWhiteSpace(CacheFileName))
            {
                try
                {
                    // Make sure we have loaded this file
                    if (CacheFile == null)
                        CacheFile = new FileInfo(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache", CacheFileName + ".html"));

                    using (FileStream Stream = CacheFile.Open(FileMode.OpenOrCreate, FileAccess.Write))
                    using (StreamWriter Writer = new StreamWriter(Stream))
                    {
                        Writer.BaseStream.SetLength(0);
                        Writer.Write(Response.ResponseBody.ToString());
                        Writer.Flush();
                    }

                    // Manually set write time!!!
                    CacheFile.LastWriteTime = DateTime.Now;
                }
                catch (Exception e)
                {
                    Program.ErrorLog.Write("WARNING: [Controller.CreateCacheFile] " + e.Message);
                }
            }
        }

        /// <summary>
        /// Fills the client response with the contents of the Cache file specified
        /// </summary>
        /// <param name="CacheFileName">The name of the cache file</param>
        public void SendCachedResponse(string CacheFileName)
        {
            try
            {
                // Make sure we have loaded this file
                if (CacheFile == null)
                    CacheFile = new FileInfo(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache", CacheFileName + ".html"));

                // Open the file
                using (FileStream Stream = CacheFile.Open(FileMode.Open, FileAccess.Read))
                using (StreamReader Reader = new StreamReader(Stream))
                {
                    // Read our page source
                    string source = Reader.ReadToEnd();

                    // Replace Http Link Addresses to match the request URL, preventing Localhost links with external requests
                    BF2PageModel Model = new IndexModel(Client);
                    Client.Response.ResponseBody.Append(
                        Regex.Replace(source, "http://.*?/Bf2stats", Model.Root, RegexOptions.IgnoreCase)
                    );
                }
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("ERROR: [Controller.SendCachedResponse] " + e.Message);
                Client.Response.ResponseBody.Append("Cache Read Error: " + e.GetType().Name);
            }

            // Send Response
            Client.Response.ContentType = "text/html";
            Client.Response.Send();
        }

        /// <summary>
        /// Formats an integer timestamp to a timespan format that was used in BF2sClone
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public string FormatTime(int Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Time);
            return String.Format("{0:00}:{1:00}:{2:00}", Span.TotalHours, Span.Minutes, Span.Seconds);
        }

        /// <summary>
        /// Takes a timestamp and converts it to a data format that was used in BF2sClone
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public string FormatDate(int Time)
        {
            DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (T.AddSeconds(Time)).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
