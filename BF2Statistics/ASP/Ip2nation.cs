using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using BF2Statistics.Database;
using BF2Statistics.Utilities;
using BF2Statistics.Web;

namespace BF2Statistics.ASP
{
    class Ip2nation
    {
        /// <summary>
        /// The connection string to our Ip2Nation sqlite database
        /// </summary>
        protected static string ConnectionString;

        static Ip2nation()
        {
            try
            {
                // Dont attempt to create, just quit
                string file = Path.Combine(Program.RootPath, "Ip2nation.db");
                if (!File.Exists(file))
                    throw new Exception("Ip2nation.db file is missing!");

                // Create connection string
                SQLiteConnectionStringBuilder Builder = new SQLiteConnectionStringBuilder();
                Builder.DataSource = file;
                Builder.Version = 3;
                Builder.LegacyFormat = false;
                Builder.DefaultTimeout = 500;
                ConnectionString = Builder.ConnectionString;
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("WARNING: [Ip2nation..ctor] " + e.Message);
            }
        }
        
        /// <summary>
        /// Gets the country code for a string IP address
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static string GetCountryCode(IPAddress IP)
        {
            // Return default config Country Code
            if (IPAddress.IsLoopback(IP) || HttpServer.LocalIPs.Contains(IP))
                return Program.Config.ASP_LocalIpCountryCode;

            try
            {
                using (DatabaseDriver Driver = new DatabaseDriver("Sqlite", ConnectionString))
                {
                    // Fetch country code from Ip2Nation
                    Driver.Connect();
                    List<Dictionary<string, object>> Rows = Driver.Query(
                        "SELECT country FROM ip2nation WHERE ip < @P0 ORDER BY ip DESC LIMIT 1",
                        Networking.IP2Long(IP.ToString())
                    );
                    string CC = (Rows.Count == 0) ? "xx" : Rows[0]["country"].ToString();

                    // Fix country!
                    return (CC == "xx" || CC == "01") ? Program.Config.ASP_LocalIpCountryCode : CC;
                }
            }
            catch
            {
                return Program.Config.ASP_LocalIpCountryCode;
            }
        }

        /// <summary>
        /// Fethces the full country name from a country code supplied from GetCountryCode()
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static string GetCountyNameFromCode(string Code)
        {
            try
            {
                using (DatabaseDriver Driver = new DatabaseDriver("Sqlite", ConnectionString))
                {
                    // Fetch country code from Ip2Nation
                    Driver.Connect();
                    List<Dictionary<string, object>> Rows = Driver.Query(
                        "SELECT country FROM ip2nationcountries WHERE iso_code_2 = @P0", Code.ToUpperInvariant()
                    );

                    return (Rows.Count == 0) ? Code: Rows[0]["country"].ToString();
                }
            }
            catch
            {
                return Code;
            }
        }
    }
}
