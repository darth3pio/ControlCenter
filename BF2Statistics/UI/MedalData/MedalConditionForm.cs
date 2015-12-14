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
    public partial class MedalConditionForm : ConditionForm
    {
        protected MedalOrRankCondition Obj;

        protected TreeNode Node;

        public MedalConditionForm(TreeNode Node)
        {
            InitializeComponent();

            this.Node = Node;
            this.Obj = (MedalOrRankCondition)Node.Tag;
            List<string> Params = Obj.GetParams();
            int I = 0;
            int Index = 0;

            // Set default award requirement type
            if (Params[0] == "has_medal")
            {
                AwardType Type = Award.GetType(Params[1]);
                switch (Type)
                {
                    case AwardType.Badge:
                        TypeSelect.SelectedIndex = 0;
                        foreach (Award A in AwardCache.GetBadges())
                        {
                            AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                            if (A.Id == Params[1])
                                Index = I;
                            I++;
                        }
                        break;
                    case AwardType.Medal:
                        TypeSelect.SelectedIndex = 1;
                        foreach (Award A in AwardCache.GetMedals())
                        {
                            AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                            if (A.Id == Params[1])
                                Index = I;
                            I++;
                        }
                        break;
                    case AwardType.Ribbon:
                        TypeSelect.SelectedIndex = 2;
                        foreach (Award A in AwardCache.GetRibbons())
                        {
                            AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                            if (A.Id == Params[1])
                                Index = I;
                            I++;
                        }
                        break;
                }
            }
            else
            {
                TypeSelect.SelectedIndex = 3;
                foreach (Rank R in AwardCache.GetRanks())
                {
                    AwardSelect.Items.Add(new KeyValuePair(R.Id.ToString(), R.Name));
                    if (R.Id.ToString() == Params[1])
                        Index = I;
                    I++;
                }

            }

            // Add index change event
            AwardSelect.SelectedIndex = Index;
            TypeSelect.SelectedIndexChanged += new EventHandler(TypeSelect_SelectedIndexChanged);
        }

        public MedalConditionForm()
        {
            InitializeComponent();
            this.Node = new TreeNode();
            TypeSelect.SelectedIndexChanged += new EventHandler(TypeSelect_SelectedIndexChanged);
            TypeSelect.SelectedIndex = 0;
            AwardSelect.SelectedIndex = 0;
        }

        public override Condition GetCondition()
        {
            return this.Obj;
        }

        void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            AwardSelect.Items.Clear();

            switch (TypeSelect.SelectedIndex)
            {
                case 0:
                    foreach (Award A in AwardCache.GetBadges())
                        AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                    break;
                case 1:
                    foreach (Award A in AwardCache.GetMedals())
                        AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                    break;
                case 2:
                    foreach (Award A in AwardCache.GetRibbons())
                        AwardSelect.Items.Add(new KeyValuePair(A.Id, A.Name));
                    break;
                case 3:
                    foreach (Rank A in AwardCache.GetRanks())
                        AwardSelect.Items.Add(new KeyValuePair(A.Id.ToString(), A.Name));
                    break;
            }

            AwardSelect.SelectedIndex = 0;
        }

        private void FinishBtn_Click(object sender, EventArgs e)
        {
            // Tell the base condition form that we modifed the condition
            base.Canceled = false;

            // First param is always the python method name
            List<string> Params = new List<string>();
            KeyValuePair I = (KeyValuePair) AwardSelect.SelectedItem;
            Params.Add( (TypeSelect.SelectedIndex < 3) ? "has_medal" : "has_rank" );
            Params.Add(I.Key);

            // Are we creating a new Object?
            if (Obj == null)
                Obj = new MedalOrRankCondition(Params);
            else
                Obj.SetParams(Params);

            // Close the form
            this.Node.Tag = Obj;
            MedalDataEditor.ChangesMade = true;
            this.DialogResult = DialogResult.OK;
        }
    }
}
