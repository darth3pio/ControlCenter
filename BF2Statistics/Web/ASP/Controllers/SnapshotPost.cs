using System;
using System.IO;
using System.Net;
using BF2Statistics.ASP;
using BF2Statistics.ASP.StatsProcessor;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/bf2statistics.php
    /// </summary>
    public sealed class SnapshotPost : ASPController
    {
        /// <summary>
        /// This request takes snapshot data, and processes it into the stats database
        /// </summary>
        /// <param name="Client">The HttpClient who made the request</param>
        public SnapshotPost(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // First and foremost. Make sure that we are authorized to be here!
            IPEndPoint RemoteIP = Client.RemoteEndPoint;
            ASPResponse Response = Client.Response as ASPResponse;

            // Make sure we have post data
            if (!Client.Request.HasEntityBody)
            {
                // No Post Data
                if (Client.Request.UserAgent == "GameSpyHTTP/1.0")
                {
                    Response.WriteResponseStart(false);
                    Response.WriteHeaderLine("response");
                    Response.WriteDataLine("SNAPSHOT Data NOT found!");
                }
                else
                    Client.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                Response.Send();
                return;
            }

            // Create snapshot backup file if the snapshot is valid
            try
            {
                // Read Snapshot
                using (StreamReader Reader = new StreamReader(Client.Request.InputStream))
                {
                    // Attempt to Queue the snapshot in the Processor factory
                    string SnapshotData = Reader.ReadToEnd();
                    StatsManager.QueueServerSnapshot(SnapshotData, RemoteIP.Address);
                }

                // Tell the server we are good to go
                Response.WriteResponseStart();
                Response.WriteHeaderLine("response");
                Response.WriteDataLine("OK");
                Response.Send();
            }
            catch (UnauthorizedAccessException)
            {
                // Notify User
                Notify.Show("Snapshot Denied!", "Invalid Server IP: " + RemoteIP.Address.ToString(), AlertType.Warning);
                if (Client.Request.UserAgent == "GameSpyHTTP/1.0")
                {
                    Response.WriteResponseStart(false);
                    Response.WriteHeaderLine("response");
                    Response.WriteDataLine("Unauthorised Gameserver");
                }
                else
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;

                Response.Send();
                return;
            }
            catch (InvalidDataException E)
            {
                // Generate Exception Log
                HttpServer.AspStatsLog.Write("ERROR: [SnapshotPreProcess] " + E.Message);
                ExceptionHandler.GenerateExceptionLog(E);

                // Notify the user and the connection client
                Notify.Show("Error Processing Snapshot!", "Snapshot Data NOT Complete or Invalid!", AlertType.Warning);
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("response");
                Response.WriteDataLine("SNAPSHOT Data NOT complete or invalid!");
                Response.Send();
                return;
            }
            catch (Exception E)
            {
                HttpServer.AspStatsLog.Write("ERROR: [SnapshotPreProcess] " + E.Message + " @ " + E.TargetSite);
                Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                Response.Send();
                return;
            }
        }
    }
}
