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
        // File paths
        private string ScoringCommonPy;
        private string ScoringConqPy;
        private string ScoringCoopPy;

        // Ai Prefix?
        protected bool PrefixAI = false;
        protected string Prefix;

        public ScoreSettings(bool PrefixAI)
        {
            InitializeComponent();

            // Assign folder vars
            string ScoringFolder = Path.Combine(MainForm.Config.ServerPath, "mods", MainForm.SelectedMod, "python", "game");
            ScoringCommonPy = Path.Combine(ScoringFolder, "scoringCommon.py");
            ScoringConqPy = Path.Combine(ScoringFolder, "gamemodes", "gpm_cq.py");
            ScoringCoopPy = Path.Combine(ScoringFolder, "gamemodes", "gpm_coop.py");

            // Make sure the files all exist
            if (!File.Exists(ScoringCommonPy) || !File.Exists(ScoringConqPy) || !File.Exists(ScoringCoopPy))
            {
                MessageBox.Show("One or more scoring files are missing. Unable to modify scoring.", "Scoring Editor Error");
                this.Close();
            }

            // Are we doing AI scoring?
            this.PrefixAI = PrefixAI;
            this.Prefix = (PrefixAI) ? "AI_" : "";

            // Hide Conquest tab as its not needed obviously
            if (PrefixAI)
                tabControl1.TabPages.Remove(tabPage2);

            LoadSettings();
        }

        private void LoadSettings()
        {
            // First, we need to parse all 3 scoring files
            string file = File.ReadAllText(ScoringCommonPy);
            string ModPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", MainForm.SelectedMod + "_scoringCommon.py");
            string DefaultPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", "bf2_scoringCommon.py");

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
                        MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }
                else
                {
                    // Show warn dialog
                    if (MessageBox.Show("The scoringCommon.py file needs to be formatted to use this feature."
                        + " Do you want to Format now?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }

                // Check for a mod specific scoring common before replacing
                if (!File.Exists(ModPath))
                    file = File.ReadAllText(DefaultPath);
                else
                    file = File.ReadAllText(ModPath);

                File.WriteAllText(ScoringCommonPy, file);
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
                        KillScore.Text = m.Groups["value"].Value;
                        break;
                    case "TEAMKILL":
                        TeamKillScore.Text = m.Groups["value"].Value;
                        break;
                    case "SUICIDE":
                        SuicideScore.Text = m.Groups["value"].Value;
                        break;
                    case "REVIVE":
                        ReviveScore.Text = m.Groups["value"].Value;
                        break;
                    case "TEAMDAMAGE":
                        TeamDamage.Text = m.Groups["value"].Value;
                        break;
                    case "TEAMVEHICLEDAMAGE":
                        TeamVehicleDamage.Text = m.Groups["value"].Value;
                        break;
                    case "DESTROYREMOTECONTROLLED":
                        DestroyEnemyAsset.Text = m.Groups["value"].Value;
                        break;
                    case "KILLASSIST_DRIVER": 
                        DriverKA.Text = m.Groups["value"].Value;
                        break;
                    case "KILLASSIST_PASSENGER":
                        PassangerKA.Text = m.Groups["value"].Value;
                        break;
                    case "KILLASSIST_TARGETER":
                        TargeterKA.Text = m.Groups["value"].Value;
                        break;
                    case "KILLASSIST_DAMAGE":
                        DamageAssist.Text = m.Groups["value"].Value;
                        break;
                    case "HEAL":
                        GiveHealth.Text = m.Groups["value"].Value;
                        break;
                    case "GIVEAMMO":
                        GiveAmmo.Text = m.Groups["value"].Value;
                        break;
                    case "REPAIR":
                        VehicleRepair.Text = m.Groups["value"].Value;
                        break;
                }
            }

            // Move on to the Conquest Scoring
            if (!PrefixAI)
            {
                file = File.ReadAllText(ScoringConqPy);
                Matches = Reg.Matches(file);

                foreach (Match m in Matches)
                {
                    switch (m.Groups["varname"].Value)
                    {
                        case "CAPTURE":
                            ConqFlagCapture.Text = m.Groups["value"].Value;
                            break;
                        case "CAPTUREASSIST":
                            ConqFlagCaptureAsst.Text = m.Groups["value"].Value;
                            break;
                        case "NEUTRALIZE":
                            ConqFlagNeutralize.Text = m.Groups["value"].Value;
                            break;
                        case "NEUTRALIZEASSIST":
                            ConqFlagNeutralizeAsst.Text = m.Groups["value"].Value;
                            break;
                        case "DEFEND":
                            ConqDefendFlag.Text = m.Groups["value"].Value;
                            break;
                    }
                }
            }

            // Move on to the Coop Scoring
            file = File.ReadAllText(ScoringCoopPy);
            if (!file.Contains("AI_SCORE_CAPTURE "))
            {
                // We need to replace the default file with the embedded one that
                // Correctly formats the AI_ Scores
                DefaultPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", "bf2_coop.py");
                ModPath = Path.Combine(MainForm.Root, "Python", "ScoringFiles", MainForm.SelectedMod + "_coop.py");

                if (!File.Exists(ModPath))
                {
                    // Show warn dialog
                    if (MessageBox.Show(
                        "The Coop Scoring file needs to be formatted to use this feature. If you are using a third party mod,"
                        + " then formatting can break the scoring. Do you want to Format now?",
                        "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }
                else
                {
                    // Show warn dialog
                    if (MessageBox.Show("The Coop Scoring file needs to be formatted to use this feature."
                        + " Do you want to Format now?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }

                // Check for a mod specific scoring common before replacing
                if (!File.Exists(ModPath))
                    file = File.ReadAllText(DefaultPath);
                else
                    file = File.ReadAllText(ModPath);

                File.WriteAllText(ScoringCoopPy, file);
            }

            Matches = Reg.Matches(file);
            foreach (Match m in Matches)
            {
                switch (m.Groups["varname"].Value)
                {
                    case "CAPTURE":
                        CoopFlagCapture.Text = m.Groups["value"].Value;
                        break;
                    case "CAPTUREASSIST":
                        CoopFlagCaptureAsst.Text = m.Groups["value"].Value;
                        break;
                    case "NEUTRALIZE":
                        CoopFlagNeutralize.Text = m.Groups["value"].Value;
                        break;
                    case "NEUTRALIZEASSIST":
                        CoopFlagNeutralizeAsst.Text = m.Groups["value"].Value;
                        break;
                    case "DEFEND":
                        CoopDefendFlag.Text = m.Groups["value"].Value;
                        break;
                }
            }
        }

        #region Events

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // for each of the 2 scoring files, we use regex to set the values
            // Scoring Common
            string contents = File.ReadAllText(ScoringCommonPy);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_KILL = " + KillScore.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMKILL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_TEAMKILL = " + TeamKillScore.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_SUICIDE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_SUICIDE = " + SuicideScore.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_REVIVE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_REVIVE = " + ReviveScore.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMDAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", 
                Prefix + "SCORE_TEAMDAMAGE = " + TeamDamage.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_DESTROYREMOTECONTROLLED(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_DESTROYREMOTECONTROLLED = " + DestroyEnemyAsset.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_TEAMVEHICLEDAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_TEAMVEHICLEDAMAGE = " + TeamVehicleDamage.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_DRIVER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_DRIVER = " + DriverKA.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_PASSENGER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_PASSENGER = " + PassangerKA.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_TARGETER(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_TARGETER = " + TargeterKA.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_KILLASSIST_DAMAGE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_KILLASSIST_DAMAGE = " + DamageAssist.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_HEAL(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_HEAL = " + GiveHealth.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_GIVEAMMO(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_GIVEAMMO = " + GiveAmmo.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_REPAIR(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_REPAIR = " + VehicleRepair.Text, RegexOptions.Multiline);
            File.WriteAllText(ScoringCommonPy, contents);

            // Scoring Conquest
            if (!PrefixAI)
            {
                contents = File.ReadAllText(ScoringConqPy);
                contents = Regex.Replace(contents, @"SCORE_CAPTURE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_CAPTURE = " + ConqFlagCapture.Text);
                contents = Regex.Replace(contents, @"SCORE_CAPTUREASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                    "SCORE_CAPTUREASSIST = " + ConqFlagCaptureAsst.Text);
                contents = Regex.Replace(contents, @"SCORE_NEUTRALIZE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_NEUTRALIZE = " + ConqFlagNeutralize.Text);
                contents = Regex.Replace(contents, @"SCORE_NEUTRALIZEASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                    "SCORE_NEUTRALIZEASSIST = " + ConqFlagNeutralizeAsst.Text);
                contents = Regex.Replace(contents, @"SCORE_DEFEND(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)", "SCORE_DEFENT = " + ConqDefendFlag.Text);
                File.WriteAllText(ScoringConqPy, contents);
            }

            // Scoring Coop
            contents = File.ReadAllText(ScoringCoopPy);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_CAPTURE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_CAPTURE = " + CoopFlagCapture.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_CAPTUREASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_CAPTUREASSIST = " + CoopFlagCaptureAsst.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_NEUTRALIZE(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_NEUTRALIZE = " + CoopFlagNeutralize.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_NEUTRALIZEASSIST(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_NEUTRALIZEASSIST = " + CoopFlagNeutralizeAsst.Text, RegexOptions.Multiline);
            contents = Regex.Replace(contents, @"^" + Prefix + @"SCORE_DEFEND(?:[\s|\t]*)=(?:[\s|\t]*)([-]?[0-9]+)",
                Prefix + "SCORE_DEFEND = " + CoopDefendFlag.Text, RegexOptions.Multiline);
            File.WriteAllText(ScoringCoopPy, contents);

            this.Close();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Scoring Common
            KillScore.Text = "2";
            ReviveScore.Text = "2";
            DestroyEnemyAsset.Text = "1";
            GiveHealth.Text = "1";
            GiveAmmo.Text = "1";
            VehicleRepair.Text = "1";
            TeamKillScore.Text = "-4";
            TeamDamage.Text = "-2";
            TeamVehicleDamage.Text = "-1";
            SuicideScore.Text = "-2";
            DriverKA.Text = "1";
            PassangerKA.Text = "1";
            TargeterKA.Text = "0";
            DamageAssist.Text = "1";

            // Conquest
            ConqFlagCapture.Text = "2";
            ConqFlagCaptureAsst.Text = "1";
            ConqFlagNeutralize.Text = "2";
            ConqFlagNeutralizeAsst.Text = "1";
            ConqDefendFlag.Text = "1";

            // Coop
            CoopFlagCapture.Text = "2";
            CoopFlagCaptureAsst.Text = "1";
            CoopFlagNeutralize.Text = "2";
            CoopFlagNeutralizeAsst.Text = "1";
            CoopDefendFlag.Text = "1";
        }

        #endregion

        #region KeyPress Events

        private void KillScore_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ReviveScore_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DestroyEnemyAsset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GiveHealth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GiveAmmo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void VehicleRepair_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TeamKillScore_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void TeamDamage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void TeamVehicleDamage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void SuicideScore_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void DriverKA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void PassangerKA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TargeterKA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DamageAssist_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ConqFlagCapture_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ConqFlagCaptureAsst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ConqFlagNeutralize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ConqFlagNeutralizeAsst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void ConqDefendFlag_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CoopFlagCapture_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CoopFlagCaptureAsst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CoopFlagNeutralize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CoopFlagNeutralizeAsst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CoopDefendFlag_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
