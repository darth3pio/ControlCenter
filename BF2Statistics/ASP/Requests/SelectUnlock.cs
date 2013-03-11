using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Fetch Player
            Rows = Driver.Query("SELECT availunlocks, usedunlocks FROM player WHERE id={0}", Pid);
            if (Rows.Count == 0)
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Player Doesnt Exist!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Update Unlock
            Driver.Execute("UPDATE unlocks SET state = 's' WHERE id = {0} AND kit = {1}", Pid, Unlock);

            // Subtract 1 unlock
            Driver.Execute("UPDATE player SET availunlocks = {0}, usedunlocks = {1} WHERE id = {2}", 
                int.Parse(Rows[0]["availunlocks"].ToString()) - 1,
                int.Parse(Rows[0]["usedunlocks"].ToString()) + 1,
                Pid
            );
        }
    }
}
