using System;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/createplayer.aspx
    /// </summary>
    /// <queryParam name="pid" type="int">The unique player ID</queryParam>
    /// <queryParam name ="nick" type="string">Unique player nickname</queryParam>
    public sealed class CreatePlayer : ASPController
    {
        /// <summary>
        /// This request creates a player with the specified Pid when called
        /// </summary>
        /// <param name="Client">The HttpClient who made the request</param>
        public CreatePlayer(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            int Pid;
            
            // make sure we have a valid player ID
            if (!Request.QueryString.ContainsKey("pid")
                || !Int32.TryParse(Request.QueryString["pid"], out Pid)
                || !Request.QueryString.ContainsKey("nick"))
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
                string PlayerNick = Request.QueryString["nick"].Replace("%20", " ");
                string CC = (Request.QueryString.ContainsKey("cid")) ? Client.Request.QueryString["cid"] : "";
                if (Database.ExecuteScalar<int>("SELECT COUNT(*) FROM player WHERE id=@P0 OR name=@P1", Pid, PlayerNick) > 0)
                {
                    Response.WriteResponseStart(false);
                    Response.WriteFreeformLine("Player already Exists!");
                    Response.Send();
                    return;
                }

                // Create Player
                Database.Execute(
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
}
