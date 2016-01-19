using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace BF2Statistics
{
    public partial class ScoreSettings : Form
    {
        // File Infos
        private FileInfo ScoringCommonFile;
        private FileInfo ScoringConqFile;
        private FileInfo ScoringCoopFile;

        /// <summary>
        /// The score settings from the scoring common
        /// </summary>
        protected Dictionary<string, string[]> Scores;

        /// <summary>
        /// The score settings from the conquest file
        /// </summary>
        protected Dictionary<string, string[]> ConqScores;

        /// <summary>
        /// score settings from the coop file
        /// </summary>
        protected Dictionary<string, string[]> CoopScores;

        /// <summary>
        /// Our Regex object that will parse the current score settings
        /// </summary>
        private Regex Reg = new Regex(
            @"^(?<varname>[A-Z_]+)(?:[\s|\t]*)=(?:[\s|\t]*)(?<value>[-]?[0-9]+)(?:.*)$", 
            RegexOptions.Multiline
        );

        /// <summary>
        /// Constructor
        /// </summary>
        public ScoreSettings()
        {
            InitializeComponent();
            this.Text = MainForm.SelectedMod.Title + " Score Settings";

            // Assign folder vars
            string ScoringFolder = Path.Combine(MainForm.SelectedMod.RootPath, "python", "game");
            ScoringCommonFile = new FileInfo(Path.Combine(ScoringFolder, "scoringCommon.py"));
            ScoringConqFile = new FileInfo(Path.Combine(ScoringFolder, "gamemodes", "gpm_cq.py"));
            ScoringCoopFile = new FileInfo(Path.Combine(ScoringFolder, "gamemodes", "gpm_coop.py"));

            // Make sure the files all exist
            if (!ScoringCommonFile.Exists|| !ScoringConqFile.Exists || !ScoringCoopFile.Exists)
            {
                MessageBox.Show("One or more scoring files are missing. Unable to modify scoring.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
                return;
            }
                
            // Load Common Scoring File
            if (!LoadScoringCommon())
                return;

            // Load the Coop Scoring file
            if (!LoadCoopFile())
                return;

            // Load the Conquest Scoring file
            if (!LoadConqFile())
                return;

            // Fill form values
            FillFormFields();
        }

        /// <summary>
        /// Loads the scoring files, and initializes the values of all the input fields
        /// </summary>
        private bool LoadScoringCommon()
        {
            // First, we need to parse all 3 scoring files
            string file;
            string ModPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_scoringCommon.py");
            string DefaultPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", "bf2_scoringCommon.py");

            // Scoring Common. Check for Read and Write access
            try
            {
                using (Stream Str = ScoringCommonFile.Open(FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader Rdr = new StreamReader(Str))
                    file = Rdr.ReadToEnd();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Unable to Read/Write to the Common scoring file:" + Environment.NewLine
                    + Environment.NewLine + "File: " + ScoringCommonFile.FullName
                    + Environment.NewLine + "Error: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                this.Load += (s, ev) => this.Close();
                return false;
            }

            // First, we are going to check for a certain string... if it exists
            // Then these config file has been reformated already, else we need
            // to reformat it now
            if (!file.StartsWith("# BF2Statistics Formatted Common Scoring"))
            {
                if (!File.Exists(ModPath))
                {
                    // Show warn dialog
                    if (MessageBox.Show(
                        "The scoringCommon.py file needs to be formatted to use this feature. If you are using a third party mod,"
                        + " then formatting can break the scoring. Do you want to Format now?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += (s, e) => this.Close();
                        return false;
                    }

                    file = File.ReadAllText(DefaultPath);
                }
                else
                {
                    // Show warn dialog
                    if (MessageBox.Show("The scoringCommon.py file needs to be formatted to use this feature."
                        + " Do you want to Format now?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += (s, e) => this.Close();
                        return false;
                    }

                    file = File.ReadAllText(ModPath);
                }   

                // Write formated data to the common soring file
                using (Stream Str = ScoringCommonFile.Open(FileMode.Truncate, FileAccess.Write))
                using (StreamWriter Wtr = new StreamWriter(Str))
                {
                    Wtr.Write(file);
                    Wtr.Flush();
                }
            }

            // Build our regex for getting scoring values
            Scores = new Dictionary<string, string[]>();

            // Get all matches for the ScoringCommon.py
            MatchCollection Matches = Reg.Matches(file);
            foreach (Match m in Matches)
                Scores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, @"^" + m.Value + "(?:.*)$" });

            return true;
        }

        /// <summary>
        /// Loads the conquest scoring file
        /// </summary>
        /// <remarks>We do a search and replace for the word DEFENT because the AIX devs spelled DEFEND incorrectly!</remarks>
        private bool LoadConqFile()
        {
            string file;
            try
            {
                using (Stream Str = ScoringConqFile.Open(FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader Rdr = new StreamReader(Str))
                    file = Rdr.ReadToEnd().Replace("SCORE_DEFENT", "SCORE_DEFEND");
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Unable to Read/Write to the Conquest scoring file:" + Environment.NewLine
                    + Environment.NewLine + "File: " + ScoringConqFile.FullName
                    + Environment.NewLine + "Error: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                this.Load += (s, ev) => this.Close();
                return false;
            }

            // Process
            if (!file.StartsWith("# BF2Statistics Formatted Conq Scoring"))
            {
                // We need to replace the default file with the embedded one that
                // Correctly formats the AI_ Scores
                string DefaultPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", "bf2_cq.py");
                string ModPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_cq.py");

                file = (!File.Exists(ModPath)) ? File.ReadAllText(DefaultPath) : File.ReadAllText(ModPath);

                // Write formated data to the common soring file
                using (Stream Str = ScoringConqFile.Open(FileMode.Truncate, FileAccess.Write))
                using (StreamWriter Wtr = new StreamWriter(Str))
                {
                    Wtr.Write(file);
                    Wtr.Flush();
                }
            }

            ConqScores = new Dictionary<string, string[]>();
            MatchCollection Matches = Reg.Matches(file);
            foreach (Match m in Matches)
                ConqScores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, @"^" + m.Value + "(?:.*)$" });

            return true;
        }

        /// <summary>
        /// Loads the Coop File Settings
        /// </summary>
        private bool LoadCoopFile()
        {
            string file;
            try
            {
                using (Stream Str = ScoringCoopFile.Open(FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader Rdr = new StreamReader(Str))
                    file = Rdr.ReadToEnd().Replace("SCORE_DEFENT", "SCORE_DEFEND");
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Unable to Read/Write to the Coop scoring file:" + Environment.NewLine
                    + Environment.NewLine + "File: " + ScoringCoopFile.FullName
                    + Environment.NewLine + "Error: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                this.Load += (s, ev) => this.Close();
                return false;
            }

            // Process
            if (!file.StartsWith("# BF2Statistics Formatted Coop Scoring"))
            {
                // We need to replace the default file with the embedded one that
                // Correctly formats the AI_ Scores
                string DefaultPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", "bf2_coop.py");
                string ModPath = Path.Combine(Program.RootPath, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_coop.py");

                if (!File.Exists(ModPath))
                {
                    // Show warn dialog
                    if (MessageBox.Show(
                        "The Coop Scoring file needs to be formatted to use this feature. If you are using a third party mod,"
                        + " then formatting can break the scoring. Do you want to Format now?",
                        "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += (s, e) => this.Close();
                        return false;
                    }

                    file = File.ReadAllText(DefaultPath);
                }
                else
                {
                    // Show warn dialog
                    if (MessageBox.Show("The Coop Scoring file needs to be formatted to use this feature."
                        + " Do you want to Format now?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += (s, e) => this.Close();
                        return false;
                    }

                    file = File.ReadAllText(ModPath);
                }

                // Write formated data to the common soring file
                using (Stream Str = ScoringCoopFile.Open(FileMode.Truncate, FileAccess.Write))
                using (StreamWriter Wtr = new StreamWriter(Str))
                {
                    Wtr.Write(file);
                    Wtr.Flush();
                }
            }

            CoopScores = new Dictionary<string, string[]>();
            MatchCollection Matches = Reg.Matches(file);
            foreach (Match m in Matches)
                CoopScores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, @"^" + m.Value + "(?:.*)$" });

            return true;
        }

        /// <summary>
        /// Fills the form values
        /// </summary>
        private void FillFormFields()
        {
            // Player Global Scoring
            KillScore.Value = Int32.Parse(Scores["SCORE_KILL"][0]);
            TeamKillScore.Value = Int32.Parse(Scores["SCORE_TEAMKILL"][0]);
            SuicideScore.Value = Int32.Parse(Scores["SCORE_SUICIDE"][0]);
            ReviveScore.Value = Int32.Parse(Scores["SCORE_REVIVE"][0]);
            TeamDamage.Value = Int32.Parse(Scores["SCORE_TEAMDAMAGE"][0]);
            TeamVehicleDamage.Value = Int32.Parse(Scores["SCORE_TEAMVEHICLEDAMAGE"][0]);
            DestroyEnemyAsset.Value = Int32.Parse(Scores["SCORE_DESTROYREMOTECONTROLLED"][0]);
            DriverKA.Value = Int32.Parse(Scores["SCORE_KILLASSIST_DRIVER"][0]);
            PassangerKA.Value = Int32.Parse(Scores["SCORE_KILLASSIST_PASSENGER"][0]);
            TargeterKA.Value = Int32.Parse(Scores["SCORE_KILLASSIST_TARGETER"][0]);
            DamageAssist.Value = Int32.Parse(Scores["SCORE_KILLASSIST_DAMAGE"][0]);
            GiveHealth.Value = Int32.Parse(Scores["SCORE_HEAL"][0]);
            GiveAmmo.Value = Int32.Parse(Scores["SCORE_GIVEAMMO"][0]);
            VehicleRepair.Value = Int32.Parse(Scores["SCORE_REPAIR"][0]);

            // Player Conquest Settings
            ConqFlagCapture.Value = Int32.Parse(ConqScores["SCORE_CAPTURE"][0]);
            ConqFlagCaptureAsst.Value = Int32.Parse(ConqScores["SCORE_CAPTUREASSIST"][0]);
            ConqFlagNeutralize.Value = Int32.Parse(ConqScores["SCORE_NEUTRALIZE"][0]);
            ConqFlagNeutralizeAsst.Value = Int32.Parse(ConqScores["SCORE_NEUTRALIZEASSIST"][0]);
            ConqDefendFlag.Value = Int32.Parse(ConqScores["SCORE_DEFEND"][0]);

            // Player Coop Settings
            CoopFlagCapture.Value = Int32.Parse(CoopScores["SCORE_CAPTURE"][0]);
            CoopFlagCaptureAsst.Value = Int32.Parse(CoopScores["SCORE_CAPTUREASSIST"][0]);
            CoopFlagNeutralize.Value = Int32.Parse(CoopScores["SCORE_NEUTRALIZE"][0]);
            CoopFlagNeutralizeAsst.Value = Int32.Parse(CoopScores["SCORE_NEUTRALIZEASSIST"][0]);
            CoopDefendFlag.Value = Int32.Parse(CoopScores["SCORE_DEFEND"][0]);

            // AI Bots Global Scoring
            AiKillScore.Value = Int32.Parse(Scores["AI_SCORE_KILL"][0]);
            AiTeamKillScore.Value = Int32.Parse(Scores["AI_SCORE_TEAMKILL"][0]);
            AiSuicideScore.Value = Int32.Parse(Scores["AI_SCORE_SUICIDE"][0]);
            AiReviveScore.Value = Int32.Parse(Scores["AI_SCORE_REVIVE"][0]);
            AiTeamDamage.Value = Int32.Parse(Scores["AI_SCORE_TEAMDAMAGE"][0]);
            AiTeamVehicleDamage.Value = Int32.Parse(Scores["AI_SCORE_TEAMVEHICLEDAMAGE"][0]);
            AiDestroyEnemyAsset.Value = Int32.Parse(Scores["AI_SCORE_DESTROYREMOTECONTROLLED"][0]);
            AiDriverKA.Value = Int32.Parse(Scores["AI_SCORE_KILLASSIST_DRIVER"][0]);
            AiPassangerKA.Value = Int32.Parse(Scores["AI_SCORE_KILLASSIST_PASSENGER"][0]);
            AiTargeterKA.Value = Int32.Parse(Scores["AI_SCORE_KILLASSIST_TARGETER"][0]);
            AiDamageAssist.Value = Int32.Parse(Scores["AI_SCORE_KILLASSIST_DAMAGE"][0]);
            AiGiveHealth.Value = Int32.Parse(Scores["AI_SCORE_HEAL"][0]);
            AiGiveAmmo.Value = Int32.Parse(Scores["AI_SCORE_GIVEAMMO"][0]);
            AiVehicleRepair.Value = Int32.Parse(Scores["AI_SCORE_REPAIR"][0]);

            // AI Bots Coop Settings
            AiCoopFlagCapture.Value = Int32.Parse(CoopScores["AI_SCORE_CAPTURE"][0]);
            AiCoopFlagCaptureAsst.Value = Int32.Parse(CoopScores["AI_SCORE_CAPTUREASSIST"][0]);
            AiCoopFlagNeutralize.Value = Int32.Parse(CoopScores["AI_SCORE_NEUTRALIZE"][0]);
            AiCoopFlagNeutralizeAsst.Value = Int32.Parse(CoopScores["AI_SCORE_NEUTRALIZEASSIST"][0]);
            AiCoopDefendFlag.Value = Int32.Parse(CoopScores["AI_SCORE_DEFEND"][0]);

            // AI Bots Conquest Settings
            AiCqFlagCapture.Value = Int32.Parse(ConqScores["AI_SCORE_CAPTURE"][0]);
            AiCqFlagCaptureAsst.Value = Int32.Parse(ConqScores["AI_SCORE_CAPTUREASSIST"][0]);
            AiCqFlagNeutralize.Value = Int32.Parse(ConqScores["AI_SCORE_NEUTRALIZE"][0]);
            AiCqFlagNeutralizeAsst.Value = Int32.Parse(ConqScores["AI_SCORE_NEUTRALIZEASSIST"][0]);
            AiCqDefendFlag.Value = Int32.Parse(ConqScores["AI_SCORE_DEFEND"][0]);

            // Replenish Scoring
            RepairPointLimit.Value = Int32.Parse(Scores["REPAIR_POINT_LIMIT"][0]);
            HealPointLimit.Value = Int32.Parse(Scores["HEAL_POINT_LIMIT"][0]);
            AmmoPointLimit.Value = Int32.Parse(Scores["GIVEAMMO_POINT_LIMIT"][0]);
            TeamDamageLimit.Value = Int32.Parse(Scores["TEAMDAMAGE_POINT_LIMIT"][0]);
            TeamVDamageLimit.Value = Int32.Parse(Scores["TEAMVEHICLEDAMAGE_POINT_LIMIT"][0]);
            ReplenishInterval.Value = Int32.Parse(Scores["REPLENISH_POINT_MIN_INTERVAL"][0]);
        }

        #region Events

        /// <summary>
        /// Event fired when the Cancel button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event fired when the Save buttons is pressed.
        /// Saves all the current scoring settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string contents;

            // ========================================== Scoring Common
            // Get current file contents
            using (Stream Str = ScoringCommonFile.OpenRead())
            using (StreamReader Rdr = new StreamReader(Str))
                contents = Rdr.ReadToEnd();

            // Player
            contents = Regex.Replace(contents, Scores["SCORE_KILL"][1], "SCORE_KILL = " + KillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMKILL"][1], "SCORE_TEAMKILL = " + TeamKillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_SUICIDE"][1], "SCORE_SUICIDE = " + SuicideScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_REVIVE"][1], "SCORE_REVIVE = " + ReviveScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMDAMAGE"][1], "SCORE_TEAMDAMAGE = " + TeamDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMVEHICLEDAMAGE"][1], "SCORE_TEAMVEHICLEDAMAGE = " + TeamVehicleDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_DESTROYREMOTECONTROLLED"][1], "SCORE_DESTROYREMOTECONTROLLED = " + DestroyEnemyAsset.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_DRIVER"][1], "SCORE_KILLASSIST_DRIVER = " + DriverKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_PASSENGER"][1], "SCORE_KILLASSIST_PASSENGER = " + PassangerKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_TARGETER"][1], "SCORE_KILLASSIST_TARGETER = " + TargeterKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_DAMAGE"][1], "SCORE_KILLASSIST_DAMAGE = " + DamageAssist.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_HEAL"][1], "SCORE_HEAL = " + GiveHealth.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_GIVEAMMO"][1], "SCORE_GIVEAMMO = " + GiveAmmo.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["SCORE_REPAIR"][1], "SCORE_REPAIR = " + VehicleRepair.Value, RegexOptions.Multiline);
            // Bots
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILL"][1], "AI_SCORE_KILL = " + AiKillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMKILL"][1], "AI_SCORE_TEAMKILL = " + AiTeamKillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_SUICIDE"][1], "AI_SCORE_SUICIDE = " + AiSuicideScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_REVIVE"][1], "AI_SCORE_REVIVE = " + AiReviveScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMDAMAGE"][1], "AI_SCORE_TEAMDAMAGE = " + AiTeamDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMVEHICLEDAMAGE"][1], "AI_SCORE_TEAMVEHICLEDAMAGE = " + AiTeamVehicleDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_DESTROYREMOTECONTROLLED"][1], "AI_SCORE_DESTROYREMOTECONTROLLED = " + AiDestroyEnemyAsset.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_DRIVER"][1], "AI_SCORE_KILLASSIST_DRIVER = " + AiDriverKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_PASSENGER"][1], "AI_SCORE_KILLASSIST_PASSENGER = " + AiPassangerKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_TARGETER"][1], "AI_SCORE_KILLASSIST_TARGETER = " + AiTargeterKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_DAMAGE"][1], "AI_SCORE_KILLASSIST_DAMAGE = " + AiDamageAssist.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_HEAL"][1], "AI_SCORE_HEAL = " + AiGiveHealth.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_GIVEAMMO"][1], "AI_SCORE_GIVEAMMO = " + AiGiveAmmo.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["AI_SCORE_REPAIR"][1], "AI_SCORE_REPAIR = " + AiVehicleRepair.Value, RegexOptions.Multiline);
            // Replenish
            contents = Regex.Replace(contents, Scores["REPAIR_POINT_LIMIT"][1], "REPAIR_POINT_LIMIT = " + RepairPointLimit.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["HEAL_POINT_LIMIT"][1], "HEAL_POINT_LIMIT = " + HealPointLimit.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["GIVEAMMO_POINT_LIMIT"][1], "GIVEAMMO_POINT_LIMIT = " + AmmoPointLimit.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["TEAMDAMAGE_POINT_LIMIT"][1], "TEAMDAMAGE_POINT_LIMIT = " + TeamDamageLimit.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["TEAMVEHICLEDAMAGE_POINT_LIMIT"][1], "TEAMVEHICLEDAMAGE_POINT_LIMIT = " + TeamVDamageLimit.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, Scores["REPLENISH_POINT_MIN_INTERVAL"][1], "REPLENISH_POINT_MIN_INTERVAL = " + ReplenishInterval.Value, RegexOptions.Multiline);
            
            // Save File
            using (Stream Str = ScoringCommonFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(contents);
                Wtr.Flush();
            }

            // ========================================== Scoring Conquest
            // Get curent file contents
            using (Stream Str = ScoringConqFile.OpenRead())
            using (StreamReader Rdr = new StreamReader(Str))
                contents = Rdr.ReadToEnd();

            // Do Replacements
            contents = Regex.Replace(contents, ConqScores["SCORE_CAPTURE"][1], "SCORE_CAPTURE = " + ConqFlagCapture.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["SCORE_CAPTUREASSIST"][1], "SCORE_CAPTUREASSIST = " + ConqFlagCaptureAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["SCORE_NEUTRALIZE"][1], "SCORE_NEUTRALIZE = " + ConqFlagNeutralize.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["SCORE_NEUTRALIZEASSIST"][1], "SCORE_NEUTRALIZEASSIST = " + ConqFlagNeutralizeAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["SCORE_DEFEND"][1], "SCORE_DEFEND = " + ConqDefendFlag.Value, RegexOptions.Multiline);
            // Bots
            contents = Regex.Replace(contents, ConqScores["AI_SCORE_CAPTURE"][1], "AI_SCORE_CAPTURE = " + AiCqFlagCapture.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["AI_SCORE_CAPTUREASSIST"][1], "AI_SCORE_CAPTUREASSIST = " + AiCqFlagCaptureAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["AI_SCORE_NEUTRALIZE"][1], "AI_SCORE_NEUTRALIZE = " + AiCqFlagNeutralize.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["AI_SCORE_NEUTRALIZEASSIST"][1], "AI_SCORE_NEUTRALIZEASSIST = " + AiCqFlagNeutralizeAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, ConqScores["AI_SCORE_DEFEND"][1], "AI_SCORE_DEFEND = " + AiCqDefendFlag.Value, RegexOptions.Multiline);
            

            // Save File
            using (Stream Str = ScoringConqFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(contents);
                Wtr.Flush();
            }

            // ========================================== Scoring Coop
            // Get current file contents
            using (Stream Str = ScoringCoopFile.OpenRead())
            using (StreamReader Rdr = new StreamReader(Str))
                contents = Rdr.ReadToEnd();

            // Do Replacements
            contents = Regex.Replace(contents, CoopScores["SCORE_CAPTURE"][1], "SCORE_CAPTURE = " + CoopFlagCapture.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["SCORE_CAPTUREASSIST"][1], "SCORE_CAPTUREASSIST = " + CoopFlagCaptureAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["SCORE_NEUTRALIZE"][1], "SCORE_NEUTRALIZE = " + CoopFlagNeutralize.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["SCORE_NEUTRALIZEASSIST"][1], "SCORE_NEUTRALIZEASSIST = " + CoopFlagNeutralizeAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["SCORE_DEFEND"][1], "SCORE_DEFEND = " + CoopDefendFlag.Value, RegexOptions.Multiline);
            // Bots
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_CAPTURE"][1], "AI_SCORE_CAPTURE = " + AiCoopFlagCapture.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_CAPTUREASSIST"][1], "AI_SCORE_CAPTUREASSIST = " + AiCoopFlagCaptureAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_NEUTRALIZE"][1], "AI_SCORE_NEUTRALIZE = " + AiCoopFlagNeutralize.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_NEUTRALIZEASSIST"][1], "AI_SCORE_NEUTRALIZEASSIST = " + AiCoopFlagNeutralizeAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_DEFEND"][1], "AI_SCORE_DEFEND = " + AiCoopDefendFlag.Value, RegexOptions.Multiline);
            

            // Save File
            using (Stream Str = ScoringCoopFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(contents);
                Wtr.Flush();
            }

            // Remove the ServerSettings.con file as that screws with the replenish scores
            FileInfo file = new FileInfo(Path.Combine(MainForm.SelectedMod.RootPath, "Settings", "ScoreManagerSetup.con"));
            if (file.Exists)
                file.Rename("ScoreManagerSetup.con.bak");
                
            // Close this form
            this.Close();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Scoring Common
            KillScore.Value = 2;
            ReviveScore.Value = 2;
            DestroyEnemyAsset.Value = 1;
            GiveHealth.Value = 1;
            GiveAmmo.Value = 1;
            VehicleRepair.Value = 1;
            TeamKillScore.Value = -4;
            TeamDamage.Value = -2;
            TeamVehicleDamage.Value = -1;
            SuicideScore.Value = -2;
            DriverKA.Value = 1;
            PassangerKA.Value = 1;
            TargeterKA.Value = 0;
            DamageAssist.Value = 1;

            // Conquest
            ConqFlagCapture.Value = 2;
            ConqFlagCaptureAsst.Value = 1;
            ConqFlagNeutralize.Value = 2;
            ConqFlagNeutralizeAsst.Value = 1;
            ConqDefendFlag.Value = 1;

            // Coop
            CoopFlagCapture.Value = 2;
            CoopFlagCaptureAsst.Value = 1;
            CoopFlagNeutralize.Value = 2;
            CoopFlagNeutralizeAsst.Value = 1;
            CoopDefendFlag.Value = 1;

            // Scoring Common
            AiKillScore.Value = 2;
            AiReviveScore.Value = 2;
            AiDestroyEnemyAsset.Value = 1;
            AiGiveHealth.Value = 1;
            AiGiveAmmo.Value = 1;
            AiVehicleRepair.Value = 1;
            AiTeamKillScore.Value = -4;
            AiTeamDamage.Value = -2;
            AiTeamVehicleDamage.Value = -1;
            AiSuicideScore.Value = -2;
            AiDriverKA.Value = 1;
            AiPassangerKA.Value = 1;
            AiTargeterKA.Value = 0;
            AiDamageAssist.Value = 1;

            // Conquest
            AiCoopFlagCapture.Value = 2;
            AiCoopFlagCaptureAsst.Value = 1;
            AiCoopFlagNeutralize.Value = 2;
            AiCoopFlagNeutralizeAsst.Value = 1;
            AiCoopDefendFlag.Value = 1;

            // Replensih
            RepairPointLimit.Value = 100;
            HealPointLimit.Value = 100;
            AmmoPointLimit.Value = 100;
            TeamDamageLimit.Value = 50;
            TeamVDamageLimit.Value = 50;
            ReplenishInterval.Value = 30;
        }

        #endregion

        private void ImportSettingsMenuItem_Click(object sender, EventArgs e)
        {
            // Define our file path and 
            string file = Path.Combine(Paths.DocumentsFolder, "ScoringSettings.xml");

            // Make sure the file exists
            if (!File.Exists(file))
            {
                MessageBox.Show(
                    "There are currently no exported savings to import.",
                    "No Saved Settings", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
                return;
            }

            //=== Load the file, and import

            // Generate our mappings
            Dictionary<string, string[]>[] types = { Scores, ConqScores, CoopScores };
            string[] names = { "general", "conquest", "coop" };

            // Itterate through all of our saved settings, and set them in the 
            // correct scoring dictionary
            try
            {
                using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    // Load the XML file
                    XDocument Doc = XDocument.Load(stream);
                    for (int i = 0; i < 3; i++)
                    {
                        // Load element
                        XElement element = Doc.Root.Element(names[i]);

                        // Add each scoring item to the XML
                        foreach (XElement item in element.Elements())
                        {
                            string itemName = item.FirstAttribute.Value;
                            switch (i)
                            {
                                case 0:
                                    Scores[itemName][0] = item.Value;
                                    break;
                                case 1:
                                    ConqScores[itemName][0] = item.Value;
                                    break;
                                case 2:
                                    CoopScores[itemName][0] = item.Value;
                                    break;
                            }
                        }
                    }
                }

                // Fill form values
                FillFormFields();
            }
            catch
            {
                throw;
            }
        }

        private void ExportSettingsMenuItem_Click(object sender, EventArgs e)
        {
            // Warn the user about saved changes
            DialogResult res = MessageBox.Show(
                "Changes made since this window was opened will not reflect in the ScoringSettings.xml file "
                + "without reloading the saved changes. Would you like me to reload the last saved values?", 
                "Reload Settings", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question
            );

            // Return if user cancels
            if (res == DialogResult.Cancel) return;

            // Show loading form
            LoadingForm.ShowScreen(this);

            // Reload settings!
            if (res == DialogResult.Yes && (!LoadConqFile() || !LoadCoopFile() || !LoadScoringCommon()))
            {
                LoadingForm.CloseForm();
                return;
            }

            try
            {
                // Define our file path and 
                string file = Path.Combine(Paths.DocumentsFolder, "ScoringSettings.xml");

                // Generate our mappings
                Dictionary<string, string[]>[] types = { Scores, ConqScores, CoopScores };
                string[] names = { "general", "conquest", "coop" };

                // Create XML Settings
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = Environment.NewLine;
                settings.NewLineHandling = NewLineHandling.Replace;

                // Write to file
                using (FileStream stream = File.Open(file, FileMode.Create))
                using (XmlWriter Writer = XmlWriter.Create(stream, settings))
                {
                    // Player Element
                    Writer.WriteStartDocument();

                    // Write editing warning
                    Writer.WriteComment(" Auto Generated :: Please DO NOT Edit Me! ");

                    // Begin
                    Writer.WriteStartElement("settings");

                    // Itterate through all setting catagories
                    for (int i = 0; i < 3; i++)
                    {
                        // Start general Element
                        Writer.WriteStartElement(names[i]);

                        // Add each scoring item to the XML
                        foreach (KeyValuePair<string, string[]> item in types[i])
                        {
                            // Open Row tag
                            Writer.WriteStartElement("item");
                            Writer.WriteAttributeString("name", item.Key);
                            Writer.WriteValue(item.Value[0]);
                            Writer.WriteEndElement();
                        }

                        // Close settings element
                        Writer.WriteEndElement();
                    }

                    // Close Tags and File
                    Writer.WriteEndElement();  // Close general Element
                    Writer.WriteEndDocument(); // End and Save file
                }

                // Notify user
                Notify.Show("Settings Exported", "Scoring settings were exported successfully!", AlertType.Success);
            }
            finally
            {
                LoadingForm.CloseForm();
            }
        }
    }
}
