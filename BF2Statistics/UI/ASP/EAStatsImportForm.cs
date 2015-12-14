using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Windows.Forms;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Web;
using BF2Statistics.Web.ASP;

namespace BF2Statistics
{
    public partial class EAStatsImportForm : Form
    {
        /// <summary>
        /// Background worker for importing player stats
        /// </summary>
        protected BackgroundWorker bWorker;

        /// <summary>
        /// Constructor
        /// </summary>
        public EAStatsImportForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Import Stats Button Click Event
        /// </summary>
        private void ImportBtn_Click(object sender, EventArgs e)
        {
            // Make sure PID text box is a valid PID
            if (!Validator.IsValidPID(PidTextBox.Text))
            {
                MessageBox.Show("The player ID entered is NOT a valid PID. Please try again.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Establist Database connection
            try
            {
                using (StatsDatabase Database = new StatsDatabase())
                {
                    // Make sure the PID doesnt exist already
                    int Pid = Int32.Parse(PidTextBox.Text);
                    if (Database.PlayerExists(Pid))
                    {
                        MessageBox.Show("The player ID entered already exists.",
                            "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Show Task Form
                    TaskForm.Show(this, "Import ASP Stats", "Importing ASP Stats...", false, ProgressBarStyle.Blocks, 13);

                    // Setup the worker
                    bWorker = new BackgroundWorker();
                    bWorker.WorkerSupportsCancellation = false;
                    bWorker.WorkerReportsProgress = true;

                    // Run Worker
                    bWorker.DoWork += bWorker_ImportEaStats;
                    bWorker.ProgressChanged += (s, ea) =>
                    {
                        TaskForm.Progress.Report(new TaskProgressUpdate(ea.UserState.ToString(), ea.ProgressPercentage));
                    };
                    bWorker.RunWorkerCompleted += bWorker_RunWorkerCompleted;
                    bWorker.RunWorkerAsync(PidTextBox.Text);
                }
            }
            catch (DbConnectException Ex)
            {
                ExceptionForm.ShowDbConnectError(Ex);
                HttpServer.Stop();
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Imports a players stats from the official gamespy ASP.
        /// This method is to be used in a background worker
        /// </summary>
        public void bWorker_ImportEaStats(object sender, DoWorkEventArgs e)
        {
            // Setup variables
            BackgroundWorker Worker = (BackgroundWorker)sender;
            int Pid = Int32.Parse(e.Argument.ToString());

            // Do work
            using (StatsDatabase Driver = new StatsDatabase())
            {
                // Make sure the player doesnt exist!
                if (Driver.PlayerExists(Pid))
                    throw new Exception(String.Format("Player with PID {0} already exists!", Pid));

                // Build variables
                Uri GsUrl;
                WebRequest Request;
                HttpWebResponse Response;
                List<string[]> PlayerLines;
                List<string[]> AwardLines;
                List<string[]> MapLines;
                InsertQueryBuilder Query;

                // Create Request
                string Url = String.Format(
                    "getplayerinfo.aspx?pid={0}&info=per*,cmb*,twsc,cpcp,cacp,dfcp,kila,heal,rviv,rsup,rpar,tgte,dkas,dsab,cdsc,rank,cmsc,kick,kill,deth,suic,"
                    + "ospm,klpm,klpr,dtpr,bksk,wdsk,bbrs,tcdr,ban,dtpm,lbtl,osaa,vrk,tsql,tsqm,tlwf,mvks,vmks,mvn*,vmr*,fkit,fmap,fveh,fwea,wtm-,wkl-,wdt-,"
                    + "wac-,wkd-,vtm-,vkl-,vdt-,vkd-,vkr-,atm-,awn-,alo-,abr-,ktm-,kkl-,kdt-,kkd-",
                    Pid);
                Worker.ReportProgress(1, "Requesting Player Stats");
                GsUrl = new Uri("http://bf2web.gamespy.com/ASP/" + Url);
                Request = WebRequest.Create(GsUrl);

                // Get response
                Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Unable to connect to the Gamespy ASP Webservice!");

                // Read response data
                Worker.ReportProgress(2, "Parsing Stats Response");
                PlayerLines = new List<string[]>();
                using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
                    while (!Reader.EndOfStream)
                        PlayerLines.Add(Reader.ReadLine().Split('\t'));

                // Does the player exist?
                if (PlayerLines[0][0] != "O")
                    throw new Exception("Player does not exist on the gamespy servers!");

                // Fetch player mapinfo
                Worker.ReportProgress(3, "Requesting Player Map Data");
                GsUrl = new Uri(String.Format("http://bf2web.gamespy.com/ASP/getplayerinfo.aspx?pid={0}&info=mtm-,mwn-,mls-", Pid));
                Request = WebRequest.Create(GsUrl);

                // Get response
                Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Unable to connect to the Gamespy ASP Webservice!");

                // Read response data
                Worker.ReportProgress(4, "Parsing Map Data Response");
                MapLines = new List<string[]>();
                using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
                    while (!Reader.EndOfStream)
                        MapLines.Add(Reader.ReadLine().Split('\t'));

                // Fetch player awards
                Worker.ReportProgress(5, "Requesting Player Awards");
                GsUrl = new Uri(String.Format("http://bf2web.gamespy.com/ASP/getawardsinfo.aspx?pid={0}", Pid));
                Request = WebRequest.Create(GsUrl);

                // Get response
                Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Unable to connect to the Gamespy ASP Webservice!");

                // Read response data
                Worker.ReportProgress(6, "Parsing Player Awards Response");
                AwardLines = new List<string[]>();
                using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
                    while (!Reader.EndOfStream)
                        AwardLines.Add(Reader.ReadLine().Split('\t'));

                // === Process Player Info === //

                // Parse Player Data
                Dictionary<string, string> PlayerInfo = StatsParser.ParseHeaderData(PlayerLines[3], PlayerLines[4]);
                int Rounds = Int32.Parse(PlayerInfo["mode0"]) + Int32.Parse(PlayerInfo["mode1"]) + Int32.Parse(PlayerInfo["mode2"]);

                // Begin database transaction
                DbTransaction Transaction = Driver.BeginTransaction();

                // Wrap all DB inserts into a try block so we can rollback on error
                try
                {
                    // Insert Player Data
                    Worker.ReportProgress(7, "Inserting Player Data Into Table: player");
                    Query = new InsertQueryBuilder("player", Driver);
                    Query.SetField("id", Pid);
                    Query.SetField("name", " " + PlayerInfo["nick"].Trim()); // Online accounts always start with space in bf2stats
                    Query.SetField("country", "xx");
                    Query.SetField("time", PlayerInfo["time"]);
                    Query.SetField("rounds", Rounds);
                    Query.SetField("ip", "127.0.0.1");
                    Query.SetField("score", PlayerInfo["scor"]);
                    Query.SetField("cmdscore", PlayerInfo["cdsc"]);
                    Query.SetField("skillscore", PlayerInfo["cmsc"]);
                    Query.SetField("teamscore", PlayerInfo["twsc"]);
                    Query.SetField("kills", PlayerInfo["kill"]);
                    Query.SetField("deaths", PlayerInfo["deth"]);
                    Query.SetField("captures", PlayerInfo["cpcp"]);
                    Query.SetField("captureassists", PlayerInfo["cacp"]);
                    Query.SetField("defends", PlayerInfo["dfcp"]);
                    Query.SetField("damageassists", PlayerInfo["kila"]);
                    Query.SetField("heals", PlayerInfo["heal"]);
                    Query.SetField("revives", PlayerInfo["rviv"]);
                    Query.SetField("ammos", PlayerInfo["rsup"]);
                    Query.SetField("repairs", PlayerInfo["rpar"]);
                    Query.SetField("driverspecials", PlayerInfo["dsab"]);
                    Query.SetField("suicides", PlayerInfo["suic"]);
                    Query.SetField("killstreak", PlayerInfo["bksk"]);
                    Query.SetField("deathstreak", PlayerInfo["wdsk"]);
                    Query.SetField("rank", PlayerInfo["rank"]);
                    Query.SetField("banned", PlayerInfo["ban"]);
                    Query.SetField("kicked", PlayerInfo["kick"]);
                    Query.SetField("cmdtime", PlayerInfo["tcdr"]);
                    Query.SetField("sqltime", PlayerInfo["tsql"]);
                    Query.SetField("sqmtime", PlayerInfo["tsqm"]);
                    Query.SetField("lwtime", PlayerInfo["tlwf"]);
                    Query.SetField("wins", PlayerInfo["wins"]);
                    Query.SetField("losses", PlayerInfo["loss"]);
                    Query.SetField("joined", PlayerInfo["jond"]);
                    Query.SetField("rndscore", PlayerInfo["bbrs"]);
                    Query.SetField("lastonline", PlayerInfo["lbtl"]);
                    Query.SetField("mode0", PlayerInfo["mode0"]);
                    Query.SetField("mode1", PlayerInfo["mode1"]);
                    Query.SetField("mode2", PlayerInfo["mode2"]);
                    Query.Execute();

                    // Insert Army Data
                    Worker.ReportProgress(8, "Inserting Player Data Into Table: army");
                    Query = new InsertQueryBuilder("army", Driver);
                    Query.SetField("id", Pid);
                    for (int i = 0; i < 10; i++)
                    {
                        Query.SetField("time" + i, PlayerInfo["atm-" + i]);
                        Query.SetField("win" + i, PlayerInfo["awn-" + i]);
                        Query.SetField("loss" + i, PlayerInfo["alo-" + i]);
                        Query.SetField("best" + i, PlayerInfo["abr-" + i]);
                    }
                    Query.Execute();

                    // Insert Kit Data
                    Worker.ReportProgress(9, "Inserting Player Data Into Table: kits");
                    Query = new InsertQueryBuilder("kits", Driver);
                    Query.SetField("id", Pid);
                    for (int i = 0; i < 7; i++)
                    {
                        Query.SetField("time" + i, PlayerInfo["ktm-" + i]);
                        Query.SetField("kills" + i, PlayerInfo["kkl-" + i]);
                        Query.SetField("deaths" + i, PlayerInfo["kdt-" + i]);
                    }
                    Query.Execute();

                    // Insert Vehicle Data
                    Worker.ReportProgress(10, "Inserting Player Data Into Table: vehicles");
                    Query = new InsertQueryBuilder("vehicles", Driver);
                    Query.SetField("id", Pid);
                    Query.SetField("timepara", 0);
                    for (int i = 0; i < 7; i++)
                    {
                        Query.SetField("time" + i, PlayerInfo["vtm-" + i]);
                        Query.SetField("kills" + i, PlayerInfo["vkl-" + i]);
                        Query.SetField("deaths" + i, PlayerInfo["vdt-" + i]);
                        Query.SetField("rk" + i, PlayerInfo["vkr-" + i]);
                    }
                    Query.Execute();

                    // Insert Weapon Data
                    Worker.ReportProgress(11, "Inserting Player Data Into Table: weapons");
                    Query = new InsertQueryBuilder("weapons", Driver);
                    Query.SetField("id", Pid);
                    for (int i = 0; i < 9; i++)
                    {
                        Query.SetField("time" + i, PlayerInfo["wtm-" + i]);
                        Query.SetField("kills" + i, PlayerInfo["wkl-" + i]);
                        Query.SetField("deaths" + i, PlayerInfo["wdt-" + i]);
                    }

                    // Knife
                    Query.SetField("knifetime", PlayerInfo["wtm-9"]);
                    Query.SetField("knifekills", PlayerInfo["wkl-9"]);
                    Query.SetField("knifedeaths", PlayerInfo["wdt-9"]);
                    // Shockpad
                    Query.SetField("shockpadtime", PlayerInfo["wtm-10"]);
                    Query.SetField("shockpadkills", PlayerInfo["wkl-10"]);
                    Query.SetField("shockpaddeaths", PlayerInfo["wdt-10"]);
                    // Claymore
                    Query.SetField("claymoretime", PlayerInfo["wtm-11"]);
                    Query.SetField("claymorekills", PlayerInfo["wkl-11"]);
                    Query.SetField("claymoredeaths", PlayerInfo["wdt-11"]);
                    // Handgrenade
                    Query.SetField("handgrenadetime", PlayerInfo["wtm-12"]);
                    Query.SetField("handgrenadekills", PlayerInfo["wkl-12"]);
                    Query.SetField("handgrenadedeaths", PlayerInfo["wdt-12"]);
                    // SF Weapn Data
                    Query.SetField("tacticaldeployed", PlayerInfo["de-6"]);
                    Query.SetField("grapplinghookdeployed", PlayerInfo["de-7"]);
                    Query.SetField("ziplinedeployed", PlayerInfo["de-8"]);

                    Query.Execute();

                    // === Process Awards Data === //
                    Worker.ReportProgress(12, "Inserting Player Awards");
                    List<Dictionary<string, string>> Awards = StatsParser.ParseAwards(AwardLines);
                    foreach (Dictionary<string, string> Award in Awards)
                    {
                        Query = new InsertQueryBuilder("awards", Driver);
                        Query.SetField("id", Pid);
                        Query.SetField("awd", Award["id"]);
                        Query.SetField("level", Award["level"]);
                        Query.SetField("earned", Award["when"]);
                        Query.SetField("first", Award["first"]);
                        Query.Execute();
                    }

                    // === Process Map Data === //
                    Worker.ReportProgress(13, "Inserting Player Map Data");
                    PlayerInfo = StatsParser.ParseHeaderData(MapLines[3], MapLines[4]);
                    int[] Maps = new int[] { 
                        0, 1, 2, 3, 4, 5, 6, 100, 101, 102, 103, 105, 
                        601, 300, 301, 302, 303, 304, 305, 306, 307, 
                        10, 11, 110, 200, 201, 202, 12 
                    };
                    foreach (int MapId in Maps)
                    {
                        if (PlayerInfo.ContainsKey("mtm-" + MapId))
                        {
                            Query = new InsertQueryBuilder("maps", Driver);
                            Query.SetField("id", Pid);
                            Query.SetField("mapid", MapId);
                            Query.SetField("time", PlayerInfo["mtm-" + MapId]);
                            Query.SetField("win", PlayerInfo["mwn-" + MapId]);
                            Query.SetField("loss", PlayerInfo["mls-" + MapId]);
                            Query.SetField("best", 0);
                            Query.SetField("worst", 0);
                            Query.Execute();
                        }
                    }

                    // Commit transaction
                    Transaction.Commit();
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                    throw;
                }
                finally
                {
                    // Dispose dispose the transaction
                    Transaction.Dispose();
                }
            }
        } 

        /// <summary>
        /// Finishes the import process
        /// </summary>
        private void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Close the task form
            TaskForm.CloseForm();

            // Close this form!
            if (e.Error != null)
            {
                Exception E = e.Error as Exception;
                MessageBox.Show(
                    "An error occured while trying to import player stats."
                    + Environment.NewLine + Environment.NewLine + E.Message,
                    "Import Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                this.Close();
                return;
            }

            Notify.Show("Player Imported Successfully!", "All the players stats and awards are now available on the server.", AlertType.Success);
            this.Close();
        }
    }
}
