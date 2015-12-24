using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.ASP;
using BF2Statistics.ASP.StatsProcessor;
using BF2Statistics.Database;
using BF2Statistics.Logging;
using BF2Statistics.Web.ASP;
using BF2Statistics.Web.Bf2Stats;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace BF2Statistics.Web
{
    /// <summary>
    /// The ASP Server is used to emulate the official Gamespy BF2 Stats
    /// Server HTTP Requests, and provide players with the ability to run 
    /// thier own BF2 Ranking system on thier personal PC's.
    /// </summary>
    public static class HttpServer
    {
        /// <summary>
        /// The HTTPListner for the webserver
        /// </summary>
        private static HttpListener Listener;

        /// <summary>
        /// Limits the simultaneous connections to prevent an app overload
        /// </summary>
        private static SemaphoreSlim ConnectionPool;

        /// <summary>
        /// The StatsDebug.log file
        /// </summary>
        public static LogWriter AspStatsLog { get; private set; }

        /// <summary>
        /// THe Http Server Access log
        /// </summary>
        public static LogWriter HttpAccessLog { get; private set; }

        /// <summary>
        /// A List of local IP addresses for this machine
        /// </summary>
        public static readonly List<IPAddress> LocalIPs;

        /// <summary>
        /// An array of http request methods that this server will accept
        /// </summary>
        public static readonly string[] AcceptableMethods = { "GET", "POST", "HEAD" };

        /// <summary>
        /// Number of session web requests
        /// </summary>
        public static int SessionRequests = 0;

        /// <summary>
        /// Is the webserver running?
        /// </summary>
        public static bool IsRunning
        {
            get 
            {
                try 
                {
                    return Listener.IsListening;
                }
                catch (ObjectDisposedException) 
                {
                    CreateHttpListener();
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the default ModelType for bf2stats pages
        /// </summary>
        public static readonly Type ModelType;

        /// <summary>
        /// Contains a list of Mime types for the response
        /// </summary>
        private static readonly Dictionary<string, string> MIMETypes = new Dictionary<string, string>
        {
            {"css", "text/css"},
            {"gif", "image/gif"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"ico", "image/x-icon"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"js", "application/x-javascript"},
            {"png", "image/png"}
        };

        /// <summary>
        /// Occurs when the server is started.
        /// </summary>
        public static event EventHandler Started;

        /// <summary>
        /// Occurs when the server is about to stop.
        /// </summary>
        public static event EventHandler Stopping;

        /// <summary>
        /// Occurs when the server is stopped.
        /// </summary>
        public static event EventHandler Stopped;

        /// <summary>
        /// Event fired when ASP server recieves a connection
        /// </summary>
        public static event AspRequest RequestRecieved;

        /// <summary>
        /// Static constructor
        /// </summary>
        static HttpServer()
        {
            // Create our Server and Access logs
            AspStatsLog = new LogWriter(Path.Combine(Program.RootPath, "Logs", "AspServer.log"));
            HttpAccessLog = new LogWriter(Path.Combine(Program.RootPath, "Logs", "AspAccess.log"), true);

            // Get a list of all our local IP addresses
            LocalIPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList();

            // Create our conenction pool
            ConnectionPool = new SemaphoreSlim(50, 50);

            // Create our HttpListener instance
            CreateHttpListener();

            // Create our RazorEngine service
            CreateRazorService();

            // Try and clear any old cache files
            ClearRazorCache();

            // Set the Model type once to keep things speedy
            ModelType = typeof(BF2PageModel);
        }

        /// <summary>
        /// Start the ASP listener, and Connects to the stats database
        /// </summary>
        public static void Start()
        {
            // Can't start if we are already running!
            if (IsRunning) return;

            // === Try to connect to the database
            using (StatsDatabase Database = new StatsDatabase())  
            {
                if (!Database.TablesExist)
                {
                    string message = "In order to use the Private Stats feature of this program, we need to setup a database. "
                        + "You may choose to do this later by clicking \"Cancel\". Would you like to setup the database now?";
                    DialogResult R = MessageBox.Show(message, "Stats Database Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (R == DialogResult.Yes)
                        SetupManager.ShowDatabaseSetupForm(DatabaseMode.Stats, MainForm.Instance);

                    // Call the stopped event to Re-enable the main form's buttons
                    Stopped(null, EventArgs.Empty);
                    return;
                }

                // Initialize the stats manager
                StatsManager.Load(Database);

                // Drop the SQLite ip2nation country tables (old table versions)
                if (Database.DatabaseEngine == DatabaseEngine.Sqlite)
                {
                    string query = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='ip2nation'";
                    if (Database.ExecuteScalar<bool>(query)) // 0 count converts to false
                    {
                        Database.Execute("DROP TABLE IF EXISTS 'ip2nation';");
                        Database.Execute("DROP TABLE IF EXISTS 'ip2nationcountries';");
                        Database.Execute("VACUUM;");
                    }
                }
            }

            // === Compile our templates
            string path = Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Views");
            foreach (string file in Directory.EnumerateFiles(path, "*.cshtml"))
            {
                // If this template file is loaded already, then skip
                string fileName = Path.GetFileName(file);
                if (Engine.Razor.IsTemplateCached(fileName, ModelType))
                    continue;

                // Open the file, and compile it
                try
                {
                    using (FileStream stream = File.OpenRead(file))
                    using (StreamReader reader = new StreamReader(stream))
                        Engine.Razor.Compile(reader.ReadToEnd(), fileName, ModelType);
                }
                catch (TemplateCompilationException e)
                {
                    // Show the Exception form so the user can view
                    DialogResult Res = ExceptionForm.ShowTemplateError(e, file);

                    // If the user clicked "Quit", we stop
                    if (Res == DialogResult.Abort) return;
                }
            }
            

            // === Load XML stats and awards files
            Bf2StatsData.Load();
            BackendAwardData.BuildAwardData();

            // Start the Listener and accept new connections
            try
            {
                Listener.Start();
                Listener.BeginGetContext(HandleRequest, Listener);
            }
            catch (ObjectDisposedException)
            {
                // If we are disposed (happens when port 80 was in use already before, and we tried to start)
                // Then we need to start over with a new Listener
                CreateHttpListener();
                Listener.Start();
                Listener.BeginGetContext(HandleRequest, Listener);
            }
                
            // Fire Startup Event
            Started(null, EventArgs.Empty);
        }

        /// <summary>
        /// Stops the ASP listener, and unbinds from the port.
        /// </summary>
        public async static void Stop()
        {
            if (IsRunning)
            {
                // Call Stopping Event to disconnect clients
                if (Stopping != null)
                    Stopping(null, EventArgs.Empty);

                try
                {
                    // Stop listening for HTTP requests
                    Listener.Stop();

                    // Fire the stopped event
                    Stopped(null, EventArgs.Empty);
                }
                catch(Exception E)
                {
                    Program.ErrorLog.Write("ERROR: [HttpServer.Stop] " + E.Message);
                }

                SessionRequests = 0;
            }

            // Wait on pending Snapshots
            if (StatsManager.ImportTask != null && StatsManager.ImportTask.Status == TaskStatus.Running)
                await StatsManager.ImportTask;
        }

        /// <summary>
        /// Accepts the connection
        /// </summary>
        private static async void HandleRequest(IAsyncResult Sync)
        {
            bool Waiting = false;

            try
            {
                // Finish accepting the client
                HttpListenerContext Context = Listener.EndGetContext(Sync);
                await Task.Run(async() =>
                {
                    // Grab our connection
                    HttpClient Client = new HttpClient(Context);

                    // Wait for a connection slot to open up
                    await ConnectionPool.WaitAsync();

                    // Begin acceptinging another connection
                    if (!Waiting && IsRunning)
                    {
                        Listener.BeginGetContext(HandleRequest, Listener);
                        Waiting = true;
                    }

                    // Process the client connection
                    ProcessRequest(Client);
                });
            }
            catch (HttpListenerException e)
            {
                // Thread abort, or application abort request... Ignore
                if (e.ErrorCode == 995)
                    return;

                // Log error
                Program.ErrorLog.Write(
                    "ERROR: [HttpServer.HandleRequest] \r\n\t - {0}\r\n\t - ErrorCode: {1}", 
                    e.Message, 
                    e.ErrorCode
                );
            }
            catch (Exception e)
            {
                ExceptionHandler.GenerateExceptionLog(e);
                Program.ErrorLog.Write("ERROR: [HttpServer.HandleRequest] \r\n\t - {0}", e.Message);
            }
            finally
            {
                // Begin Listening again
                if (!Waiting && IsRunning)
                    Listener.BeginGetContext(HandleRequest, Listener);
            }
        }

        /// <summary>
        /// Handles the Http Connecting client and processes the HttpResponse
        /// </summary>
        private static void ProcessRequest(HttpClient Client)
        {
            // Update connection count
            Interlocked.Increment(ref SessionRequests);

            // Make sure request method is supported
            if (!AcceptableMethods.Contains(Client.Request.HttpMethod))
            {
                Client.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                Client.Response.Send();
                return;
            }

            // Process Request
            try
            {
                // Get our requested document
                string Document = Client.Request.Url.AbsolutePath.ToLower().TrimStart(new char[] { '/' }); ;
                if (Document.StartsWith("asp"))
                {
                    // Get our requested document's Controller
                    ASPController Controller = GetASPController(Client, Path.GetFileName(Document));
                    if (Controller != null)
                        // The controller will take over from here and process the response
                        Controller.HandleRequest();
                    else 
                        // Send a 404 if we dont have a controller for this request
                        Client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else if (!Program.Config.BF2S_Enabled) // If we dont have BF2S enabled, deny page access
                {
                    // Set service is unavialable
                    Client.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    Client.Response.Send();
                }
                else // Bf2sClone
                {
                    // Remove bf2stats folder and trim out the forward slashes
                    Document = Document.Replace("bf2stats", "").Trim(new char[] { '/' });
                    if (String.IsNullOrWhiteSpace(Document))
                        Document = "index";

                    // If the document has an extension, we are loading a resource (js, css, jpg) instead of a page
                    if (Path.HasExtension(Document))
                    {
                        // First we get the relative path with no empty entries. Path.Combine also checks for invalid characters
                        string RelaPath = Path.Combine(Document.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries));
                        string FullPath = Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Resources", RelaPath);
                        if (File.Exists(FullPath))
                        {
                            // Set the content type based from our extension
                            string Ext = Path.GetExtension(RelaPath).TrimStart('.');
                            if (MIMETypes.ContainsKey(Ext))
                                Client.Response.ContentType = MIMETypes[Ext];

                            // Send response
                            Client.Response.Send(File.ReadAllBytes(FullPath));
                        }
                        else
                        {
                            // Resource doesn't exist
                            Client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                    else
                    {
                        // Convert our document path into an MvC route
                        MvcRoute Route = new MvcRoute(Document);

                        // Try and fetch the controller, and handle the request
                        Controller Cont = GetBf2StatsController(Route.Controller, Client);
                        if (Cont != null)
                        {
                            // We let the Controller handle the request from here
                            Cont.HandleRequest(Route);
                        }
                        // Check the Razor engine to see if we have this template compiled...
                        else if (Engine.Razor.IsTemplateCached(Route.Controller + ".cshtml", ModelType))
                        {
                            // Custom made template with No Controller
                            Client.Response.ContentType = "text/html";
                            Client.Response.ResponseBody.Append(
                                Engine.Razor.Run(Route.Controller + ".cshtml", ModelType, new IndexModel(Client))
                            );
                        }
                        else
                        {
                            // Show 404
                            Client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            catch (DbConnectException e)
            {
                string message = e.InnerException?.Message ?? e.Message;
                Program.ErrorLog.Write("WARNING: [HttpServer.ProcessRequest] Unable to connect to database: " + message);

                // Set service is unavialable
                Client.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                Client.Response.Send();
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("ERROR: [HttpServer.ProcessRequest] " + e.Message);
                ExceptionHandler.GenerateExceptionLog(e);
                if (!Client.ResponseSent)
                {
                    // Internal service error
                    Client.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Client.Response.Send();
                }
            }
            finally
            {
                // Make sure a response is sent to prevent client hang
                if (!Client.ResponseSent)
                    Client.Response.Send();

                // Open a connection
                ConnectionPool.Release();

                // Fire this last!
                RequestRecieved();
            }
        }

        /// <summary>
        /// Returns the specified ASPController object for handling the ASP response, or
        /// null if the Document does not have a controller
        /// </summary>
        /// <param name="Client">The request client</param>
        /// <param name="Document">The requested document</param>
        /// <param name="Database">The stats database connection</param>
        private static ASPController GetASPController(HttpClient Client, string Document)
        {
            switch (Document)
            {
                case "bf2statistics.php": return new SnapshotPost(Client);
                case "createplayer.aspx": return new CreatePlayer(Client);
                case "getbackendinfo.aspx": return new GetBackendInfo(Client);
                case "getawardsinfo.aspx": return new GetAwardsInfo(Client);
                case "getclaninfo.aspx": return new GetClanInfo(Client);
                case "getleaderboard.aspx": return new GetLeaderBoard(Client);
                case "getmapinfo.aspx": return new GetMapInfo(Client);
                case "getplayerid.aspx": return new GetPlayerID(Client);
                case "getplayerinfo.aspx": return new GetPlayerInfo(Client);
                case "getrankinfo.aspx": return new GetRankInfo(Client);
                case "getunlocksinfo.aspx": return new GetUnlocksInfo(Client);
                case "ranknotification.aspx": return new RankNotification(Client);
                case "searchforplayers.aspx": return new SearchForPlayers(Client);
                case "selectunlock.aspx": return new SelectUnlock(Client);
                default: return null;
            }
        }

        /// <summary>
        /// Returns the specified Controller object for handling the bf2stats response, or
        /// null if the Document does not have a controller
        /// </summary>
        /// <param name="Document">The requested document</param>
        /// <param name="Client">The connected client object</param>
        /// <param name="Database">The stats database connection</param>
        /// <returns></returns>
        private static Controller GetBf2StatsController(string Document, HttpClient Client)
        {
            switch (Document)
            {
                case "index": return new IndexController(Client);
                case "search": return new SearchController(Client);
                case "rankings": return new RankingsController(Client);
                case "myleaderboard": return new LeaderboardController(Client);
                case "player": return new PlayerController(Client);
                default: return null;
            }
        }

        /// <summary>
        /// Creates the HttpListener object, and configures the prefixes
        /// </summary>
        private static void CreateHttpListener()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://*/ASP/");
            Listener.Prefixes.Add("http://*/bf2stats/");
        }

        /// <summary>
        /// Creates the Razor Engine Service needed to properly handle the cshtml files
        /// </summary>
        private static void CreateRazorService()
        {
            // Setup RazorEngine
            TemplateServiceConfiguration config = new TemplateServiceConfiguration();
            config.CachingProvider = new DefaultCachingProvider();
            config.EncodedStringFactory = new RawStringFactory();
            config.DisableTempFileLocking = true;
            config.BaseTemplateType = typeof(HtmlTemplateBase<>);
            Engine.Razor = RazorEngineService.Create(config);
        }

        /// <summary>
        /// Removes all of the Razor Engine built temporary files from the AppData/Temp folder
        /// </summary>
        public static void ClearRazorCache()
        {
            // For safety
            if (IsRunning)
                throw new Exception("The Razor Cache cannot be cleared while the HttpServer is running.");

            try
            {
                // Get our [User]/AppData/Local/Temp folder Location
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string path = Path.Combine(folderPath, "Temp");

                // Clear dynamic cache files
                foreach (string dir in Directory.GetDirectories(path, "RazorEngine_*"))
                    Directory.Delete(dir, true);
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("NOTICE: [HttpServer.ClearRazorCache] " + e.Message);
            }
        }
    }
}
