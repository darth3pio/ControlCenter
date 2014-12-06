using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    class CreatePlayer
    {
        /// <summary>
        /// This request creates a player with the specified Pid when called
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <queryParam name ="nick" type="string">Unique player nickname</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public CreatePlayer(HttpClient Client, StatsDatabase Driver)
        {
            int Pid;
            ASPResponse Response = Client.Response as ASPResponse;

            // make sure we have a valid player ID
            if (!Client.Request.QueryString.ContainsKey("pid")
                || !Int32.TryParse(Client.Request.QueryString["pid"], out Pid)
                || !Client.Request.QueryString.ContainsKey("nick"))
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // Fetch Player
            string PlayerNick = Client.Request.QueryString["nick"].Replace("%20", " ");
            string CC = (Client.Request.QueryString.ContainsKey("cid")) ? Client.Request.QueryString["cid"] : "";
            var Rows = Driver.Query("SELECT name FROM player WHERE id=@P0 OR name=@P1", Pid, PlayerNick);
            if (Rows.Count > 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteFreeformLine("Player already Exists!");
                Response.Send();
                return;
            }

            // Create Player
            Driver.Execute(
                "INSERT INTO player(id, name, country, joined, isbot) VALUES(@P0, @P1, @P2, @P3, 0)",
                Pid, PlayerNick, CC, DateTime.UtcNow.ToUnixTimestamp()
            );

            // Confirm
            Response.WriteResponseStart();
            Response.WriteFreeformLine("Player Created Successfully!");
            Response.Send();
        }
    }
}
