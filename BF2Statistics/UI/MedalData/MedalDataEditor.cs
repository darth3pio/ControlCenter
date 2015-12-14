using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.Properties;

namespace BF2Statistics.MedalData
{
    public partial class MedalDataEditor : Form
    {
        /// <summary>
        /// The full path to the "stats" python folder
        /// </summary>
        public static string PythonPath = Path.Combine(BF2Server.RootPath, "python", "bf2", "stats");

        /// <summary>
        /// The current selected award
        /// </summary>
        public static IAward SelectedAward;

        /// <summary>
        /// The current selected award node
        /// </summary>
        protected TreeNode SelectedAwardNode;

        /// <summary>
        /// The highlighted Award condition node (if any)
        /// </summary>
        protected TreeNode ConditionNode;

        /// <summary>
        /// Our child form if applicable
        /// </summary>
        protected ConditionForm Child;

        /// <summary>
        /// A list of all found medal data profiles, lowercased to prevent duplicates.
        /// </summary>
        public static List<string> Profiles { get; protected set; }

        /// <summary>
        /// The current active profile in the BF2sConfig.py
        /// </summary>
        protected string ActiveProfile;

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
            if (!StatsPython.Installed)
            {
                this.Load += (s, e) => this.Close();
                MessageBox.Show("Stats need to be enabled before using the medal data editor.", "Error");
                return;
            }

            // Set active profile and load the rest
            ActiveProfile = StatsPython.Config.MedalDataProfile;
            ChangesMade = false;
            LoadProfiles();
        }

        #region Class Methods

        /// <summary>
        /// Scans the stats directory, and adds each found medal data profile to the profile selector
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

                // Remove .py extension, and add it to the list of files
                fileF = fileF.Remove(fileF.Length - 3, 3).Replace("medal_data_", "");
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
            try 
            {
                // Dispose old image
                if(AwardPictureBox.Image != null)
                    AwardPictureBox.Image.Dispose();

                AwardPictureBox.Image = Image.FromStream(Program.GetResource("BF2Statistics.Resources." + name + ".jpg"));
            }
            catch 
            {
                AwardPictureBox.Image = null;
            }
        }

        /// <summary>
        /// Brings up the Criteria Editor for the selected Award Condition Node
        /// </summary>
        public void EditCriteria()
        {
            // Make sure we have a node selected
            if (ConditionNode == null)
            {
                MessageBox.Show("Please select a criteria to edit.");
                return;
            }

            // Make sure its a child node
            if (ConditionNode.Parent == null && ConditionNode.Nodes.Count != 0)
                return;

            // Open correct condition editor form
            if (ConditionNode.Tag is ObjectStat)
                Child = new ObjectStatForm(ConditionNode);
            else if (ConditionNode.Tag is PlayerStat)
                Child = new ScoreStatForm(ConditionNode);
            else if (ConditionNode.Tag is MedalOrRankCondition)
                Child = new MedalConditionForm(ConditionNode);
            else if (ConditionNode.Tag is GlobalStatMultTimes)
                Child = new GlobalStatMultTimesForm(ConditionNode);
            else if (ConditionNode.Tag is ConditionList)
                Child = new ConditionListForm(ConditionNode);
            else
                return;

            if (Child.ShowDialog() == DialogResult.OK)
            {
                // Delay tree redraw
                AwardConditionsTree.BeginUpdate();

                // Set awards new conditions from the tree node tagged conditions
                SelectedAward.SetCondition(MedalDataParser.ParseNodeConditions(AwardConditionsTree.Nodes[0]));

                // Clear all current Nodes
                AwardConditionsTree.Nodes.Clear();

                // Reparse conditions
                AwardConditionsTree.Nodes.Add(SelectedAward.ToTree());

                // Validation highlighting
                ValidateConditions(SelectedAwardNode, SelectedAward.GetCondition());

                // Conditions tree's are to be expanded at all times
                AwardConditionsTree.ExpandAll();
                AwardConditionsTree.EndUpdate();
            }
        }

        /// <summary>
        /// Removes a criteria from the selected awards condition tree
        /// </summary>
        public void DeleteCriteria()
        {
            // Make sure we have a node selected
            if (ConditionNode == null)
            {
                MessageBox.Show("Please select a criteria to remove.");
                return;
            }

            // Dont update tree till we are finished adding/removing
            AwardConditionsTree.BeginUpdate();

            if (ConditionNode.Parent == null)
            {
                AwardConditionsTree.Nodes.Remove(ConditionNode);
                ValidateConditions(SelectedAwardNode, SelectedAward.GetCondition());
            }

            // Dont delete on Plus / Div Trees
            else if (!(ConditionNode.Tag is ConditionList))
            {
                TreeNode Parent = ConditionNode.Parent;
                ConditionList C = (ConditionList)Parent.Tag;

                // Remove Not Conditions, as they only hold 1 criteria anyways
                if (C.Type == ConditionType.Not)
                {
                    AwardConditionsTree.Nodes.Remove(Parent);
                }

                // Dont remove Plus or Div elements as they need 2 or 3 to work!
                else if (C.Type == ConditionType.Plus || C.Type == ConditionType.Div)
                {
                    ConditionNode = Parent;
                    AwardConditionsTree.SelectedNode = Parent;
                    EditCriteria();
                }
                else
                {
                    // Remove Node
                    AwardConditionsTree.Nodes.Remove(ConditionNode);

                    // remove empty parent nodes
                    if (Parent.Nodes.Count == 0)
                        AwardConditionsTree.Nodes.Remove(Parent);
                }
            }
            else
                AwardConditionsTree.Nodes.Remove(ConditionNode);

            // Get our selected medal condition
            Condition Cond = null;

            // Set awards new conditions
            if (AwardConditionsTree.Nodes.Count > 0)
            {
                Cond = MedalDataParser.ParseNodeConditions(AwardConditionsTree.Nodes[0]);
                SelectedAward.SetCondition(Cond);
            }
            else
                SelectedAward.SetCondition(null);

            // Reparse conditions
            AwardConditionsTree.Nodes.Clear();
            TreeNode NN = SelectedAward.ToTree();
            if (NN != null)
                AwardConditionsTree.Nodes.Add(NN);

            ValidateConditions(SelectedAwardNode, Cond);
            AwardConditionsTree.EndUpdate();
            AwardConditionsTree.ExpandAll();
        }

        /// <summary>
        /// This method is used to highlight the invalid condition nodes
        /// of the current selected AwardNode, to let the user know that the
        /// conditions selected wont work properly when loaded by the server
        /// </summary>
        /// <remarks>Applies highlighting from the Award Node => to => The Root Node</remarks>
        protected void ValidateConditions(TreeNode AwardNode, Condition Con)
        {
            // Color the Conditions texts to show invalid condition returns
            if (Con != null)
            {
                // Condition lists
                if (Con is ConditionList)
                {
                    if ((Con as ConditionList).HasConditionErrors)
                        AwardNode.ForeColor = Color.Red;
                    else
                        AwardNode.ForeColor = Color.Black;

                    // Double check that our condition list returns a bool
                    if (Con.Returns() != ReturnType.Bool)
                    {
                        // The root must always return a bool
                        AwardNode.ForeColor = Color.Red;
                        if (AwardConditionsTree.Nodes.Count > 0)
                            AwardConditionsTree.Nodes[0].ForeColor = Color.Red;
                    }
                }

                // Any kind of condition, Root must always 
                else if (Con.Returns() != ReturnType.Bool)
                {
                    // The root must always return a bool
                    AwardNode.ForeColor = Color.Red;
                    if (AwardConditionsTree.Nodes.Count > 0)
                        AwardConditionsTree.Nodes[0].ForeColor = Color.Red;
                }
                else
                {
                    // All is well
                    AwardNode.ForeColor = Color.Black;
                    if(AwardConditionsTree.Nodes.Count > 0)
                        AwardConditionsTree.Nodes[0].ForeColor = Color.Black;
                }
            }
            else
            {
                // A null award is bad
                AwardNode.ForeColor = Color.Red;
            }

            // Now we also highlight all the parent nodes highlight as well
            TreeNode qq = AwardNode;
            while (qq.Parent != null)
            {
                qq = qq.Parent;
                qq.ForeColor = AwardNode.ForeColor;
            }
        }

        /// <summary>
        /// This method is used to highlight the invalid condition nodes
        /// in the conditions treeview, as to let the user know that the
        /// conditions selected wont work properly when loaded by the server
        /// </summary>
        /// <remarks>Applies highlighting from the Root Nodes => to => The Award Nodes</remarks>
        private void ValidateConditions()
        {
            // Apply highlighting of badges
            TreeNode BadgeNode = AwardTree.Nodes[0];
            for (int i = 0; i < BadgeNode.Nodes.Count; i++)
            {
                int j = 0;
                foreach (TreeNode N in BadgeNode.Nodes[i].Nodes)
                {
                    // Fetch the award and its condition
                    Condition Cond = AwardCache.GetAward(N.Name).GetCondition();
                    if (Cond is ConditionList)
                    {
                        ConditionList Clist = Cond as ConditionList;
                        if (Clist.HasConditionErrors)
                        {
                            // Top level node
                            BadgeNode.ForeColor = Color.Red;
                            // Badge Node
                            BadgeNode.Nodes[i].ForeColor = Color.Red;
                            // Badege Level Node
                            BadgeNode.Nodes[i].Nodes[j].ForeColor = Color.Red;
                        }
                    }
                    // Make sure that this condition returns a bool
                    else if (Cond.Returns() != ReturnType.Bool)
                    {
                        // Top level node
                        BadgeNode.ForeColor = Color.Red;
                        // Badge Node
                        BadgeNode.Nodes[i].ForeColor = Color.Red;
                        // Badege Level Node
                        BadgeNode.Nodes[i].Nodes[j].ForeColor = Color.Red;
                    }
                    j++;
                }
            }

            // Apply highlighting for the rest of the awards
            for (int i = 1; i < 4; i++)
            {
                int j = 0;
                foreach (TreeNode N in AwardTree.Nodes[i].Nodes)
                {
                    // Fetch the award and its condition
                    Condition Cond = AwardCache.GetAward(N.Name).GetCondition();
                    if (Cond is ConditionList)
                    {
                        ConditionList Clist = Cond as ConditionList;
                        if (Clist.HasConditionErrors)
                        {
                            // Top level Node
                            AwardTree.Nodes[i].ForeColor = Color.Red;
                            // Award Node
                            AwardTree.Nodes[i].Nodes[j].ForeColor = Color.Red;
                        }
                    }
                    // Make sure that this condition returns a bool
                    else if (Cond.Returns() != ReturnType.Bool)
                    {
                        // Top level Node
                        AwardTree.Nodes[i].ForeColor = Color.Red;
                        // Award Node
                        AwardTree.Nodes[i].Nodes[j].ForeColor = Color.Red;
                    }
                    j++;
                }
            }
        }

        #endregion Class Methods

        #region New Criteria

        /// <summary>
        /// Shows the correct new criteria screen when the uesr selects which type.
        /// </summary>
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
        private void AddNewCriteria(object sender, FormClosingEventArgs e)
        {
            // If there is a node referenced.
            if (ConditionNode != null && ConditionNode.Tag is ConditionList)
            {
                Condition Add = Child.GetCondition();
                ConditionList List = (ConditionList)ConditionNode.Tag;
                List.Add(Add);
            }
            else
            {
                // No Node referenced... Use top most
                ConditionList A = new ConditionList(ConditionType.And);
                Condition B = SelectedAward.GetCondition();
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
                SelectedAward.SetCondition(A);
            }

            // Update the tree view
            AwardConditionsTree.BeginUpdate();
            AwardConditionsTree.Nodes.Clear();
            AwardConditionsTree.Nodes.Add(SelectedAward.ToTree());
            ValidateConditions(SelectedAwardNode, SelectedAward.GetCondition());
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
            for (int i = 0; i < 4; i++)
            {
                AwardTree.Nodes[i].Nodes.Clear();
                AwardTree.Nodes[i].ForeColor = Color.Black;
            }

            // Get Medal Data
            try 
            {
                MedalDataParser.LoadMedalDataFile(Path.Combine(PythonPath, "medal_data_" + Profile + ".py"));
            }
            catch (Exception ex) 
            {
                AwardTree.EndUpdate();
                MessageBox.Show(ex.Message, "Medal Data Parse Error");
                ProfileSelector.SelectedIndex = -1;
                this.Enabled = true;
                LoadingForm.CloseForm();
                return;
            }

            // Iterator for badges
            int itr = -1;

            // Add all awards to the corresponding Node
            foreach (Award A in AwardCache.GetBadges())
            {
                if(Award.GetBadgeLevel(A.Id) == BadgeLevel.Bronze)
                {
                    AwardTree.Nodes[0].Nodes.Add(A.Id, A.Name.Replace("Basic ", ""));
                    ++itr;
                }

                AwardTree.Nodes[0].Nodes[itr].Nodes.Add(A.Id, A.Name.Split(' ')[0]);
            }

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

            // Apply inital highlighting of condition nodes
            ValidateConditions();

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
        private void NewProfileBtn_Click(object sender, EventArgs e)
        {
            using (NewProfilePrompt Form = new NewProfilePrompt())
            {
                if (Form.ShowDialog() == DialogResult.OK)
                {
                    LoadProfiles();
                    ProfileSelector.SelectedIndex = Profiles.IndexOf(NewProfilePrompt.LastProfileName);
                }
            }
        }

        /// <summary>
        /// Deletes the selected profile
        /// </summary>
        private void DelProfileBtn_Click(object sender, EventArgs e)
        {
            // Always confirm
            if (MessageBox.Show("Are you sure you want to delete this medal data profile?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Make my typing easier in the future
                string Profile = ProfileSelector.SelectedItem.ToString();
                this.Enabled = false;

                try
                {
                    // Delete medal data files
                    File.Delete(Path.Combine(PythonPath, "medal_data_" + Profile + ".py"));

                    // Set current selected profile to null in the Bf2sConfig
                    if (StatsPython.Config.MedalDataProfile == Profile)
                    {
                        // Unselect this as the default profile
                        ActiveProfile = null;
                        StatsPython.Config.MedalDataProfile = "";
                        StatsPython.Config.Save();
                    }

                    // Update form
                    ActivateProfileBtn.Text = "Set as Server Profile";
                    ActivateProfileBtn.BackgroundImage = Resources.power;
                }
                catch (Exception ex)
                {
                    Notify.Show("Unable to Delete Profile Medaldata Files!", "Error: " + ex.Message, AlertType.Warning);
                    this.Enabled = true;
                    return;
                }

                // Suppress repainting the TreeView until all the objects have been created.
                AwardConditionsTree.Nodes.Clear();
                AwardTree.BeginUpdate();

                // Remove old medal data if applicable
                for (int i = 0; i < 4; i++)
                {
                    AwardTree.Nodes[i].Nodes.Clear();
                    AwardTree.Nodes[i].ForeColor = Color.Black;
                }
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
                Notify.Show("Medal Data Profile Deleted Successfully.", "Operation Successful", AlertType.Success);
            }
        }

        /// <summary>
        /// Activates the current profile within the BF2StatisticsConfig.py
        /// </summary>
        private void ActivateProfileBtn_Click(object sender, EventArgs e)
        {
            if (ProfileSelector.SelectedItem.ToString() == ActiveProfile)
            {
                ActiveProfile = null;

                // save medal data profile
                StatsPython.Config.MedalDataProfile = "";
                StatsPython.Config.Save();

                // Update form
                ActivateProfileBtn.Text = "Set as Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.power;
            }
            else
            {
                ActiveProfile = ProfileSelector.SelectedItem.ToString();

                // save medal data profile
                StatsPython.Config.MedalDataProfile = ActiveProfile;
                StatsPython.Config.Save();

                // Update form
                ActivateProfileBtn.Text = "Current Server Profile";
                ActivateProfileBtn.BackgroundImage = Resources.check;
            }
        }

        /// <summary>
        /// Saves the medal data to a file
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // ============ Check For Condition Errors and Alert User of Any
            string Profile = ProfileSelector.SelectedItem.ToString();
            int CondErrors = 0;

            // Check for condition errors on badges
            TreeNode BadgeNode = AwardTree.Nodes[0];
            for (int i = 0; i < BadgeNode.Nodes.Count; i++)
            {
                foreach (TreeNode N in BadgeNode.Nodes[i].Nodes)
                {
                    IAward t = AwardCache.GetAward(N.Name);
                    Condition Cond = t.GetCondition();
                    if (Cond is ConditionList)
                    {
                        ConditionList Clist = Cond as ConditionList;
                        if (Clist.HasConditionErrors)
                            CondErrors++;
                    }
                    else if (Cond.Returns() != ReturnType.Bool)
                    {
                        CondErrors++;
                    }
                }
            }

            // Check for condition errors on the rest of the awards
            for (int i = 1; i < 4; i++)
            {
                foreach (TreeNode N in AwardTree.Nodes[i].Nodes)
                {
                    IAward t = AwardCache.GetAward(N.Name);
                    Condition Cond = t.GetCondition();
                    if (Cond is ConditionList)
                    {
                        ConditionList Clist = Cond as ConditionList;
                        if (Clist.HasConditionErrors)
                            CondErrors++;
                    }
                    else if (Cond.Returns() != ReturnType.Bool)
                    {
                        CondErrors++;
                    }
                }
            }

            // Warn the user of any condition errors, and verify if we wanna save anyways
            if (CondErrors > 0)
            {
                DialogResult Res = MessageBox.Show(
                    "A total of " + CondErrors + " award condition errors were found."
                    + Environment.NewLine + Environment.NewLine
                    + "Are you sure you want to save these changes without fixing these issues?",
                    "Condition Errors Found", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                );

                if (Res != DialogResult.Yes)
                    return;
            }

            // ============ Begin applying Medal Data Update

            // Add base medal data functions
            StringBuilder MedalData = new StringBuilder();
            MedalData.AppendLine(Program.GetResourceAsString("BF2Statistics.MedalData.PyFiles.functions.py"));
            MedalData.AppendLine("medal_data = (");

            // Add Each Award (except ranks) to the MedalData Array
            foreach (Award A in AwardCache.GetBadges())
                MedalData.AppendLine("\t" + A.ToPython());

            foreach (Award A in AwardCache.GetMedals())
                MedalData.AppendLine("\t" + A.ToPython());

            foreach (Award A in AwardCache.GetRibbons())
                MedalData.AppendLine("\t" + A.ToPython());

            // Append Rank Data
            MedalData.AppendLine(")");
            MedalData.AppendLine("rank_data = (");
            foreach (Rank R in AwardCache.GetRanks())
                MedalData.AppendLine("\t" + R.ToPython());

            // Close off the Rank Data Array
            MedalData.AppendLine(")#end");


            // ============ Save Medal Data
            try
            {
                // Write to file
                File.WriteAllText(
                    Path.Combine(PythonPath, "medal_data_" + Profile + ".py"),
                    MedalData.ToString().TrimEnd()
                );

                // Update variables, and display success
                ChangesMade = false;
                Notify.Show("Medal Data Saved Successfully!", "Operation Successful", AlertType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An exception was thrown while trying to save medal data. Medal data has NOT saved."
                        + Environment.NewLine + Environment.NewLine
                        + "Message: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion Bottom Menu

        #region Award Tree Events

        /// <summary>
        /// An event called everytime a new award is selected...
        /// It repaints the current award info
        /// </summary>
        private void AwardTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Set our award globally
            SelectedAwardNode = e.Node;

            // Only proccess child Nodes
            if (SelectedAwardNode.Nodes.Count == 0)
            {
                // Set Award Image
                SetAwardImage(SelectedAwardNode.Name);

                // Set award name and type
                switch (Award.GetType(SelectedAwardNode.Name))
                {
                    case AwardType.Badge:
                        AwardTypeBox.Text = "Badge";
                        AwardNameBox.Text = Award.GetName(SelectedAwardNode.Name);
                        break;
                    case AwardType.Medal:
                        AwardTypeBox.Text = "Medal";
                        AwardNameBox.Text = Award.GetName(SelectedAwardNode.Name);
                        break;
                    case AwardType.Ribbon:
                        AwardTypeBox.Text = "Ribbon";
                        AwardNameBox.Text = Award.GetName(SelectedAwardNode.Name);
                        break;
                    case AwardType.Rank:
                        AwardTypeBox.Text = "Rank";
                        AwardNameBox.Text = Rank.GetName(Int32.Parse(SelectedAwardNode.Name));
                        break;
                }

                // Delay or tree redraw
                AwardConditionsTree.BeginUpdate();

                // Clear all Nodes
                AwardConditionsTree.Nodes.Clear();

                // Parse award conditions into tree view
                SelectedAward = AwardCache.GetAward(SelectedAwardNode.Name);
                TreeNode Conds = SelectedAward.ToTree();
                if(Conds != null)
                    AwardConditionsTree.Nodes.Add(Conds);

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
        private void AwardConditionsTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EditCriteria();
        }

        /// <summary>
        /// Assigns the correct Context menu options based on the selected node
        /// </summary>
        private void AwardConditionsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ConditionNode = AwardConditionsTree.SelectedNode;
            if (ConditionNode == null)
                return;

            ConditionNode.ContextMenuStrip = (ConditionNode.Tag is ConditionList && ((ConditionList)ConditionNode.Tag).Type == ConditionType.And)
                ? CriteriaRootMenu
                : CriteriaItemMenu;
        }

        /// <summary>
        /// Allows enter and delete keys to edit and delete criteria
        /// </summary>
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
        private void AwardConditionsTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion Award Condition Tree Events

        #region Context Menu

        /// <summary>
        /// Adding a new Criteria
        /// </summary>
        private void NewCriteriaMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure an award is selected!!!!
            if (SelectedAwardNode == null || SelectedAwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Is this the root criteria node?
            TreeNode Node = AwardConditionsTree.SelectedNode;
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
            Child.FormClosing += NewCriteria_Closing;
            Child.Show();
        }

        /// <summary>
        /// Edit Criteria option Click
        /// </summary>
        private void EditCriteriaMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure an award is selected!!!!
            if (SelectedAwardNode == null || SelectedAwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            EditCriteria();
        }

        /// <summary>
        /// When the delete option is pressed, removes the criteria
        /// </summary>
        private void DeleteCriteriaMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure an award is selected!!!!
            if (SelectedAwardNode == null || SelectedAwardNode.Parent == null)
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
        private void UndoAllChangesMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an award selected
            if (SelectedAwardNode == null || SelectedAwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Delay or tree redraw
            AwardConditionsTree.BeginUpdate();

            // Clear all Nodes
            AwardConditionsTree.Nodes.Clear();

            // Parse award conditions into tree view
            IAward SAward = AwardCache.GetAward(SelectedAwardNode.Name);
            SAward.UndoConditionChanges();
            AwardConditionsTree.Nodes.Add(SAward.ToTree());

            // Revalidate
            ValidateConditions(SelectedAwardNode, SAward.GetCondition());

            // Conditions tree's are to be expanded at all times
            AwardConditionsTree.ExpandAll();

            // Redraw the tree
            AwardConditionsTree.EndUpdate();
        }

        /// <summary>
        /// Restores the condition / criteria back to default (vanilla)
        /// </summary>
        private void RestoreToDefaultMenuItem_Click(object sender, EventArgs e)
        {
            // Make sure we have an award selected
            if (SelectedAwardNode == null || SelectedAwardNode.Parent == null)
            {
                MessageBox.Show("Please select an award!");
                return;
            }

            // Delay or tree redraw
            AwardConditionsTree.BeginUpdate();

            // Clear all Nodes
            AwardConditionsTree.Nodes.Clear();

            // Parse award conditions into tree view
            IAward SAward = AwardCache.GetAward(SelectedAwardNode.Name);
            SAward.RestoreDefaultConditions();
            AwardConditionsTree.Nodes.Add(SAward.ToTree());

            // Revalidate
            ValidateConditions(SelectedAwardNode, SAward.GetCondition());

            // Conditions tree's are to be expanded at all times
            AwardConditionsTree.ExpandAll();

            // Redraw the tree
            AwardConditionsTree.EndUpdate();
        }

        #endregion Context Menu
    }
}
