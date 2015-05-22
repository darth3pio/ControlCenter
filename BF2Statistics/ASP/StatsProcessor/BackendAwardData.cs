using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace BF2Statistics.ASP.StatsProcessor
{
    class BackendAwardData
    {
        public static readonly Dictionary<string, int> Awards = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            // Badges
            {"kcb", 1031406},   // Knife Combat Badge
            {"pcb", 1031619},   // Pistol Combat Badge
            {"Acb", 1031119},   // Assault Combat Badge
            {"Atcb", 1031120},  // Assault Combat Badge
            {"Sncb", 1031109},  // Sniper Combat Badge
            {"Socb", 1031115},  // Special Ops Combat Badge
            {"Sucb", 1031121},  // Support Combat Badge
            {"Ecb", 1031105},   // Engineer Combat Badge
            {"Mcb", 1031113},   // Medic Combat Badge
            {"Eob", 1032415},   // Explosive Ordinance Badge
            {"Fab", 1190601},   // First Aid Badge
            {"Eb", 1190507},    // Engineer Repair Badge
            {"Rb", 1191819},    // Resupply Badge
            {"Cb", 1190304},    // Command Badge
            {"Ab", 1220118},    // Armor Badge
            {"Tb", 1222016},    // Transport Badge
            {"Hb", 1220803},    // Helicopter Badge
            {"Avb", 1220122},   // Aviators Badge
            {"adb", 1220104},   // Air Defense Badge
            {"Swb", 1031923},   // Ground Defense Badge

            // Xpack Badges
            {"X1Acb", 1261119},     // Assault Specialist
            {"X1Atcb", 1261120},    // AntiTank Specialist
            {"X1Sncb", 1261109},    // Sniper Specialist
            {"X1Socb", 1261115},    // Special Ops Specialist
            {"X1Sucb", 1261121},    // Support Specialist
            {"X1Ecb", 1261105},     // Engineer Specialist
            {"X1Mcb", 1261113},     // Medical Specialist
            {"X1fbb", 1260602},     // Tactical Support Specialist
            {"X1ghb", 1260708},     // Grappling Hook Specialist
            {"X1zlb", 1262612},     // Zipline Specialist

            // Medals
            {"ph", 2191608},    // Purple Heart
            {"Msm", 2191319},   // Meritorious Service Medal
            {"Cam", 2190303},   // Combat Action Medal
            {"Acm", 2190309},   // Aviator Combat Medal
            {"Arm", 2190318},   // Armored Combat Medal
            {"Hcm", 2190308},   // Helicopter Combat Medal
            {"gcm", 2190703},   // Good Conduct Medal
            {"Cim", 2020903},   // Combat Infantry Medal
            {"Mim", 2020913},   // Marksman Infantry Medal
            {"Sim", 2020919},   // Sharpshooter Infantry Medal
            {"Mvn", 2021322},   // Medal of Valor
            {"Dsm", 2020419},   // Distinguished Service Medal
            {"pmm", 2021613},   // Peoples Medallion

            // Round Medals
            {"erg", 2051907},    // End of Round Gold
            {"ers", 2051919},    // End of Round Silver
            {"erb", 2051902},    // End of Round Bronze

            // Ribbons
            {"Car", 3240301},   // Combat Action Ribbon
            {"Mur", 3211305},   // Meritorious Unit Ribbon
            {"Ior", 3150914},   // Infantry Officer Ribbon
            {"Sor", 3151920},   // Staff Officer Ribbon
            {"Dsr", 3190409},   // Distingusihed Service Ribbon
            {"Wcr", 3242303},   // War College Ribbon
            {"Vur", 3212201},   // Valorous Unit Ribbon
            {"Lmr", 3241213},   // Legion Of Merrit
            {"Csr", 3190318},   // Crew Service Ribbon
            {"Arr", 3190118},   // Armored Ribbon
            {"Aer", 3190105},   // Aviator Ribbon
            {"Hsr", 3190803},   // Helicopter Service Ribbon
            {"Adr", 3040109},   // Airdefense Service Ribbon
            {"Gdr", 3040718},   // Ground Defense Service Ribbon
            {"Ar", 3240102},    // Airborne Ribbon
            {"gcr", 3240703},   // Good Conduct Ribbon

            // Xpack Ribbons
            {"X1Csr", 3260318},     // Crew Service Ribbon
            {"X1Arr", 3260118},     // Armored Service
            {"X1Aer", 3260105},     // Ariel Service
            {"X1Hsr", 3260803},     // Helo Specialist
        };

        /// <summary>
        /// The list of Backend Awards and their criteria's
        /// </summary>
        public static List<BackendAward> BackendAwards = new List<BackendAward>();

        /// <summary>
        /// Builds the Backend award data for the snapshot processor to use
        /// </summary>
        public static void BuildAwardData()
        {
            // Clear out old data
            BackendAwards.Clear();

            // Enumerate through each .XML file in the data directory
            string xmlFile = Path.Combine(Paths.DocumentsFolder, "BackendAwards.xml");
            XmlDocument Doc = new XmlDocument();

            // Load Army Data, Creating file if it doesnt exist already
            using (FileStream file = new FileStream(xmlFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                if (file.Length == 0)
                {
                    using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8, 1024, true))
                        writer.Write(Utils.GetResourceAsString("BF2Statistics.ASP.BackendAwards.xml"));

                    file.Flush();
                    file.Seek(0, SeekOrigin.Begin);
                }

                // Load the xml data
                Doc.Load(file);
                foreach (XmlNode Node in Doc.GetElementsByTagName("award"))
                {
                    // Never know what the user is capable of!
                    if (!Node.HasChildNodes) continue;

                    // Fetch child nodes into a list of conditions
                    List<AwardCriteria> criterias = new List<AwardCriteria>();
                    foreach(XmlNode cNode in Node.ChildNodes)
                    {
                        // Make sure its a valid tag
                        if (cNode.Name != "query") continue;

                        // Add the criteria query
                        criterias.Add(new AwardCriteria(
                            cNode.Attributes["table"].Value,
                            cNode.Attributes["select"].Value, 
                            Int32.Parse(cNode.Attributes["result"].Value, NumberFormatInfo.InvariantInfo),
                            cNode.Attributes["where"].Value
                        ));
                    }

                    // Add award
                    BackendAwards.Add(new BackendAward(
                        Int32.Parse(Node.Attributes["id"].Value, NumberFormatInfo.InvariantInfo), 
                        criterias.ToArray()
                    ));
                }
            }
        }
    }
}
