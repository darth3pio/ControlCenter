using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BF2Statistics.Web.Bf2Stats
{
    /// <summary>
    /// This class loads and contains all of the data found in the Web/Bf2Stats/.xml files
    /// </summary>
    public static class Bf2StatsData
    {
        #region Static Variables

        /// <summary>
        /// ArmyId => Army Name. Data generated from ArmyData.xml
        /// </summary>
        public static Dictionary<int, string> Armies;

        /// <summary>
        /// MapId => Map Name. Data generated from MapData.xml
        /// </summary>
        public static Dictionary<int, string> Maps;

        /// <summary>
        /// ModName => List of MapIds. Data generated from MapData.xml
        /// </summary>
        public static Dictionary<string, List<int>> ModMapIds;

        /// <summary>
        /// TheaterName => List of MapIds. Data generated from TeaterData.xml
        /// </summary>
        public static Dictionary<string, int[]> TheatreMapIds;

        #endregion Static Variables

        /// <summary>
        /// Loads the XML files from the "Web/Bf2Stats" folder.
        /// These XML files are used for displaying certain information
        /// on some of the Bf2Stats pages
        /// </summary>
        public static void Load()
        {
            try
            {
                // Reset all incase of file changes
                Armies = new Dictionary<int, string>();
                Maps = new Dictionary<int, string>();
                ModMapIds = new Dictionary<string, List<int>>()
                {
                    { "bf", new List<int>() },
                    { "sf", new List<int>() },
                    { "ef", new List<int>() },
                    { "af", new List<int>() }
                };
                TheatreMapIds = new Dictionary<string, int[]>();

                // Enumerate through each .XML file in the data directory
                string DataDir = Path.Combine(Program.RootPath, "Web", "Bf2Stats");
                FileStream file;
                XmlDocument Doc = new XmlDocument();

                // Load Army Data, Creating file if it doesnt exist already
                using (file = new FileStream(Path.Combine(DataDir, "ArmyData.xml"), FileMode.OpenOrCreate))
                {
                    if (file.Length == 0)
                    {
                        using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8, 1024, true))
                            writer.Write(Program.GetResourceAsString("BF2Statistics.Web.Bf2Stats.ArmyData.xml"));

                        file.Flush();
                        file.Seek(0, SeekOrigin.Begin);
                    }

                    // Load the xml data
                    Doc.Load(file);
                    foreach (XmlNode Node in Doc.GetElementsByTagName("army"))
                    {
                        Armies.Add(Int32.Parse(Node.Attributes["id"].Value), Node.InnerText);
                    }
                }

                // Load Map Data, Creating file if it doesnt exist already
                using (file = new FileStream(Path.Combine(DataDir, "MapData.xml"), FileMode.OpenOrCreate))
                {
                    if (file.Length == 0)
                    {
                        using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8, 1024, true))
                            writer.Write(Program.GetResourceAsString("BF2Statistics.Web.Bf2Stats.MapData.xml"));

                        file.Flush();
                        file.Seek(0, SeekOrigin.Begin);
                    }

                    // Load the xml data
                    Doc.Load(file);
                    foreach (XmlNode Node in Doc.GetElementsByTagName("map"))
                    {
                        int mid = Int32.Parse(Node.Attributes["id"].Value);
                        string mod = Node.Attributes["mod"].Value;
                        Maps[mid] = Node.InnerText;

                        // Add map to mod map ids if mod is not empty
                        if (!String.IsNullOrWhiteSpace(mod) && ModMapIds.ContainsKey(mod))
                            ModMapIds[mod].Add(mid);
                    }
                }

                // Load Theater Data, Creating file if it doesnt exist already
                using (file = new FileStream(Path.Combine(DataDir, "TheaterData.xml"), FileMode.OpenOrCreate))
                {
                    if (file.Length == 0)
                    {
                        using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8, 1024, true))
                            writer.Write(Program.GetResourceAsString("BF2Statistics.Web.Bf2Stats.TheaterData.xml"));

                        file.Flush();
                        file.Seek(0, SeekOrigin.Begin);
                    }

                    // Load the xml data
                    Doc.Load(file);
                    foreach (XmlNode Node in Doc.GetElementsByTagName("theater"))
                    {
                        string name = Node.Attributes["name"].Value;
                        string[] arr = Node.Attributes["maps"].Value.Split(',');
                        TheatreMapIds.Add(name, Array.ConvertAll(arr, Int32.Parse));
                    }
                }

                // Load Rank Data
                Rank[] Ranks = new Rank[22];
                int i = 0;

                // Load Rank Data, Creating file if it doesnt exist already
                using (file = new FileStream(Path.Combine(DataDir, "RankData.xml"), FileMode.OpenOrCreate))
                {
                    if (file.Length == 0)
                    {
                        using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8, 1024, true))
                            writer.Write(Program.GetResourceAsString("BF2Statistics.Web.Bf2Stats.RankData.xml"));

                        file.Flush();
                        file.Seek(0, SeekOrigin.Begin);
                    }

                    // Load the xml data
                    Doc.Load(file);
                    foreach (XmlNode Node in Doc.GetElementsByTagName("rank"))
                    {
                        Dictionary<string, int> Awards = new Dictionary<string, int>();
                        XmlNode AwardsNode = Node.SelectSingleNode("reqAwards");
                        if (AwardsNode != null && AwardsNode.HasChildNodes)
                        {
                            foreach (XmlNode E in AwardsNode.ChildNodes)
                            {
                                Awards.Add(E.Attributes["id"].Value, Int32.Parse(E.Attributes["level"].Value));
                            }
                        }
                        string[] arr = Node.SelectSingleNode("reqRank").InnerText.Split(',');

                        Ranks[i] = new Rank
                        {
                            Id = i,
                            MinPoints = Int32.Parse(Node.SelectSingleNode("reqPoints").InnerText),
                            ReqRank = Array.ConvertAll(arr, Int32.Parse),
                            ReqAwards = Awards
                        };
                        i++;
                    }

                    RankCalculator.SetRankData(Ranks);
                }
            }
            catch(Exception e)
            {
                ExceptionHandler.GenerateExceptionLog(e);
                throw;
            }
        }

        /// <summary>
        /// Returns the army title based on the given raarmy id
        /// </summary>
        /// <param name="ArmyId"></param>
        /// <returns></returns>
        public static string GetArmyName(int ArmyId)
        {
            if (ArmyId >= Armies.Count)
                return "Unknown";

            return Armies[ArmyId];
        }

        /// <summary>
        /// Returns the map title based on the given map id
        /// </summary>
        /// <param name="MapId"></param>
        /// <returns></returns>
        public static string GetMapName(int MapId)
        {
            if (!Maps.ContainsKey(MapId))
                return "Unknown";

            return Maps[MapId];
        }
    }
}
