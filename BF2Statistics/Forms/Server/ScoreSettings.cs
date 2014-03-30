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

        // Ai Prefix?
        protected bool PrefixAI = false;
        protected string Prefix;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PrefixAI">Are we doing AI scoring files?</param>
        public ScoreSettings(bool PrefixAI)
        {
            InitializeComponent();

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

            // Are we doing AI scoring?
            if (PrefixAI)
            {
                // Hide Conquest tab as its not needed for AI obviously
                tabControl1.TabPages.Remove(tabPage2);
                this.Prefix = "AI_";
                this.Text = "AI Score Settings";
                this.PrefixAI = true;
            }
                
            // Load score settings
            LoadSettings();
        }

        /// <summary>
        /// Loads the scoring files, and initializes the values of all the input fields
        /// </summary>
        private void LoadSettings()
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
                return;
            }

            // First, we are going to check for a certain string... if it exists
            // Then these config file has been reformated already, else we need
            // to reformat it now
            if (!file.Contains("AI_SCORE_KILL"))
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
                        return;
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
                        return;
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
            string Expression = (PrefixAI)
                ? @"^AI_SCORE_(?<varname>[A-Z_]+)(?:[\s|\t]*)=(?:[\s|\t]*)(?<value>[-]?[0-9]+)"
                : @"^SCORE_(?<varname>[A-Z_]+)(?:[\s|\t]*)=(?:[\s|\t]*)(?<value>[-]?[0-9]+)";
            Regex Reg = new Regex(Expression, RegexOptions.Multiline);

            // Get all matches for the ScoringCommon.py
            MatchCollection Matches = Reg.Matches(file);
            foreach (Match m in Matches)
            {
                switch (m.Groups["varname"].Value)
                {
                    case "KILL":
                        KillScore.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "TEAMKILL":
                        TeamKillScore.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "SUICIDE":
                        SuicideScore.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "REVIVE":
                        ReviveScore.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "TEAMDAMAGE":
                        TeamDamage.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "TEAMVEHICLEDAMAGE":
                        TeamVehicleDamage.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "DESTROYREMOTECONTROLLED":
                        DestroyEnemyAsset.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "KILLASSIST_DRIVER": 
                        DriverKA.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "KILLASSIST_PASSENGER":
                        PassangerKA.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "KILLASSIST_TARGETER":
                        TargeterKA.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "KILLASSIST_DAMAGE":
                        DamageAssist.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "HEAL":
                        GiveHealth.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "GIVEAMMO":
                        GiveAmmo.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "REPAIR":
                        VehicleRepair.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                }
            }

            // Move on to the Conquest Scoring
            if (!PrefixAI)
            {
                file = File.ReadAllText(ScoringConqFile.FullName);
                Matches = Reg.Matches(file);

                foreach (Match m in Matches)
                {
                    switch (m.Groups["varname"].Value)
                    {
                        case "CAPTURE":
                            ConqFlagCapture.Value = Int32.Parse(m.Groups["value"].Value);
                            break;
                        case "CAPTUREASSIST":
                            ConqFlagCaptureAsst.Value = Int32.Parse(m.Groups["value"].Value);
                            break;
                        case "NEUTRALIZE":
                            ConqFlagNeutralize.Value = Int32.Parse(m.Groups["value"].Value);
                            break;
                        case "NEUTRALIZEASSIST":
                            ConqFlagNeutralizeAsst.Value = Int32.Parse(m.Groups["value"].Value);
                            break;
                        case "DEFEND":
                            ConqDefendFlag.Value = Int32.Parse(m.Groups["value"].Value);
                            break;
                    }
                }
            }

            // Move on to the Coop Scoring. Check file for read and write access
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
                return;
            }

            // Process
            if (!file.Contains("AI_SCORE_CAPTURE "))
            {
                // We need to replace the default file with the embedded one that
                // Correctly formats the AI_ Scores
                DefaultPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", "bf2_coop.py");
                ModPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", MainForm.SelectedMod.Name + "_coop.py");

                if (!File.Exists(ModPath))
                {
                    // Show warn dialog
                    if (MessageBox.Show(
                        "The Coop Scoring file needs to be formatted to use this feature. If you are using a third party mod,"
                        + " then formatting can break the scoring. Do you want to Format now?",
                        "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Load += new EventHandler(CloseOnStart);
                        return;
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
                        return;
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

            Matches = Reg.Matches(file);
            foreach (Match m in Matches)
            {
                switch (m.Groups["varname"].Value)
                {
                    case "CAPTURE":
                        CoopFlagCapture.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "CAPTUREASSIST":
                        CoopFlagCaptureAsst.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "NEUTRALIZE":
                        CoopFlagNeutralize.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "NEUTRALIZEASSIST":
                        CoopFlagNeutralizeAsst.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                    case "DEFEND":
                        CoopDefendFlag.Value = Int32.Parse(m.Groups["value"].Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Events

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string contents;

            // for each of the 2 scoring files, we use regex to set the values
            // Scoring Common
            using (Stream Str = ScoringCommonFile.OpenRead())
            using (StreamReader Rdr = new StreamReader(Str))
                contents = Rdr.ReadToEnd();

            // Do replacements
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_KILL = " + KillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMKILL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_TEAMKILL = " + TeamKillScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_SUICIDE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_SUICIDE = " + SuicideScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_REVIVE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_REVIVE = " + ReviveScore.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMDAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_TEAMDAMAGE = " + TeamDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_DESTROYREMOTECONTROLLED(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_DESTROYREMOTECONTROLLED = " + DestroyEnemyAsset.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMVEHICLEDAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_TEAMVEHICLEDAMAGE = " + TeamVehicleDamage.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_DRIVER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_DRIVER = " + DriverKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_PASSENGER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_PASSENGER = " + PassangerKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_TARGETER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_TARGETER = " + TargeterKA.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_DAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_DAMAGE = " + DamageAssist.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_HEAL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_HEAL = " + GiveHealth.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_GIVEAMMO(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_GIVEAMMO = " + GiveAmmo.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_REPAIR(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_REPAIR = " + VehicleRepair.Value, RegexOptions.Multiline);
            
            // Save File
            using (Stream Str = ScoringCommonFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(contents);
                Wtr.Flush();
            }

            // Scoring Conquest
            if (!PrefixAI)
            {
                // Get curent file contents
                using (Stream Str = ScoringConqFile.OpenRead())
                using (StreamReader Rdr = new StreamReader(Str))
                    contents = Rdr.ReadToEnd();

                // Do Replacements
                contents = Regex.Replace(contents, @"SCORE_CAPTURE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_CAPTURE = " + ConqFlagCapture.Value);
                contents = Regex.Replace(contents, @"SCORE_CAPTUREASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                    "SCORE_CAPTUREASSIST = " + ConqFlagCaptureAsst.Value);
                contents = Regex.Replace(contents, @"SCORE_NEUTRALIZE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_NEUTRALIZE = " + ConqFlagNeutralize.Value);
                contents = Regex.Replace(contents, @"SCORE_NEUTRALIZEASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                    "SCORE_NEUTRALIZEASSIST = " + ConqFlagNeutralizeAsst.Value);
                contents = Regex.Replace(contents, @"SCORE_DEFEND(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_DEFENT = " + ConqDefendFlag.Value);

                // Save File
                using (Stream Str = ScoringConqFile.Open(FileMode.Truncate, FileAccess.Write))
                using (StreamWriter Wtr = new StreamWriter(Str))
                {
                    Wtr.Write(contents);
                    Wtr.Flush();
                }
            }

            // Scoring Coop
            using (Stream Str = ScoringCoopFile.OpenRead())
            using (StreamReader Rdr = new StreamReader(Str))
                contents = Rdr.ReadToEnd();

            // Do Replacements
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_CAPTURE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_CAPTURE = " + CoopFlagCapture.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_CAPTUREASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_CAPTUREASSIST = " + CoopFlagCaptureAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_NEUTRALIZE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_NEUTRALIZE = " + CoopFlagNeutralize.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_NEUTRALIZEASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_NEUTRALIZEASSIST = " + CoopFlagNeutralizeAsst.Value, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_DEFEND(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_DEFEND = " + CoopDefendFlag.Value, RegexOptions.Multiline);

            // Save File
            using (Stream Str = ScoringCoopFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(contents);
                Wtr.Flush();
            }

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
        }

        #endregion
    }
}
