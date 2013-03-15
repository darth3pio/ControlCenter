using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace BF2Statistics.ASP.Requests
{
    class SnapshotPost
    {
        /// <summary>
        /// Full path to where Temporary snapshots are stored
        /// </summary>
        public static readonly string TempPath = Path.Combine(MainForm.Root, "Snapshots", "Temp");

        /// <summary>
        /// Full path to where the Processed snapshots are stored
        /// </summary>
        public static readonly string ProcPath = Path.Combine(MainForm.Root, "Snapshots", "Processed");

        public SnapshotPost(HttpListenerRequest Request, ASPResponse Response)
        {
            // Begin Output
            FormattedOutput Out = new FormattedOutput("response");

            // First and foremost. Make sure that we are authorized to be here!
            IPEndPoint RemoteIP = Request.LocalEndPoint;
            if (!Request.IsLocal)
            {
                // Setup local vars
                bool IsValid = false;
                IPAddress Ip;

                // Loop through all Config allowed game hosts
                if (!String.IsNullOrWhiteSpace(MainForm.Config.ASP_GameHosts))
                {
                    string[] Hosts = MainForm.Config.ASP_GameHosts.Split(',');
                    foreach (string Host in Hosts)
                    {
                        if (IPAddress.TryParse(Host, out Ip))
                            if (Ip.Equals(RemoteIP.Address))
                                IsValid = true;
                    }
                }

                // If we are not on the GameHost list, too bad sucka!
                if (!IsValid)
                {
                    ASPServer.UpdateStatus("Denied snapshot data from " + RemoteIP.Address.ToString());
                    if (Request.UserAgent == "GameSpyHTTP/1.0")
                    {
                        Out.AddRow("Unauthorised Gameserver");
                        Response.AddData(Out);
                        Response.IsValidData(false);
                    }
                    else
                        Response.StatusCode = 403;

                    Response.Send();
                    return;
                }
            }

            // Make sure we have post data
            if (!Request.HasEntityBody)
            {
                // No Post Data
                if (Request.UserAgent == "GameSpyHTTP/1.0")
                {
                    Out.AddRow("SNAPSHOT Data NOT found!");
                    Response.AddData(Out);
                    Response.IsValidData(false);
                }
                else
                    Response.StatusCode = 400;

                Response.Send();
                return;
            }

            // Report
            ASPServer.UpdateStatus("Recieved snapshot from " + RemoteIP.Address.ToString());

            // Save the snapshot to the snapshots path
            string Snapshot;
            using (StreamReader Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                Snapshot = Reader.ReadToEnd();

            // Create our snapshot object and filename
            Snapshot SnapObj;
            string FileName;

            try
            {
                // Create the Snapshot Object
                SnapObj = new Snapshot(Snapshot);

                // Make sure data is valid!
                if (SnapObj.IsValidSnapshot)
                {
                    // Backup the snapshot
                    FileName = SnapObj.ServerPrefix + "-" + SnapObj.MapName + "_" + DateTime.Now.ToString("yyyyMMdd_HHMM") + ".txt";
                    File.AppendAllText(Path.Combine(TempPath, FileName), Snapshot);
                }
                else
                {
                    ASPServer.UpdateStatus("Snapshot recieved was invalid!");
                    Out.AddRow("SNAPSHOT Data NOT complete or invalid!");
                    Response.AddData(Out);
                    Response.IsValidData(false);
                    Response.Send();
                    return;
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 503;
                Response.Send();
                return;
            }

            // Tell the server we are good to go
            Out.AddRow("OK");
            Response.AddData(Out);
            Response.Send();

            // Have the snapshot class handle the rest :)
            try
            {
                // Do the snapshot
                SnapObj.Process();

                // Move the Temp snapshot to the Processed folder
                File.Move(Path.Combine(TempPath, FileName), Path.Combine(ProcPath, FileName));

                // Report
                ASPServer.UpdateStatus("Processed snapshot from " + RemoteIP.Address.ToString());
            }
            catch (Exception E)
            {
                // Report
                ASPServer.UpdateStatus("Error processing snapshot!\r\n" + E.Message);
                ASPServer.Log(E.Message);
            }
        }
    }
}
