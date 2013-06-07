using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using BF2Statistics.Properties;

namespace BF2Statistics.MedalData
{
    public partial class MedalDataEditor : Form
    {
        /// <summary>
        /// The current Medal Data file location
        /// </summary>
        public static string ConfigFile = Path.Combine(MainForm.Config.ServerPath, "python", "bf2", "BF2StatisticsConfig.py");

        /// <summary>
        /// The full path to the "stats" python folder
        /// </summary>
        public static string PythonPath = Path.Combine(MainForm.Config.ServerPath, "python", "bf2", "stats");

        /// <summary>
        /// The current selected award
        /// </summary>
        public static IAward SelectedAward;

        /// <summary>
        /// Current executing Assembly
        /// </summary>
        Assembly Me = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Our child form if applicable
        /// </summary>
        protected ConditionForm Child;

        /// <summary>
        /// TreeNode holding variable
        /// </summary>
        protected TreeNode Node;

        /// <summary>
        /// The current active profile in the BF2sConfig.py
        /// </summary>
        protected string ActiveProfile;

        /// <summary>
        /// A list of all found medal data profiles, lowercased to prevent duplicates.
        /// </summary>
        public static List<string> Profiles { get; protected set; }

        /// <summary>
        /// Indicates the last selected profile
        /// </summary>
        protected string LastSelectedProfile = null;

        /// <summary>
        /// Indicates whether changes have been made to any criteria
        /// </summary>
        public static bool ChangesMade = false;


        /// <summary>
        /// Constructor
        /// </summary>
        public MedalDataEditor()
        {
            InitializeComponent();

            // Make sure the bf2s config is present
            if (!File.Exists(ConfigFile))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("Bf2Statistics Config python file is missing! Please re-install the stats python", "Error");
                return;
            }

            // Get current active profile
            string Contents = File.ReadAllText(ConfigFile, Encoding.UTF8);
            Match M = Regex.Match(Contents, @"medals_custom_data = '(?<value>[A-Za-z0-9_]*)'");
            if (!M.Success)
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("The Bf2Statistics Config python file is corrupt. Please restore your python files.", "Config Parse Error");
                return;
            }

            // Set active profile and load the rest
            ActiveProfile = M.Groups["value"].Value;
            ChangesMade = false;
            LoadProfiles();

            // Make sure the parser is initialized
            if (!MedalDataParser.IsInitialized)
                MedalDataParser.Initialize();
        }

        #region Class Methods

        /// <summary>
        /// Scans the stats directory, and adding each found medal data profile to the profile selector
        /// </summary>
        protected void LoadProfiles()
        {
            // Clear out old junk
            ProfileSelector.Items.Clear();
            Profiles = new List<string>();

            // Load all profiles
            string[] medalList = Directory.GetFiles(PythonPath, "medal_data_*.py");
            foreach (string file in medalList)
            {
                // Remove the path to the file
                string fileF = file.Remove(0, PythonPath.Length + 1);

                // Only Add special forces ones
                if (!fileF.Contains("_xpack") || fileF == "medal_data_xpack.py")
                    continue;

                // Remove .py extension, and add it to the list of files
                fileF = fileF.Remove(fileF.Length - 3, 3).Replace("medal_data_", "").Replace("_xpack", "");
                ProfileSelector.Items.Add(fileF);
                Profiles.Add(fileF.ToLower());
            }
        }

        /// <summary>
        /// Sets the award image based on the Award ID Passed
        /// </summary>
        /// <param name="name">The Award ID</param>
        public void SetAwardImage(string name)
        {
            try {
                AwardPictureBox.Image = Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources." + name + ".jpg"));
            }
            catch {
                AwardPictureBox.Image = null;
            }
        }

        /// <summary>
        /// Brings up the Criteria Editor for an Award
        /// </summary>
        public void EditCriteria()
        {
            TreeNode Node = AwardConditionsTree.SelectedNode;

            // Make sure we have a node selected
            if (Node == null)
            {
                MessageBox.Show("Please select a criteria to edit.");
                return;
            }

            // Make sure its a child node
            if (Node.Parent == null && Node.Nodes.Count != 0)
                return;

            // Get our selected award, and set our criteria Node
            TreeNode N = AwardTree.SelectedNode;
            IAward A = AwardCache.GetAward(N.Name);
            this.Node = Node;

            // Open correct condition editor form
            if (Node.Tag is ObjectStat)
                Child = new ObjectStatForm(Node);
            else if (Node.Tag is PlayerStat)
                Child = new ScoreStatForm(Node);
            else if (Node.Tag is MedalOrRankCondition)
                Child = new MedalConditionForm(Node);
            else if (Node.Tag is GlobalStatMultTimes)
                Child = new GlobalStatMultTimesForm(Node);
            else if (Node.Tag is ConditionList)
                Child = new ConditionListForm(Node);
            else
                return;

            if (Child.ShowDialog() == DialogResult.OK)
            {
                // Delay tree redraw
                AwardConditionsTree.BeginUpdate();

                // Set awards new conditions from the tree node tagged conditions
                A.SetCondition(MedalDataParser.ParseNodeConditions(AwardConditionsTree.Nodes[0]));

                // Clear all current Nodes
                AwardConditionsTree.Nodes.Clear();

                // Reparse conditions
                AwardConditionsTree.Nodes.Add(A.ToTree());

                // Conditions tree's are to be expanded at all times
                AwardConditionsTree.ExpandAll();
                AwardConditionsTree.EndUpdate();
            }
        }

        /// <summary>
        /// Removes a criteria
        /// </summary>
        public void DeleteCriteria()
        {
            TreeNode Node = AwardConditionsTree.SelectedNode;

            // Make sure we have a node selected
            if (Node == null)
            {
                MessageBox.Show("Please select a criteria to remove.");
                return;
            }

            // Dont update tree
            AwardConditionsTree.BeginUpdate();

            if (Node.Parent == null)
            {
                AwardConditionsTree.Nodes.Remove(Node);
            }

            // Dont delete on Plus / Div Trees
            else if (!(Node.Tag is ConditionList))
            {
                TreeNode Parent = Node.Parent;
                ConditionList C = (ConditionList)Parent.Tag;

                // Remove Not Conditions, as they only hold 1 criteria anyways
                if (C.Type == ConditionType.Not)
                    AwardConditionsTree.Nodes.Remove(Parent);

                // Dont remove Plus or Div elements as they need 2 or 3 to work!
                else if (C.Type == ConditionType.Plus || C.Type == ConditionType.Div)
                {
                    AwardConditionsTree.SelectedNode = Parent;
                    EditCriteria();
                }
                else
                {
                    // Remove Node
                    AwardConditionsTree.Nodes.Remove(Node);

                    // remove empty parent nodes
                    if (Parent.Nodes.Count == 0)
                        AwardConditionsTree.Nodes.Remove(Parent);
                }
            }
            else
                AwardConditionsTree.Nodes.Remove(Node);

            // Get our selected medal
            TreeNode N = AwardTree.SelectedNode;
            IAward A = AwardCache.GetAward(N.Name);

            // Set awards new conditions
            if (AwardConditionsTree.Nodes.Count > 0)
                A.SetCondition(MedalDataParser.ParseNodeConditions(AwardConditionsTree.Nodes[0]));
            else
                A.SetCondition(null);

            // Reparse conditions
            AwardConditionsTree.Nodes.Clear();
            TreeNode NN = A.ToTree();
            if (NN != null)
                AwardConditionsTree.Nodes.Add(NN);
            AwardConditionsTree.EndUpdate();
            AwardConditionsTree.ExpandAll();
        }

        /// <summary>
        /// Returns the selected award Object
        /// </summary>
        /// <returns></returns>
        public IAward GetSelectedAward()
        {
            TreeNode N = AwardTree.SelectedNode;
            return AwardCache.GetAward(N.Name);
        }

        #endregion Class Methods

        #region New Criteria

        /// <summary>
        /// Shows the correct new criteria screen when the uesr selects which type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCriteria_Closing(object sender, FormClosingEventArgs e)
        {
            ConditionForm C = Child.GetConditionForm();
            if (C == null)
                return;

            // Hide closing New Criteria form, because setting "Child" to a new window
            // will still display the old window below it until the new child closes.
            Child.Visible = false;
            Child = C;
            Child.FormClosing += new FormClosingEventHandler(AddNewCriteria);
            Child.ShowDialog();
        }

        /// <summary>
        /// Adds a new criteria to an award from the Add Critera Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewCriteria(object sender, FormClosingEventArgs e)
        {
            TreeNode Node = AwardConditionsTree.SelectedNode;
            TreeNode AwardNode = AwardTree.SelectedNode;
            IAward Award = AwardCache.GetAward(AwardNode.Name);

            // If there is a node referenced.
            if (Node != null && Node.Tag is ConditionList)
            {
                //Condition C = (Condition)Node.Tag;
                Condition Add = Child.GetCondition();
                ConditionList List = (ConditionList)Node.Tag;
                List.Add(Add);
            }
            else
            {
                // No Node referenced... Use top most
                ConditionList A = new ConditionList(ConditionType.And);
                Condition B = Award.GetCondition();
                if (B is ConditionList)
                {
                    ConditionList C = (ConditionList)B;
                    if (C.Type == ConditionType.And)
                        A = C;
                    else
                        A.Add(B);
                }
                else
                {
                    // Add existing conditions into the condition list
                    A.Add(B);
                }

                // Add the new condition
                A.Add(Child.GetCondition());

                // Parse award conditions into tree view
                Award.SetCondition(A);
            }

            // Update the tree view
            AwardConditionsTree.BeginUpdate();
            AwardConditionsTree.Nodes.Clear();
            AwardConditionsTree.Nodes.Add(Award.ToTree());
            AwardConditionsTree.ExpandAll();
            AwardConditionsTree.EndUpdate();
        }

        #endregion New Criteria

        #region Bottom Menu

        /// <summary>
        /// This method is called upon selecting a new Medal Data Profile.
        /// It loads the new medal data, and calls the parser to parse it.
        /// </summary>
        private void ProfileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Make sure we have an index! Also make sure we didnt select the same profile again
            if (ProfileSelector.SelectedIndex == -1 || ProfileSelector.SelectedItem.ToString() == LastSelectedProfile)
                return;

            // Get current profile
            string Profile = ProfileSelector.SelectedItem.ToString();

            // Make sure the user wants to commit without saving changes
            if (ChangesMade && MessageBox.Show("Some changes have not been saved. Are you sure you want to continue?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                ProfileSelector.SelectedIndex = Profiles.IndexOf(LastSelectedProfile.ToLower());
                return;
            }

            // Disable the form to prevent errors. Show loading screen
            this.Enabled = false;
            LoadingForm.ShowScreen(this);

            // Suppress repainting the TreeView until all the objects have been created.
            AwardConditionsTree.Nodes.Clear();
            AwardTree.BeginUpdate();

            // Remove old medal data if applicable
            for(int i = 0; i <= 3; i++)
                AwardTree.Nodes[i].Nodes.Clear();

            // Get Medal Data
            try {
                MedalDataParser.LoadMedalDataFile(Path.Combine(PythonPath, "medal_data_" + Profile + "_xpack.py"));
            }
            catch (Exception ex) {
                AwardTree.EndUpdate();
                MessageBox.Show(ex.Message, "Medal Data Parse Error");
                ProfileSelector.SelectedIndex = -1;
                this.Enabled = true;
                LoadingForm.CloseForm();
                return;
            }

            // Add all awards to the corresponding Node
            foreach (Award A in AwardCache.GetBadges())
                AwardTree.Nodes[0].Nodes.Add(A.Id, A.Name);

            foreach (Award A in AwardCache.GetMedals())
                AwardTree.Nodes[1].Nodes.Add(A.Id, A.Name);

            foreach (Award A in AwardCache.GetRibbons())
                AwardTree.Nodes[2].Nodes.Add(A.Id, A.Name);

            foreach (Rank R in AwardCache.GetRanks())
                AwardTree.Nodes[3].Nodes.Add(R.Id.ToString(), R.Name);

            // Begin repainting the TreeView.
            AwardTree.CollapseAll();
            AwardTree.EndUpdate();

            // Reset current award data
            AwardNameBox.Text = null;
            AwardTypeBox.Text = null;
            AwardPictureBox.Image = null;
            AwardTree.SelectedNode = AwardTree.Nodes[0];

            // Process Active profile button
            if (Profile == ActiveProfile)
            {
                ActivateProfileBtn.Text = "Current Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.check;
            }
            else
            {
                ActivateProfileBtn.Text = "Set as Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.power;
            }

            // Enable form controls
            AwardTree.Enabled = true;
            AwardConditionsTree.Enabled = true;
            DelProfileBtn.Enabled = true;
            ActivateProfileBtn.Enabled = true;
            SaveBtn.Enabled = true;
            this.Enabled = true;
            LoadingForm.CloseForm();

            // Set this profile as the last selected profile
            LastSelectedProfile = Profile;
            ChangesMade = false;
        }

        /// <summary>
        /// Opens the dialog to create a new profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProfileBtn_Click(object sender, EventArgs e)
        {
            NewProfilePrompt Form = new NewProfilePrompt();
            if (Form.ShowDialog() == DialogResult.OK)
            {
                LoadProfiles();
                ProfileSelector.SelectedIndex = Profiles.IndexOf(NewProfilePrompt.LastProfileName);
            }
        }

        /// <summary>
        /// Deletes the selected profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProfileBtn_Click(object sender, EventArgs e)
        {
            // Always confirm
            if (MessageBox.Show("Are you sure you want to delete this medal data profile?",
                "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Make my typing easier in the future
                string Profile = ProfileSelector.SelectedItem.ToString();
                this.Enabled = false;

                try
                {
                    // Delete medal data files
                    File.Delete(Path.Combine(PythonPath, "medal_data_" + Profile + ".py"));
                    File.Delete(Path.Combine(PythonPath, "medal_data_" + Profile + "_xpack.py"));

                    // Unselect this as the default profile
                    ActiveProfile = null;

                    // Set current selected profile to null in the Bf2sConfig
                    string FileContents = File.ReadAllText(ConfigFile);
                    FileContents = Regex.Replace(FileContents, @"medals_custom_data = '([A-Za-z0-9_]*)'", "medals_custom_data = ''");
                    File.WriteAllText(ConfigFile, FileContents);

                    // Update form
                    ActivateProfileBtn.Text = "Set as Server Profile";
                    ActivateProfileBtn.BackgroundImage = Resources.power;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to delete profile medal data files!"
                        + Environment.NewLine + Environment.NewLine
                        + "Error: " + ex.Message, "Error");
                    this.Enabled = true;
                    return;
                }

                // Suppress repainting the TreeView until all the objects have been created.
                AwardConditionsTree.Nodes.Clear();
                AwardTree.BeginUpdate();

                // Remove old medal data if applicable
                for (int i = 0; i <= 3; i++)
                    AwardTree.Nodes[i].Nodes.Clear();
                AwardTree.EndUpdate();

                // Disable some form controls
                AwardTree.Enabled = false;
                AwardConditionsTree.Enabled = false;
                DelProfileBtn.Enabled = false;
                ActivateProfileBtn.Enabled = false;
                SaveBtn.Enabled = false;

                // Reset controls
                AwardNameBox.Text = null;
                AwardTypeBox.Text = null;
                AwardPictureBox.Image = null;
                ProfileSelector.SelectedIndex = -1;
                LastSelectedProfile = null;
                LoadProfiles();
                this.Enabled = true;

                // Notify User
                MessageBox.Show("Profile deleted successfully", "Success");
            }
        }

        /// <summary>
        /// Activates the current profile within the BF2StatisticsConfig.py
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivateProfileBtn_Click(object sender, EventArgs e)
        {
            if (ProfileSelector.SelectedItem.ToString() == ActiveProfile)
            {
                ActiveProfile = null;

                // Load file contents
                string FileContents = File.ReadAllText(ConfigFile);
                FileContents = Regex.Replace(FileContents,
                    @"medals_custom_data = '([A-Za-z0-9_]*)'",
                    String.Format("medals_custom_data = '{0}'", ActiveProfile)
                );

                // Save
                File.WriteAllText(ConfigFile, FileContents);

                // Update form
                ActivateProfileBtn.Text = "Set as Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.power;
            }
            else
            {
                ActiveProfile = ProfileSelector.SelectedItem.ToString();

                // Load file contents
                string FileContents = File.ReadAllText(ConfigFile);
                FileContents = Regex.Replace(FileContents,
                    @"medals_custom_data = '([A-Za-z0-9_]*)'",
                    String.Format("medals_custom_data = '{0}'", ActiveProfile)
                );

                // Save
                File.WriteAllText(ConfigFile, FileContents);

                // Update form
                ActivateProfileBtn.Text = "Current Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.check;
            }
        }

        /// <summary>
        /// Saves the medal data to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string Profile = ProfileSelector.SelectedItem.ToString();

            // Show save dialog
            SaveForm Form = new SaveForm();
            if (Form.ShowDialog() != DialogResult.OK)
                return;

            // Add base medal data functions
            StringBuilder MedalData = new StringBuilder();
            StringBuilder MedalDataSF = new StringBuilder();
            MedalData.AppendLine(Utils.GetResourceString("BF2Statistics.MedalData.PyFiles.functions.py"));
            MedalData.AppendLine("medal_data = (");

            // Add Each Award (except ranks) to the MedalData Array
            foreach (Award A in AwardCache.GetBadges())
            {
                if (!Award.IsSfAward(A.Id))
                    MedalData.AppendLine("\t" + A.ToPython());
                else
                    MedalDataSF.AppendLine("\t" + A.ToPython());
            }

            foreach (Award A in AwardCache.GetMedals())
                MedalData.AppendLine("\t" + A.ToPython());

            foreach (Award A in AwardCache.GetRibbons())
            {
                if (!Award.IsSfAward(A.Id))
                    MedalData.AppendLine("\t" + A.ToPython());
                else
                    MedalDataSF.AppendLine("\t" + A.ToPython());
            }

            // Append Rank Data
            StringBuilder RankData = new StringBuilder();
            RankData.AppendLine(")" + Environment.NewLine + "rank_data = (");
            foreach (Rank R in AwardCache.GetRanks())
                RankData.AppendLine("\t" + R.ToPython());

            // Close off the Rank Data Array
            RankData.AppendLine(")#end");

            try
            {
                // Write to the Non SF file
                File.WriteAllText(
                    Path.Combine(PythonPath, "medal_data_" + Profile + ".py"),
                    (SaveForm.IncludeSFData)
                        ? MedalData.ToString() + MedalDataSF.ToString() + RankData.ToString().TrimEnd()
                        : MedalData.ToString() + RankData.ToString().TrimEnd()
                );

                // Write to the SF file
                File.WriteAllText(
                    Path.Combine(PythonPath, "medal_data_" + Profile + "_xpack.py"),
                    MedalData.ToString() + MedalDataSF.ToString() + RankData.ToString().TrimEnd()
                );

                ChangesMade = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An exception was thrown while trying to save medal data. Medal data has NOT saved."
                        + Environment.NewLine + Environment.NewLine
                        + "Message: " + ex.Message,
                    "Error"
                );
            }
        }

        #endregion Bottom Menu

        #region Award Tree Events

        /// <summary>
        /// An event called everytime a new award is selected...
        /// It repaints the current award info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AwardTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode N = e.Node;

            // Only proccess child Nodes
            if (N.Nodes.Count == 0)
            {
                // Set Award Image
                SetAwardImage(N.Name);

                // Set award name and type
                AwardNameBox.Text = N.Text;
                switch (Award.GetType(N.Name))
                {
                    case AwardType.Badge:
                        AwardTypeBox.Text = "Badge";
                        break;
                    case AwardType.Medal:
                        AwardTypeBox.Text = "Medal";
                        break;
                    case AwardType.Ribbon:
                        AwardTypeBox.Text = "Ribbon";
                        break;
                    case AwardType.Rank:
                        AwardTypeBox.Text = "Rank";
                        break;
                }

                // Delay or tree redraw
                AwardConditionsTree.BeginUpdate();

                // Clear all Nodes
                AwardConditionsTree.Nodes.Clear();

                // Parse award conditions into tree view
                SelectedAward = AwardCache.GetAward(N.Name);
                AwardConditionsTree.Nodes.Add(SelectedAward.ToTree());

                // Conditions tree's are to be expanded at all times
                AwardConditionsTree.ExpandAll();

                // Redraw the tree
                AwardConditionsTree.EndUpdate();
            }
        }

        #endregion Award Tree Events

        #region Award Condition Tree Events

        /// <summary>
        /// Allows edit of criteria's on double mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AwardConditionsTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EditCriteria();
        }

        /// <summary>
        /// Assigns the correct Context menu options based on the selected node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AwardConditionsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode CNode = AwardConditionsTree.SelectedNode;
            if (CNode == null)
                return;

            CNode.ContextMenuStrip = (CNode.Tag is ConditionList && ((ConditionList)CNode.Tag).Type == ConditionType.And)
                ? CriteriaRootMenu
                : CriteriaItemMenu;
            //CNode.ContextMenuStrip.Show(MousePosition.X + 15, MousePosition.Y);
        }

        /// <summary>
        /// Allows enter and delete keys to edit and delete criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AwardConditionsTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EditCriteria();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                DeleteCriteria();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Disables collapsing of condition tree nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AwardConditionsTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion Award Condition Tree Events

        #region Context Menu

        /// <summary>
        /// Adding a new Criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCriteria_Click(object sender, EventArgs e)
        {
            TreeNode Node = AwardConditionsTree.SelectedNode;

            // Make sure an award is selected!!!!
            TreeNode AwardNode = AwardTree.SelectedNode;
            if (AwardNode == null || AwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Is this the root criteria node?
            if (Node == null)
            {
                Child = new NewCriteriaForm();
            }

            // If plus or div, open edit form
            else if (Node.Tag is ConditionList)
            {
                ConditionList List = Node.Tag as ConditionList;
                if (List.Type == ConditionType.Plus || List.Type == ConditionType.Div)
                    Child = new ConditionListForm(Node);
                else
                    Child = new NewCriteriaForm();
            }

            // Base Condition
            else
            {
                Child = new NewCriteriaForm();
            }

            // Show child form
            Child.FormClosing += new FormClosingEventHandler(NewCriteria_Closing);
            Child.Show();
        }

        /// <summary>
        /// Edit Criteria Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCriteria_Click(object sender, EventArgs e)
        {
            // Make sure an award is selected!!!!
            TreeNode AwardNode = AwardTree.SelectedNode;
            if (AwardNode == null || AwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            EditCriteria();
        }

        /// <summary>
        /// When the delete button is pressed, removes the criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCriteria_Click(object sender, EventArgs e)
        {
            // Make sure an award is selected!!!!
            TreeNode AwardNode = AwardTree.SelectedNode;
            if (AwardNode == null || AwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            DeleteCriteria();
        }

        /// <summary>
        /// When the Undo Changes menu option is selected, this method reverts any
        /// changes made to the current condition list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoAllChangesMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode AwardNode = AwardTree.SelectedNode;
            if (AwardNode == null || AwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Delay or tree redraw
            AwardConditionsTree.BeginUpdate();

            // Clear all Nodes
            AwardConditionsTree.Nodes.Clear();

            // Parse award conditions into tree view
            IAward SAward = AwardCache.GetAward(AwardNode.Name);
            SAward.UndoConditionChanges();
            AwardConditionsTree.Nodes.Add(SAward.ToTree());

            // Conditions tree's are to be expanded at all times
            AwardConditionsTree.ExpandAll();

            // Redraw the tree
            AwardConditionsTree.EndUpdate();

        }

        /// <summary>
        /// Restores the condition / criteria back to default (vanilla)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreToDefaultMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode AwardNode = AwardTree.SelectedNode;
            if (AwardNode == null || AwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Delay or tree redraw
            AwardConditionsTree.BeginUpdate();

            // Clear all Nodes
            AwardConditionsTree.Nodes.Clear();

            // Parse award conditions into tree view
            IAward SAward = AwardCache.GetAward(AwardNode.Name);
            SAward.RestoreDefaultConditions();
            AwardConditionsTree.Nodes.Add(SAward.ToTree());

            // Conditions tree's are to be expanded at all times
            AwardConditionsTree.ExpandAll();

            // Redraw the tree
            AwardConditionsTree.EndUpdate();
        }

        #endregion Context Menu

        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
