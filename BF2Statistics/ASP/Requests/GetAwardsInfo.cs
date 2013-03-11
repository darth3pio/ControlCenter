using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.ASP.Requests
{
    class GetAwardsInfo
    {
        public GetAwardsInfo(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            FormattedOutput Output;
            int Pid;

            // make sure we have a valid player ID
            if (!QueryString.ContainsKey("pid") || !Int32.TryParse(QueryString["pid"], out Pid))
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Output header data
            Output = new FormattedOutput("pid", "asof");
            Output.AddRow(Pid, Utils.UnixTimestamp());
            Response.AddData(Output);

            // Create Award List Header
            Output = new FormattedOutput("award", "level", "when", "first");
            List<Dictionary<string, object>> Awards = new List<Dictionary<string,object>>();

            try
            {
                Awards = ASPServer.Database.GetPlayerAwards(Pid);
            }
            catch { }

            foreach (Dictionary<string, object> Award in Awards)
            {
                Output.AddRow(
                    Award["awd"].ToString(), 
                    Award["level"].ToString(), 
                    Award["earned"].ToString(), 
                    Award["first"].ToString()
                );
            }

            Response.AddData(Output);
            Response.Send();
        }
    }
}
