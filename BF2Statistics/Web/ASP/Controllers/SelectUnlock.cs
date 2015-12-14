using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/selectunlock.aspx
    /// </summary>
    public sealed class SelectUnlock : ASPController
    {
        /// <summary>
        /// This request sets the unlock details for a player, when he picks an unlock
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <queryParam name="id" type="int">The unique unlock ID</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        public SelectUnlock(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            int Pid = 0;
            int Unlock = 0;
            List<Dictionary<string, object>> Rows;

            // Setup Params
            if (Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Request.QueryString["pid"], out Pid);
            if (Request.QueryString.ContainsKey("id"))
                Int32.TryParse(Request.QueryString["id"], out Unlock);

            // Make sure we have valid parameters
            if (Pid == 0 || Unlock == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Fetch Player
                Rows = Database.Query("SELECT availunlocks, usedunlocks FROM player WHERE id=@P0", Pid);
                if (Rows.Count == 0)
                {
                    Response.WriteResponseStart(false);
                    Response.WriteHeaderLine("asof", "err");
                    Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                    Response.Send();
                    return;
                }

                // Update Unlock, setting its state to 's' (unlocked)
                Database.Execute("UPDATE unlocks SET state = 's' WHERE id = @P0 AND kit = @P1", Pid, Unlock);

                // Update players used and avail unlock counts
                Database.Execute("UPDATE player SET availunlocks = @P0, usedunlocks = @P1 WHERE id = @P2",
                    int.Parse(Rows[0]["availunlocks"].ToString()) - 1,
                    int.Parse(Rows[0]["usedunlocks"].ToString()) + 1,
                    Pid
                );

                // Send Response
                Response.WriteResponseStart();
                Response.WriteHeaderLine("response");
                Response.WriteDataLine("OK");
                Response.Send();
            }
        }
    }
}
