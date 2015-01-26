using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public partial class ObjectStatForm : ConditionForm
    {
        /// <summary>
        /// Will contain an ObjectStat condition during editing.
        /// </summary>
        protected ObjectStat Obj;

        protected TreeNode Node;

        /// <summary>
        /// Construct used when editing an object stat
        /// </summary>
        /// <param name="Obj"></param>
        public ObjectStatForm(TreeNode Node)
        {
            InitializeComponent();

            this.Node = Node;
            this.Obj = (ObjectStat) Node.Tag;
            List<String> Params = Obj.GetParams();
            int i = 0;
            int index = 0;

            switch (Params[1])
            {
                case "kits":
                    ObjectSelect.SelectedIndex = 0;
                    foreach (KeyValuePair<string, string> D in Data.KitNames)
                    {
                        TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));
                        if (D.Key == Params[3])
                            index = i;
                        i++;
                    }
                    break;
                case "weapons":
                    ObjectSelect.SelectedIndex = 1;
                    foreach (KeyValuePair<string, string> D in Data.WeaponNames)
                    {
                        TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));
                        if (D.Key == Params[3])
                            index = i;
                        i++;
                    }
                    break;
                case "vehicles":
                    ObjectSelect.SelectedIndex = 2;
                    foreach (KeyValuePair<string, string> D in Data.VehicleNames)
                    {
                        TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));
                        if (D.Key == Params[3])
                            index = i;
                        i++;
                            
                    }
                    break;
            }

            TypeSelect.SelectedIndex = index;

            // Set the Stat value
            switch (Params[2])
            {
                case "kills":
                    StatSelect.SelectedIndex = 0;
                    break;
                case "rtime":
                    StatSelect.SelectedIndex = 1;
                    break;
                case "roadKills":
                    StatSelect.SelectedIndex = 2;
                    break;
                case "deployed":
                    StatSelect.SelectedIndex = 3;
                    break;
            }

            // Set Condition Return Type
            if (Params.Count == 5)
            {
                ConditionSelect.SelectedIndex = 1;
                ValueBox.Value = Int32.Parse(Params[4]);
                ValueBox.Visible = true;
            }
            else
            {
                ConditionSelect.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Constructor used when creating a new ObjectStat
        /// </summary>
        public ObjectStatForm() 
        {
            InitializeComponent();
            this.Node = new TreeNode();
            ObjectSelect.SelectedIndex = 0;
            foreach (KeyValuePair<string, string> D in Data.KitNames)
                TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));

            TypeSelect.SelectedIndex = 0;
            StatSelect.SelectedIndex = 0;
            ConditionSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Returns the objectstat condition
        /// </summary>
        /// <returns></returns>
        public override Condition GetCondition()
        {
            return Obj;
        }

        /// <summary>
        /// Converts the data in the form into a condition, and closes the form
        /// </summary>
        public void Save()
        {
            // Tell the base condition form that we modifed the condition
            base.Canceled = false;

            // First param is always the python method name
            List<string> Params = new List<string>();
            Params.Add("object_stat");

            // 1st param
            if (ObjectSelect.SelectedIndex == 0)
                Params.Add("kits");
            else if (ObjectSelect.SelectedIndex == 1)
                Params.Add("weapons");
            else
                Params.Add("vehicles");

            // 2nd param
            if (StatSelect.SelectedIndex == 0)
                Params.Add("kills");
            else if (StatSelect.SelectedIndex == 1)
                Params.Add("rtime");
            else if (StatSelect.SelectedIndex == 2)
                Params.Add("roadKills");
            else 
                Params.Add("deployed");

            // Make sure roadKills is not selected on kits and weapons
            if (Params.Last<string>() == "roadKills" && ObjectSelect.SelectedIndex != 2)
            {
                MessageBox.Show("Only Vehicles can use the \"Road Kills\" stat.", "Error");
                ObjectSelect.Focus();
                return;
            }

            // 3rd Param
            SelectItem I = (SelectItem)TypeSelect.SelectedItem;
            Params.Add(I.Name);

            // 4th Param (optional)
            if (ValueBox.Visible)
                Params.Add(((int)ValueBox.Value).ToString());

            // Are we creating a new ObjectStat?
            if (Obj == null)
                Obj = new ObjectStat(Params);
            else
                Obj.SetParams(Params);

            // Close the form
            this.Node.Tag = Obj;
            MedalDataEditor.ChangesMade = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Used to hide the value box if the selected condition doesnt
        /// support a value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValueBox.Visible = (ConditionSelect.SelectedIndex == 1);
            Hms2SecBtn.Visible = (ConditionSelect.SelectedIndex == 1);
        }

        /// <summary>
        /// Finish button click. Calls on the Save() method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void ObjectSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Still constructing?
            if (TypeSelect.Items.Count == 0)
                return;

            // Remove current items
            TypeSelect.Items.Clear();

            // Add new items based on the selected index
            switch (ObjectSelect.SelectedIndex)
            {
                case 0:
                    foreach (KeyValuePair<string, string> D in Data.KitNames)
                        TypeSelect.Items.Add( new SelectItem(D.Key, D.Value) );
                    break;
                case 1:
                    foreach (KeyValuePair<string, string> D in Data.WeaponNames)
                        TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));
                    break;
                case 2:
                    foreach (KeyValuePair<string, string> D in Data.VehicleNames)
                        TypeSelect.Items.Add(new SelectItem(D.Key, D.Value));
                    break;
            }

            // Set a default Index for TypeSelect
            TypeSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Shows the Hours/Minutes/Seconds to Seconds form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hms2SecBtn_Click(object sender, EventArgs e)
        {
            S2hmsForm Form = new S2hmsForm();
            Form.SetTime(Int32.Parse(ValueBox.Value.ToString()));
            if (Form.ShowDialog() == DialogResult.OK)
            {
                ValueBox.Value = S2hmsForm.LastValue;
            }
        }
    }
}
