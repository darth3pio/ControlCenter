using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BF2Statistics.ASP.Requests
{
    class SnapshotPost
    {
        public static event SnapshotRecieved SnapshotReceived;

        public SnapshotPost(HttpClient Client)
        {
            // First and foremost. Make sure that we are authorized to be here!
            IPEndPoint RemoteIP = Client.RemoteEndPoint;
            if (!Client.Request.IsLocal)
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
                    // Notify User
                    Notify.Show("Snapshot Denied!", "Invalid Server IP: " + RemoteIP.Address.ToString(), AlertType.Warning);
                    if (Client.Request.UserAgent == "GameSpyHTTP/1.0")
                    {
                        Client.Response.WriteResponseStart(false);
                        Client.Response.WriteHeaderLine("response");
                        Client.Response.WriteDataLine("Unauthorised Gameserver");
                    }
                    else
                        Client.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    Client.Response.Send();
                    return;
                }
            }

            // Make sure we have post data
            if (!Client.Request.HasEntityBody)
            {
                // No Post Data
                if (Client.Request.UserAgent == "GameSpyHTTP/1.0")
                {
                    Client.Response.WriteResponseStart(false);
                    Client.Response.WriteHeaderLine("response");
                    Client.Response.WriteDataLine("SNAPSHOT Data NOT found!");
                }
                else
                    Client.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                Client.Response.Send();
                return;
            }

            // Create our snapshot object and filename
            string Snapshot;
            Snapshot SnapObj;
            string FileName = String.Empty;
            bool BackupCreated = false;

            // Create snapshot backup file if the snapshot is valid
            try
            {
                // Read Snapshot
                using (StreamReader Reader = new StreamReader(Client.Request.InputStream, Client.Request.ContentEncoding))
                    Snapshot = Reader.ReadToEnd();

                // Create the Snapshot Object
                SnapObj = new Snapshot(Snapshot, DateTime.UtcNow);

                // Make sure data is valid!
                if (!SnapObj.IsValidSnapshot)
                {
                    Notify.Show("Error Processing Snapshot!", "Snapshot Data NOT Complete or Invalid!", AlertType.Warning);
                    Client.Response.WriteResponseStart(false);
                    Client.Response.WriteHeaderLine("response");
                    Client.Response.WriteDataLine("SNAPSHOT Data NOT complete or invalid!");
                    Client.Response.Send();
                    return;
                }
            }
            catch (Exception E)
            {
                ASPServer.Log("ERROR: [SnapshotPreProcess] " + E.Message);
                Client.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                Client.Response.Send();
                return;
            }

            // Create backup of snapshot
            try
            {
                // Backup the snapshot
                FileName = SnapObj.ServerPrefix + "-" + SnapObj.MapName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
                File.AppendAllText(Path.Combine(Paths.SnapshotTempPath, FileName), Snapshot);
                BackupCreated = true;
            }
            catch (Exception E)
            {
                BackupCreated = true;
                ASPServer.Log("WARNING: Unable to create Snapshot Backup File: " + E.Message);
            }

            // Tell the server we are good to go
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("response");
            Client.Response.WriteDataLine("OK");
            Client.Response.Send();

            // Have the snapshot class handle the rest :)
            try
            {
                // Do the snapshot
                SnapObj.Process();

                // Move the Temp snapshot to the Processed folder
                if(BackupCreated)
                    File.Move(Path.Combine(Paths.SnapshotTempPath, FileName), Path.Combine(Paths.SnapshotProcPath, FileName));

                // Notify User
                Notify.Show("Snapshot Processed Successfully!", "From Server IP: " + RemoteIP.Address.ToString(), AlertType.Success);

                // Fire Event
                SnapshotReceived(true);
            }
            catch (Exception E)
            {
                // Notify user
                Notify.Show("Error Processing Snapshot!", E.Message, AlertType.Warning);
                ASPServer.Log("ERROR: [SnapshotProcessing] " + E.Message);

                // Fire event
                SnapshotReceived(false);
            }
        }
    }
}
