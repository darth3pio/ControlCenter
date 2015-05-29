using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using BF2Statistics.ASP.StatsProcessor;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Web;
using System.Net.Sockets;

namespace BF2Statistics.ASP
{
    /// <summary>
    /// The StatsManager is used to provide methods that allow you to
    /// Validate ASP servers, Authorize game server IP addresses to post
    /// snapshot data, and import snapshot data into the stats database.
    /// </summary>
    class StatsManager
    {
        // Define Min/Max PID numbers for players
        const int DEFAULT_PID = 29000000;
        const int MAX_PID = 30000000;

        /// <summary>
        /// The Queue in which snapshots will be stored in
        /// </summary>
        protected static ConcurrentEventQueue<Snapshot> SnapshotQueue = new ConcurrentEventQueue<Snapshot>();

        /// <summary>
        /// The Import Process for snapshots
        /// </summary>
        protected static Task ImportTask;

        /// <summary>
        /// Event fires when a snapshot has been sucessfully recieved
        /// </summary>
        public static event SnapshotRecieved SnapshotReceived;

        /// <summary>
        /// On Finish Event
        /// </summary>
        public static event SnapshotProccessed SnapshotProcessed;

        /// <summary>
        /// Indicates the number of snapshots that have been accepted
        /// </summary>
        public static int SnapshotsRecieved { get; protected set; }

        /// <summary>
        /// Indicates the number of snapshots processed successfully
        /// </summary>
        public static int SnapshotsCompleted { get; protected set; }

        /// <summary>
        /// The current highest player ID value (Incremented)
        /// </summary>
        protected static int PlayerPid = 0;

        /// <summary>
        /// The current Lowest player ID value (Decremented)
        /// </summary>
        protected static int AiPid = 0;

        /// <summary>
        /// Static constructor to register for Queue Events
        /// </summary>
        static StatsManager()
        {
            // When a snapshot is added to Queue, run the Process method
            SnapshotQueue.ItemEnqueued += (s, e) =>  
            {
                SnapshotsRecieved++;
                ProcessQueue(); 
            };
        }

        /// <summary>
        /// Method to be called everytime the HttpStatsServer is started
        /// </summary>
        public static void Load(StatsDatabase Driver)
        {
            // Get the lowest Offline PID from the database
            var Rows = Driver.Query(
                String.Format(
                    "SELECT COALESCE(MIN(id), {0}) AS min, COALESCE(MAX(id), {0}) AS max FROM player WHERE id < {1}",
                    DEFAULT_PID, MAX_PID
                )
            );

            int Lowest = Int32.Parse(Rows[0]["min"].ToString());
            int Highest = Int32.Parse(Rows[0]["max"].ToString());
            AiPid = (Lowest > DEFAULT_PID) ? DEFAULT_PID : Lowest;
            PlayerPid = (Highest < DEFAULT_PID) ? DEFAULT_PID : Highest;
        }

        /// <summary>
        /// Adds a server's posted snapshot into the Snapshot Processing Queue, which
        /// will process the snapshot as soon as possible. This method is Non-Blocking.
        /// </summary>
        /// <remarks>
        /// Any errors that occur during the actual import of the data will be
        /// logged inside the StatsDebug log
        /// </remarks>
        /// <param name="Data">The snapshot data provided by the server.</param>
        /// <param name="ServerAddress">The IP address of the server.</param>
        /// <exception cref="UnauthorizedAccessException">
        ///     Thrown if the Server IP is not authorized to post game data to this server
        /// </exception>
        /// <exception cref="InvalidDataException">
        ///     Thrown if the provided Snapshot data is not valid, and cannot be processed
        /// </exception>
        public static void QueueServerSnapshot(string Data, IPAddress ServerAddress)
        {
            // Make sure the server is authorized
            if (!IsAuthorizedGameServer(ServerAddress))
                throw new UnauthorizedAccessException("Un-Authorised Gameserver (Ip: " + ServerAddress + ")");

            // Create the Snapshot Object
            Snapshot Snap = new Snapshot(Data, ServerAddress);

            // Add snapshot to Queue
            SnapshotQueue.Enqueue(Snap);
        }

        /// <summary>
        /// Returns whether the specified IP address is authorized to POST snapshot
        /// data to this ASP server. All local IP address are automatically authorized.
        /// </summary>
        /// <param name="RemoteIP">The server IP to authorize</param>
        /// <returns></returns>
        public static bool IsAuthorizedGameServer(IPAddress RemoteIP)
        {
            // Local is always authorized
            if (IPAddress.IsLoopback(RemoteIP) || HttpServer.LocalIPs.Contains(RemoteIP))
                return true;

            // Setup local vars
            IPAddress Ip;

            // Loop through all Config allowed game hosts, and determine if the remote host is allowed
            // to post snapshots here
            if (!String.IsNullOrWhiteSpace(Program.Config.ASP_GameHosts))
            {
                string[] Hosts = Program.Config.ASP_GameHosts.Split(',');
                foreach (string Host in Hosts)
                {
                    if (IPAddress.TryParse(Host, out Ip) && Ip.Equals(RemoteIP))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether the specified URI is a valid, and available ASP Service
        /// </summary>
        /// <param name="Url">The root url to the ASP server. Dont include the /ASP/ path!</param>
        public static void ValidateASPService(string Url)
        {
            // Create the ASP request, and fetch the http response
            WebRequest Request = WebRequest.Create(new Uri(Url.TrimEnd('/') + "/ASP/getbackendinfo.aspx"));
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();

            // Make sure that we connected successfully
            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Unable to connect to the Gamespy ASP Webservice: " + Response.StatusDescription);

            // Parse the response message
            using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
            {
                string Lines = Reader.ReadToEnd().TrimStart();

                // Does the player exist?
                if (!Lines.StartsWith("O"))
                    throw new Exception("The ASP webserver didnt not respond with a proper ASP response!");
            }
        }

        /// <summary>
        /// Returns a new PID number for use when creating a new player
        /// for the stats database
        /// </summary>
        /// <returns></returns>
        public static int GenerateNewPlayerPid()
        {
            // Thread safe increment
            return Interlocked.Increment(ref PlayerPid);
        }

        /// <summary>
        /// Returns a new PID number for use when creating a new AI Bot
        /// for the stats database
        /// </summary>
        /// <returns></returns>
        public static int GenerateNewAIPid()
        {
            // Thread safe decrement
            return Interlocked.Decrement(ref AiPid);
        }

        /// <summary>
        /// If not already running, Whenever a snapshot is added to the Processing Queue,
        /// This method will get called to Dequeue and process all snapshots
        /// </summary>
        protected static void ProcessQueue()
        {
            // Make sure we arent already processing
            if (ImportTask != null && ImportTask.Status == TaskStatus.Running)
                return;

            // Do this in another thread
            ImportTask = Task.Run(() =>
            {
                // Fire event
                if (SnapshotReceived != null)
                    SnapshotReceived();

                // Loop through all of the snapshots and process em
                while (SnapshotQueue.Count > 0)
                {
                    // Fetch our snapshot
                    Snapshot Snap;
                    if (!SnapshotQueue.TryDequeue(out Snap))
                        throw new Exception("Unable to Dequeue Snapshot Item");

                    // Attempt to process the snapshot
                    bool WasSuccess = false;
                    try
                    {
                        Snap.ProcessData();
                        WasSuccess = true;
                        SnapshotsCompleted++;

                        // Fire event
                        if (SnapshotProcessed != null)
                            SnapshotProcessed();

                        // Alert user
                        Notify.Show("Snapshot Processed Successfully!", "From Server IP: " + Snap.ServerIp, AlertType.Success);
                    }
                    catch (Exception E)
                    {
                        HttpServer.AspStatsLog.Write("ERROR: [SnapshotProcess] " + E.Message + " @ " + E.TargetSite);
                        ExceptionHandler.GenerateExceptionLog(E);
                    }

                    // Create backup of snapshot
                    try
                    {
                        // Backup the snapshot
                        string FileName = Snap.ServerPrefix + "-" + Snap.MapName + "_" + Snap.RoundEndDate.ToLocalTime().ToString("yyyyMMdd_HHmm") + ".txt";
                        File.AppendAllText(
                            (WasSuccess) ? Path.Combine(Paths.SnapshotProcPath, FileName) : Path.Combine(Paths.SnapshotTempPath, FileName),
                            Snap.DataString
                        );
                    }
                    catch (Exception E)
                    {
                        HttpServer.AspStatsLog.Write("WARNING: [SnapshotFileOperations] Unable to create Snapshot Backup File: " + E.Message);
                    }
                }
            })
            .ContinueWith((Action<Task>)delegate
            {
                // Create an exception log if we failed to import correctly
                if (ImportTask.IsFaulted)
                {
                    Program.ErrorLog.Write("ERROR: [SnapshotQueue] Failed to process all snapshots in Queue: " + ImportTask.Exception.Message);
                    ExceptionHandler.GenerateExceptionLog(ImportTask.Exception);
                }
            });
        }

        /// <summary>
        /// Exports a players stats and history into an Xml file
        /// </summary>
        /// <param name="XmlPath">The folder path to where the XML will be saved</param>
        /// <param name="Pid">Player ID</param>
        /// <param name="Name">Player Name</param>
        public static void ExportPlayerXml(string XmlPath, int Pid, string Name)
        {
            //  Create full path
            string sPath = Path.Combine(
                XmlPath,
                String.Format("{0}_{1}_{2}.xml", Name.Trim().MakeFileNameSafe(), Pid, DateTime.Now.ToString("yyyyMMdd_HHmm"))
            );

            // Delete file if it exists already
            if (File.Exists(sPath))
                File.Delete(sPath);

            // Create XML Settings
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true;
            Settings.IndentChars = "\t";
            Settings.NewLineChars = Environment.NewLine;
            Settings.NewLineHandling = NewLineHandling.Replace;

            // Write XML data
            using (StatsDatabase Driver = new StatsDatabase())
            using (XmlWriter Writer = XmlWriter.Create(sPath, Settings))
            {
                // Player Element
                Writer.WriteStartDocument();
                Writer.WriteStartElement("Player");

                // Manifest
                Writer.WriteStartElement("Info");
                Writer.WriteElementString("Pid", Pid.ToString());
                Writer.WriteElementString("Name", Name.EscapeXML());
                Writer.WriteElementString("BackupDate", DateTime.Now.ToString());
                Writer.WriteEndElement();

                // Start Tables Element
                Writer.WriteStartElement("TableData");

                // Add each tables data
                foreach (string Table in StatsDatabase.PlayerTables)
                {
                    // Open table tag
                    Writer.WriteStartElement(Table);

                    // Fetch row
                    List<Dictionary<string, object>> Rows;
                    if (Table == "kills")
                        Rows = Driver.Query(String.Format("SELECT * FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid));
                    else
                        Rows = Driver.Query(String.Format("SELECT * FROM {0} WHERE id={1}", Table, Pid));

                    // Write each row's columns with its value to the xml file
                    foreach (Dictionary<string, object> Row in Rows)
                    {
                        // Open Row tag
                        Writer.WriteStartElement("Row");
                        foreach (KeyValuePair<string, object> Column in Row)
                        {
                            if (Column.Key == "name")
                                Writer.WriteElementString(Column.Key, Column.Value.ToString().EscapeXML());
                            else
                                Writer.WriteElementString(Column.Key, Column.Value.ToString());
                        }

                        // Close Row tag
                        Writer.WriteEndElement();
                    }

                    // Close table tag
                    Writer.WriteEndElement();
                }

                // Close Tags and File
                Writer.WriteEndElement();  // Close Tables Element
                Writer.WriteEndElement();  // Close Player Element
                Writer.WriteEndDocument(); // End and Save file
            }
        }

        /// <summary>
        /// Imports a Player XML Sheet from the specified path
        /// </summary>
        /// <param name="XmlPath">The full path to the XML file</param>
        public static void ImportPlayerXml(string XmlPath)
        {
            // Connect to database first!
            using (StatsDatabase Driver = new StatsDatabase())
            {
                // Load elements
                XDocument Doc = XDocument.Load(new FileStream(XmlPath, FileMode.Open, FileAccess.Read));
                XElement Info = Doc.Root.Element("Info");
                XElement TableData = Doc.Root.Element("TableData");

                // Make sure player doesnt already exist
                int Pid = Int32.Parse(Info.Element("Pid").Value);
                if (Driver.PlayerExists(Pid))
                    throw new Exception(String.Format("Player with PID {0} already exists!", Pid));

                // Begin Transaction
                using (DbTransaction Transaction = Driver.BeginTransaction())
                {
                    try
                    {
                        // Loop through tables
                        foreach (XElement Table in TableData.Elements())
                        {
                            // Loop through Rows
                            foreach (XElement Row in Table.Elements())
                            {
                                InsertQueryBuilder QueryBuilder = new InsertQueryBuilder(Table.Name.LocalName, Driver);
                                foreach (XElement Col in Row.Elements())
                                {
                                    if (Col.Name.LocalName == "name")
                                        QueryBuilder.SetField(Col.Name.LocalName, Col.Value.UnescapeXML());
                                    else
                                        QueryBuilder.SetField(Col.Name.LocalName, Col.Value);
                                }

                                QueryBuilder.Execute();
                            }
                        }

                        // Commit Transaction
                        Transaction.Commit();
                    }
                    catch
                    {
                        Transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
