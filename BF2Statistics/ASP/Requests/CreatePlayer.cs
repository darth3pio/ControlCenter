using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class CreatePlayer
    {
        public CreatePlayer(HttpClient Client, StatsDatabase Driver)
        {
            int Pid;

            // make sure we have a valid player ID
            if (!Client.Request.QueryString.ContainsKey("pid")
                || !Int32.TryParse(Client.Request.QueryString["pid"], out Pid)
                || !Client.Request.QueryString.ContainsKey("nick"))
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Client.Response.Send();
                return;
            }

            // Fetch Player
            string PlayerNick = Client.Request.QueryString["nick"].Replace("%20", " ");
            string CC = (Client.Request.QueryString.ContainsKey("cid")) ? Client.Request.QueryString["cid"] : "";
            var Rows = Driver.Query("SELECT name FROM player WHERE id=@P0 OR name=@P1", Pid, PlayerNick);
            if (Rows.Count > 0)
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteFreeformLine("Player already Exists!");
                Client.Response.Send();
                return;
            }

            // Create Player
            Driver.Execute(
                "INSERT INTO player(id, name, country, joined, isbot) VALUES(@P0, @P1, @P2, @P3, 0)",
                Pid, PlayerNick, CC, DateTime.UtcNow.ToUnixTimestamp()
            );

            // Confirm
            Client.Response.WriteResponseStart();
            Client.Response.WriteFreeformLine("Player Created Successfully!");
            Client.Response.Send();
        }
    }
}
