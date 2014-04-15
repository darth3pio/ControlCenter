using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

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
                this.Load += new EventHandler(CloseOnStart);
                return;
            }
                
            // Load Common Scoring File
            if (!LoadScoringCommon())
                return;

            // Load the Coop Scoring file
            if (!LoadCoopFile())
                return;

            // Load the Conquest Scoring file
            LoadConqFile();

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
            string ModPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_scoringCommon.py");
            string DefaultPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", "bf2_scoringCommon.py");

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

                this.Load += new EventHandler(CloseOnStart);
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
                        this.Load += new EventHandler(CloseOnStart);
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
                        this.Load += new EventHandler(CloseOnStart);
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
                Scores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, m.Value });

            return true;
        }

        /// <summary>
        /// Loads the conquest scoring file
        /// </summary>
        private void LoadConqFile()
        {
            ConqScores = new Dictionary<string, string[]>();
            string file = File.ReadAllText(ScoringConqFile.FullName);
            MatchCollection Matches = Reg.Matches(file);
            foreach (Match m in Matches)
                ConqScores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, m.Value });
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
                    file = Rdr.ReadToEnd();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Unable to Read/Write to the Coop scoring file:" + Environment.NewLine
                    + Environment.NewLine + "File: " + ScoringCommonFile.FullName
                    + Environment.NewLine + "Error: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                this.Load += new EventHandler(CloseOnStart);
                return false;
            }

            // Process
            if (!file.StartsWith("# BF2Statistics Formatted Coop Scoring"))
            {
                // We need to replace the default file with the embedded one that
                // Correctly formats the AI_ Scores
                string DefaultPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", "bf2_coop.py");
                string ModPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_coop.py");

                if (!File.Exists(ModPath))
                {
                    // Show warn dialog
                    if (MessageBox.Show(
                        "The Coop Scoring file needs to be formatted to use this feature. If you are using a third party mod,"
                        + " then formatting can break the scoring. Do you want to Format now?",
                        "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += new EventHandler(CloseOnStart);
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
                        this.Load += new EventHandler(CloseOnStart);
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
                CoopScores.Add(m.Groups["varname"].Value, new string[] { m.Groups["value"].Value, m.Value });

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

            // AI Bots Conquest Settings
            AiFlagCapture.Value = Int32.Parse(CoopScores["AI_SCORE_CAPTURE"][0]);
            AiFlagCaptureAsst.Value = Int32.Parse(CoopScores["AI_SCORE_CAPTUREASSIST"][0]);
            AiFlagNeutralize.Value = Int32.Parse(CoopScores["AI_SCORE_NEUTRALIZE"][0]);
            AiFlagNeutralizeAsst.Value = Int32.Parse(CoopScores["AI_SCORE_NEUTRALIZEASSIST"][0]);
            AiDefendFlag.Value = Int32.Parse(CoopScores["AI_SCORE_DEFEND"][0]);

            // Replenish Scoring
            RepairPointLimit.Value = Int32.Parse(Scores["REPAIR_POINT_LIMIT"][0]);
            HealPointLimit.Value = Int32.Parse(Scores["HEAL_POINT_LIMIT"][0]);
            AmmoPointLimit.Value = Int32.Parse(Scores["GIVEAMMO_POINT_LIMIT"][0]);
            TeamDamageLimit.Value = Int32.Parse(Scores["TEAMDAMAGE_POINT_LIMIT"][0]);
            TeamVDamageLimit.Value = Int32.Parse(Scores["TEAMVEHICLEDAMAGE_POINT_LIMIT"][0]);
            ReplenishInterval.Value = Int32.Parse(Scores["REPLENISH_POINT_MIN_INTERVAL"][0]);
        }

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
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
            contents = Regex.Replace(contents, Scores["SCORE_KILL"][1], "SCORE_KILL = " + KillScore.Value);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMKILL"][1], "SCORE_TEAMKILL = " + TeamKillScore.Value);
            contents = Regex.Replace(contents, Scores["SCORE_SUICIDE"][1], "SCORE_SUICIDE = " + SuicideScore.Value);
            contents = Regex.Replace(contents, Scores["SCORE_REVIVE"][1], "SCORE_REVIVE = " + ReviveScore.Value);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMDAMAGE"][1], "SCORE_TEAMDAMAGE = " + TeamDamage.Value);
            contents = Regex.Replace(contents, Scores["SCORE_TEAMVEHICLEDAMAGE"][1], "SCORE_TEAMVEHICLEDAMAGE = " + TeamVehicleDamage.Value);
            contents = Regex.Replace(contents, Scores["SCORE_DESTROYREMOTECONTROLLED"][1], "SCORE_DESTROYREMOTECONTROLLED = " + DestroyEnemyAsset.Value);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_DRIVER"][1], "SCORE_KILLASSIST_DRIVER = " + DriverKA.Value);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_PASSENGER"][1], "SCORE_KILLASSIST_PASSENGER = " + PassangerKA.Value);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_TARGETER"][1], "SCORE_KILLASSIST_TARGETER = " + TargeterKA.Value);
            contents = Regex.Replace(contents, Scores["SCORE_KILLASSIST_DAMAGE"][1], "SCORE_KILLASSIST_DAMAGE = " + DamageAssist.Value);
            contents = Regex.Replace(contents, Scores["SCORE_HEAL"][1], "SCORE_HEAL = " + GiveHealth.Value);
            contents = Regex.Replace(contents, Scores["SCORE_GIVEAMMO"][1], "SCORE_GIVEAMMO = " + GiveAmmo.Value);
            contents = Regex.Replace(contents, Scores["SCORE_REPAIR"][1], "SCORE_REPAIR = " + VehicleRepair.Value);
            // Bots
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILL"][1], "AI_SCORE_KILL = " + AiKillScore.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMKILL"][1], "AI_SCORE_TEAMKILL = " + AiTeamKillScore.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_SUICIDE"][1], "AI_SCORE_SUICIDE = " + AiSuicideScore.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_REVIVE"][1], "AI_SCORE_REVIVE = " + AiReviveScore.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMDAMAGE"][1], "AI_SCORE_TEAMDAMAGE = " + AiTeamDamage.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_TEAMVEHICLEDAMAGE"][1], "AI_SCORE_TEAMVEHICLEDAMAGE = " + AiTeamVehicleDamage.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_DESTROYREMOTECONTROLLED"][1], "AI_SCORE_DESTROYREMOTECONTROLLED = " + AiDestroyEnemyAsset.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_DRIVER"][1], "AI_SCORE_KILLASSIST_DRIVER = " + AiDriverKA.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_PASSENGER"][1], "AI_SCORE_KILLASSIST_PASSENGER = " + AiPassangerKA.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_TARGETER"][1], "AI_SCORE_KILLASSIST_TARGETER = " + AiTargeterKA.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_KILLASSIST_DAMAGE"][1], "AI_SCORE_KILLASSIST_DAMAGE = " + AiDamageAssist.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_HEAL"][1], "AI_SCORE_HEAL = " + AiGiveHealth.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_GIVEAMMO"][1], "AI_SCORE_GIVEAMMO = " + AiGiveAmmo.Value);
            contents = Regex.Replace(contents, Scores["AI_SCORE_REPAIR"][1], "AI_SCORE_REPAIR = " + AiVehicleRepair.Value);
            // Replenish
            contents = Regex.Replace(contents, Scores["REPAIR_POINT_LIMIT"][1], "REPAIR_POINT_LIMIT = " + RepairPointLimit.Value);
            contents = Regex.Replace(contents, Scores["HEAL_POINT_LIMIT"][1], "HEAL_POINT_LIMIT = " + HealPointLimit.Value);
            contents = Regex.Replace(contents, Scores["GIVEAMMO_POINT_LIMIT"][1], "GIVEAMMO_POINT_LIMIT = " + AmmoPointLimit.Value);
            contents = Regex.Replace(contents, Scores["TEAMDAMAGE_POINT_LIMIT"][1], "TEAMDAMAGE_POINT_LIMIT = " + TeamDamageLimit.Value);
            contents = Regex.Replace(contents, Scores["TEAMVEHICLEDAMAGE_POINT_LIMIT"][1], "TEAMVEHICLEDAMAGE_POINT_LIMIT = " + TeamVDamageLimit.Value);
            contents = Regex.Replace(contents, Scores["REPLENISH_POINT_MIN_INTERVAL"][1], "REPLENISH_POINT_MIN_INTERVAL = " + ReplenishInterval.Value);
            
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
            contents = Regex.Replace(contents, ConqScores["SCORE_CAPTURE"][1], "SCORE_CAPTURE = " + ConqFlagCapture.Value);
            contents = Regex.Replace(contents, ConqScores["SCORE_CAPTUREASSIST"][1], "SCORE_CAPTUREASSIST = " + ConqFlagCaptureAsst.Value);
            contents = Regex.Replace(contents, ConqScores["SCORE_NEUTRALIZE"][1], "SCORE_NEUTRALIZE = " + ConqFlagNeutralize.Value);
            contents = Regex.Replace(contents, ConqScores["SCORE_NEUTRALIZEASSIST"][1], "SCORE_NEUTRALIZEASSIST = " + ConqFlagNeutralizeAsst.Value);
            contents = Regex.Replace(contents, ConqScores["SCORE_DEFEND"][1], "SCORE_DEFEND = " + ConqDefendFlag.Value);

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
            contents = Regex.Replace(contents, CoopScores["SCORE_CAPTURE"][1], "SCORE_CAPTURE = " + CoopFlagCapture.Value);
            contents = Regex.Replace(contents, CoopScores["SCORE_CAPTUREASSIST"][1], "SCORE_CAPTUREASSIST = " + CoopFlagCaptureAsst.Value);
            contents = Regex.Replace(contents, CoopScores["SCORE_NEUTRALIZE"][1], "SCORE_NEUTRALIZE = " + CoopFlagNeutralize.Value);
            contents = Regex.Replace(contents, CoopScores["SCORE_NEUTRALIZEASSIST"][1], "SCORE_NEUTRALIZEASSIST = " + CoopFlagNeutralizeAsst.Value);
            contents = Regex.Replace(contents, CoopScores["SCORE_DEFEND"][1], "SCORE_DEFEND = " + CoopDefendFlag.Value);
            // Bots
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_CAPTURE"][1], "AI_SCORE_CAPTURE = " + AiFlagCapture.Value);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_CAPTUREASSIST"][1], "AI_SCORE_CAPTUREASSIST = " + AiFlagCaptureAsst.Value);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_NEUTRALIZE"][1], "AI_SCORE_NEUTRALIZE = " + AiFlagNeutralize.Value);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_NEUTRALIZEASSIST"][1], "AI_SCORE_NEUTRALIZEASSIST = " + AiFlagNeutralizeAsst.Value);
            contents = Regex.Replace(contents, CoopScores["AI_SCORE_DEFEND"][1], "AI_SCORE_DEFEND = " + AiDefendFlag.Value);
            

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
            AiFlagCapture.Value = 2;
            AiFlagCaptureAsst.Value = 1;
            AiFlagNeutralize.Value = 2;
            AiFlagNeutralizeAsst.Value = 1;
            AiDefendFlag.Value = 1;

            // Replensih
            RepairPointLimit.Value = 100;
            HealPointLimit.Value = 100;
            AmmoPointLimit.Value = 100;
            TeamDamageLimit.Value = 50;
            TeamVDamageLimit.Value = 50;
            ReplenishInterval.Value = 30;
        }

        #endregion
    }
}
