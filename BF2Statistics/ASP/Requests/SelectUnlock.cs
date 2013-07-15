using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class SelectUnlock
    {
        public SelectUnlock(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            int Pid = 0;
            int Unlock = 0;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;
            FormattedOutput Output;

            // Setup Params
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);
            if (QueryString.ContainsKey("id"))
                Int32.TryParse(QueryString["id"], out Unlock);

            if (Pid == 0 || Unlock == 0)
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Fetch Player
            Rows = Driver.Query("SELECT availunlocks, usedunlocks FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Update Unlock
            Driver.Execute("UPDATE unlocks SET state = 's' WHERE id = @P0 AND kit = @P1", Pid, Unlock);

            // Subtract 1 unlock
            Driver.Execute("UPDATE player SET availunlocks = @P0, usedunlocks = @P1 WHERE id = @P2", 
                int.Parse(Rows[0]["availunlocks"].ToString()) - 1,
                int.Parse(Rows[0]["usedunlocks"].ToString()) + 1,
                Pid
            );

            // Send Response
            Output = new FormattedOutput("response");
            Output.AddRow("OK");
            Response.AddData(Output);
            Response.Send();
        }
    }
}
