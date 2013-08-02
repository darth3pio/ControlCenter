using System;
using System.Collections.Generic;
using BF2Statistics.Database;
using System.Data.Common;

namespace BF2Statistics.ASP.Requests
{
    class SelectUnlock
    {
        public SelectUnlock(HttpClient Client)
        {
            int Pid = 0;
            int Unlock = 0;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;

            // Setup Params
            if (Client.Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Client.Request.QueryString["pid"], out Pid);
            if (Client.Request.QueryString.ContainsKey("id"))
                Int32.TryParse(Client.Request.QueryString["id"], out Unlock);

            // Make sure we have valid parameters
            if (Pid == 0 || Unlock == 0)
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Client.Response.Send();
                return;
            }

            // Fetch Player
            Rows = Driver.Query("SELECT availunlocks, usedunlocks FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Client.Response.Send();
                return;
            }

            // Start a new Transaction
            DbTransaction Transaction = Driver.BeginTransaction();

            // Update Unlock
            Driver.Execute("UPDATE unlocks SET state = 's' WHERE id = @P0 AND kit = @P1", Pid, Unlock);

            // Subtract 1 unlock
            Driver.Execute("UPDATE player SET availunlocks = @P0, usedunlocks = @P1 WHERE id = @P2", 
                int.Parse(Rows[0]["availunlocks"].ToString()) - 1,
                int.Parse(Rows[0]["usedunlocks"].ToString()) + 1,
                Pid
            );

            // Commits the Transaction
            Transaction.Commit();

            // Send Response
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("response");
            Client.Response.WriteDataLine("OK");
            Client.Response.Send();
        }
    }
}
