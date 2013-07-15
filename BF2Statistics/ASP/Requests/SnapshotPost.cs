using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BF2Statistics.ASP.Requests
{
    class SnapshotPost
    {
        public static event SnapshotRecieved SnapshotReceived;

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

                // Loop through all Config allowed game hosts, and determine if the remote host is allowed
                // to post snapshots here
                if (!String.IsNullOrWhiteSpace(MainForm.Config.ASP_GameHosts))
                {
                    string[] Hosts = MainForm.Config.ASP_GameHosts.Split(',');
                    foreach (string Host in Hosts)
                    {
                        if (IPAddress.TryParse(Host, out Ip) && Ip.Equals(RemoteIP.Address))
                        {
                            IsValid = true;
                            break;
                        }
                    }
                }

                // If we are not on the GameHost list, too bad sucka!
                if (!IsValid)
                {
                    //ASPServer.UpdateStatus("Denied snapshot data from " + RemoteIP.Address.ToString());
                    Notify.Show("Snapshot Denied!", "Invalid Server IP: " + RemoteIP.Address.ToString(), AlertType.Warning);
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
                SnapObj = new Snapshot(Snapshot, DateTime.Now);

                // Make sure data is valid!
                if (SnapObj.IsValidSnapshot)
                {
                    // Backup the snapshot
                    FileName = SnapObj.ServerPrefix + "-" + SnapObj.MapName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
                    File.AppendAllText(Path.Combine(Paths.SnapshotTempPath, FileName), Snapshot);
                }
                else
                {
                    Notify.Show("Snapshot Data NOT Complete or Invalid!", AlertType.Warning);
                    //ASPServer.UpdateStatus("Snapshot recieved was invalid!");
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
                File.Move(Path.Combine(Paths.SnapshotTempPath, FileName), Path.Combine(Paths.SnapshotProcPath, FileName));

                // Notify User
                Notify.Show("Snapshot Proccessed Successfully!", "From Server IP: " + RemoteIP.Address.ToString(), AlertType.Success);

                // Fire Event
                SnapshotReceived(true);
            }
            catch (Exception E)
            {
                // Notify user
                Notify.Show("Error Processing Snapshot!", E.Message, AlertType.Warning);
                ASPServer.Log(E.Message);

                // Fire event
                SnapshotReceived(false);
            }
        }
    }
}
