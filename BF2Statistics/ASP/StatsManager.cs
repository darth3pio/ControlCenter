using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using BF2Statistics.Web.ASP;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.ASP
{
    class StatsManager
    {
        /// <summary>
        /// Returns whether the specified URI is a valid, and available ASP Service
        /// </summary>
        /// <param name="Url">The root url to the ASP server. Dont include the /ASP/ path!</param>
        public static void CheckASPService(string Url)
        {
            // Create the ASP request, and fetch the http response
            WebRequest Request = WebRequest.Create(new Uri(Url.TrimEnd('/') + "/ASP/getbackendinfo.aspx"));
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();

            // Make sure that we connected successfully
            if (Response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Unable to connect to the Gamespy ASP Webservice: " + Response.StatusDescription);

            // Parse the response message
            using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
            {
                string Lines = Reader.ReadToEnd().TrimStart();

                // Does the player exist?
                if (!Lines.StartsWith("O"))
                    throw new Exception("The ASP webserver didnt not respond with a proper ASP response!");
            }
        }


        /// <summary>
        /// Exports a players stats and history into an Xml file
        /// </summary>
        /// <param name="XmlPath">The folder path to where the XML will be saved</param>
        /// <param name="Pid">Player ID</param>
        /// <param name="Name">Player Name</param>
        public static void ExportPlayerXml(string XmlPath, int Pid, string Name)
        {
            //  Create full path
            string sPath = Path.Combine(
                XmlPath,
                String.Format("{0}_{1}_{2}.xml", Name.Trim().MakeFileNameSafe(), Pid, DateTime.Now.ToString("yyyyMMdd_HHmm"))
            );

            // Delete file if it exists already
            if (File.Exists(sPath))
                File.Delete(sPath);

            // Create XML Settings
            XmlWriterSettings Settings = new XmlWriterSettings();
            Settings.Indent = true;
            Settings.IndentChars = "\t";
            Settings.NewLineChars = Environment.NewLine;
            Settings.NewLineHandling = NewLineHandling.Replace;

            // Write XML data
            using (StatsDatabase Driver = new StatsDatabase())
            using (XmlWriter Writer = XmlWriter.Create(sPath, Settings))
            {
                // Player Element
                Writer.WriteStartDocument();
                Writer.WriteStartElement("Player");

                // Manifest
                Writer.WriteStartElement("Info");
                Writer.WriteElementString("Pid", Pid.ToString());
                Writer.WriteElementString("Name", Name.EscapeXML());
                Writer.WriteElementString("BackupDate", DateTime.Now.ToString());
                Writer.WriteEndElement();

                // Start Tables Element
                Writer.WriteStartElement("TableData");

                // Add each tables data
                foreach (string Table in StatsDatabase.PlayerTables)
                {
                    // Open table tag
                    Writer.WriteStartElement(Table);

                    // Fetch row
                    List<Dictionary<string, object>> Rows;
                    if (Table == "kills")
                        Rows = Driver.Query(String.Format("SELECT * FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid));
                    else
                        Rows = Driver.Query(String.Format("SELECT * FROM {0} WHERE id={1}", Table, Pid));

                    // Write each row's columns with its value to the xml file
                    foreach (Dictionary<string, object> Row in Rows)
                    {
                        // Open Row tag
                        Writer.WriteStartElement("Row");
                        foreach (KeyValuePair<string, object> Column in Row)
                        {
                            if (Column.Key == "name")
                                Writer.WriteElementString(Column.Key, Column.Value.ToString().EscapeXML());
                            else
                                Writer.WriteElementString(Column.Key, Column.Value.ToString());
                        }

                        // Close Row tag
                        Writer.WriteEndElement();
                    }

                    // Close table tag
                    Writer.WriteEndElement();
                }

                // Close Tags and File
                Writer.WriteEndElement();  // Close Tables Element
                Writer.WriteEndElement();  // Close Player Element
                Writer.WriteEndDocument(); // End and Save file
            }
        }

        /// <summary>
        /// Imports a Player XML Sheet from the specified path
        /// </summary>
        /// <param name="XmlPath">The full path to the XML file</param>
        public static void ImportPlayerXml(string XmlPath)
        {
            // Connect to database first!
            using (StatsDatabase Driver = new StatsDatabase())
            {
                // Load elements
                XDocument Doc = XDocument.Load(new FileStream(XmlPath, FileMode.Open, FileAccess.Read));
                XElement Info = Doc.Root.Element("Info");
                XElement TableData = Doc.Root.Element("TableData");

                // Make sure player doesnt already exist
                int Pid = Int32.Parse(Info.Element("Pid").Value);
                if (Driver.PlayerExists(Pid))
                    throw new Exception(String.Format("Player with PID {0} already exists!", Pid));

                // Begin Transaction
                using (DbTransaction Transaction = Driver.BeginTransaction())
                {
                    try
                    {
                        // Loop through tables
                        foreach (XElement Table in TableData.Elements())
                        {
                            // Loop through Rows
                            foreach (XElement Row in Table.Elements())
                            {
                                InsertQueryBuilder QueryBuilder = new InsertQueryBuilder(Table.Name.LocalName, Driver);
                                foreach (XElement Col in Row.Elements())
                                {
                                    if (Col.Name.LocalName == "name")
                                        QueryBuilder.SetField(Col.Name.LocalName, Col.Value.UnescapeXML());
                                    else
                                        QueryBuilder.SetField(Col.Name.LocalName, Col.Value);
                                }

                                QueryBuilder.Execute();
                            }
                        }

                        // Commit Transaction
                        Transaction.Commit();
                    }
                    catch
                    {
                        Transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
