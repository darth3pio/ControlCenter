using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class Award : IAward
    {
        /// <summary>
        /// The Award ID
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// The award string name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The Conition (or ConditionList) to get said award in-game
        /// </summary>
        public Condition Conditions { get; protected set; }

        /// <summary>
        /// The original Conition (or ConditionList) to get said award in-game
        /// </summary>
        private Condition OrigConditions;

        /// <summary>
        /// The string ID of the award
        /// </summary>
        protected string StrId;

        /// <summary>
        /// The award type
        /// </summary>
        protected int Type;

        #region Non-Static Members

        /// <summary>
        /// Class Constructor. Constructs a new award
        /// </summary>
        /// <param name="AwardId">The award ID</param>
        public Award(string AwardId, string StrId, string Type, Condition Condition)
        {
            // Throw an exception if the award is non-existant
            if (!Exists(AwardId))
                throw new Exception("Award Doesnt Exist! " + AwardId);

            // Set award vars
            this.Id = AwardId;
            this.StrId = StrId;
            this.Name = GetName(AwardId);
            this.Type = Int32.Parse(Type);
            this.Conditions = Condition;
            this.OrigConditions = (Condition)Condition.Clone();
        }

        public Award() { }

        /// <summary>
        /// Sets the Condition, or Condition list to earn the award
        /// </summary>
        /// <param name="C">The condition or condition list</param>
        public void SetCondition(Condition C) 
        {
            this.Conditions = C;
        }

        /// <summary>
        /// Returns the awards conditions
        /// </summary>
        /// <returns></returns>
        public Condition GetCondition()
        {
            return Conditions;
        }

        /// <summary>
        /// Restores any changes made to the conditions of this award
        /// </summary>
        /// <returns></returns>
        public void UndoConditionChanges()
        {
            Conditions = (Condition)OrigConditions.Clone();
        }

        /// <summary>
        /// Restores the condition of this award to the default (vanilla) state
        /// </summary>
        public void RestoreDefaultConditions()
        {
            Conditions = AwardCache.GetDefaultAwardCondition(this.Id);
        }

        /// <summary>
        /// Returns the name of the award
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Converts the medal data, and its conditions to python
        /// </summary>
        /// <returns></returns>
        public string ToPython()
        {
            return (Conditions == null) 
                ? null 
                : String.Format("('{0}', '{1}', {2}, {3}),#stop", Id, StrId, Type, Conditions.ToPython());
        }

        /// <summary>
        /// Converts the medal's conditions to a TreeView
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTree()
        {
            return (Conditions == null) ? null : Conditions.ToTree();
        }

        #endregion Non-Static Members

        #region StaticMembers

        /// <summary>
        /// Returns the Award type of an award.
        /// </summary>
        /// <param name="AwardId">The award ID</param>
        /// <returns>AwardType enumeration</returns>
        public static AwardType GetType(string AwardId)
        {
            // Badges always have an underscore
            if (AwardId.Contains('_'))
                return AwardType.Badge;
            
            // Contert to int
            int id = Int32.Parse(AwardId);
            if (id < 22)
                return AwardType.Rank;

            return (id > 3000000) ? AwardType.Ribbon : AwardType.Medal;
        }

        /// <summary>
        /// Returns the badge level
        /// </summary>
        /// <param name="BadgeId">The award ID</param>
        /// <returns>BadgeLevel enumeration</returns>
        public static BadgeLevel GetBadgeLevel(string BadgeId)
        {
            // Badges always have an underscore
            if (!BadgeId.Contains('_'))
                throw new Exception("");

            string[] parts = BadgeId.Split('_');
            switch (parts[1])
            {
                case "1":
                    return BadgeLevel.Bronze;
                case "2":
                    return BadgeLevel.Silver;
                case "3":
                    return BadgeLevel.Gold;
            }

            return BadgeLevel.Bronze;
        }

        /// <summary>
        /// Returns the full string name of an award
        /// </summary>
        /// <param name="AwardId">The award ID</param>
        /// <returns></returns>
        public static string GetName(string AwardId)
        {
            // Badges always have an underscore
            if (AwardId.Contains('_'))
            {
                // Make sure the award exists!
                string[] parts = AwardId.Split('_');
                if (!Awards.ContainsKey(parts[0]))
                    throw new Exception();

                switch (parts[1])
                {
                    case "1":
                        return "Basic " + Awards[parts[0]];
                    case "2":
                        return "Veteran " + Awards[parts[0]];
                    case "3":
                        return "Expert " + Awards[parts[0]];
                }

                return null;
            }

            // Make sure the award exists
            if (!Awards.ContainsKey(AwardId))
                throw new Exception();

            return Awards[AwardId];
        }

        /// <summary>
        /// Returns whether a specified award ID exists
        /// </summary>
        /// <param name="AwardId">The Award ID</param>
        /// <returns></returns>
        public static bool Exists(string AwardId)
        {
            if (AwardId.Contains('_'))
            {
                string[] parts = AwardId.Split('_');
                return Awards.ContainsKey(parts[0]);
            }
            return Awards.ContainsKey(AwardId);
        }

        /// <summary>
        /// Indicates whether the award is a special forces award
        /// </summary>
        /// <param name="AwardId"></param>
        /// <returns></returns>
        public static bool IsSfAward(string AwardId)
        {
            // Badges always have an underscore
            if (AwardId.Contains('_'))
            {
                string[] parts = AwardId.Split('_');
                AwardId = parts[0];
            }

            // Contert to int
            int id = Int32.Parse(AwardId);
            return ((id > 1260000 && id < 2100000) || (id > 3260000 && id < 6000000));
        }

        #endregion StaticMembers

        /// <summary>
        /// Awards list.. At the bottom because of the size :S
        /// </summary>
        public static Dictionary<string, string> Awards = new Dictionary<string, string>()
        {
            // Badges
            {"1031119", "Assault Combat Badge"},
            {"1031120", "Anti-Tank Combat Badge"},
            {"1031109", "Sniper Combat Badge"},
            {"1031115", "Spec-Ops Combat Badge"},
            {"1031121", "Support Combat Badge"},
            {"1031105", "Engineer Combat Badge"},
            {"1031113", "Medic Combat Badge"},
            {"1031406", "Knife Combat Badge"},
            {"1031619", "Pistol Combat Badge"},
            {"1032415", "Explosives Ordinance Badge"},
            {"1190601", "First Aid Badge"},
            {"1190507", "Engineer Badge"},
            {"1191819", "Resupply Badge"},
            {"1190304", "Command Badge"},
            {"1220118", "Armour Badge"},
            {"1222016", "Transport Badge"},
            {"1220803", "Helicopter Badge"},
            {"1220122", "Aviator Badge"},
            {"1220104", "Air Defense Badge"},
            {"1031923", "Ground Defense Badge"},

            // SF Badges
            {"1261119", "Assault Specialist Badge"},
            {"1261120", "Anti-Tank Specialist Badge"},
            {"1261109", "Sniper Specialist Badge"},
            {"1261115", "Spec-Ops Specialist Badge"},
            {"1261121", "Support Specialist Badge"},
            {"1261105", "Engineer Specialist Badge"},
            {"1261113", "Medic Specialist Badge"},
            {"1260602", "Tactical Support Weaponry Badge"},
            {"1260708", "Grappling Hook Specialist Badge"},
            {"1262612", "Zip Line Specialist Badge"},

            // Medals
            {"2191608", "Purple Heart"},
            {"2191319", "Meritorious Service Medal"},
            {"2190303", "Combat Action Medal"},
            {"2190309", "Air Combat Medal"},
            {"2190318", "Armour Combat Medal"},
            {"2190308", "Helecopter Combat Medal"},
            {"2190703", "Good Conduct Medal"},
            {"2020903", "Combat Infantry Medal"},
            {"2020913", "Marksman Infantry Medal"},
            {"2020919", "Sharpshooter Infantry Medal"},
            {"2021322", "Medal of Valour"},
            {"2020419", "Distinguished Service Medal"},

            // Ribbons
            {"3240301", "Combat Action Ribbon"},
            {"3211305", "Meritorious Unit Ribbon"},
            {"3150914", "Infantry Officer Ribbon"},
            {"3151920", "Staff Officer Ribbon"},
            {"3190409", "Distinguished Service Ribbon"},
            {"3242303", "War College Ribbon"},
            {"3212201", "Valorous Unit Ribbon"},
            {"3241213", "Legion of Merit Ribbon"},
            {"3190318", "Crew Service Ribbon"},
            {"3190118", "Armoured Service Ribbon"},
            {"3190105", "Aerial Service Ribbon"},
            {"3190803", "Helicopter Service Ribbon"},
            {"3040109", "Air Defense Ribbon"},
            {"3040718", "Ground Defense Ribbon"},
            {"3240102", "Airborne Ribbon"},
            {"3240703", "Good Conduct Ribbon"},

            // SF Ribbons
            {"3260318", "Crew Specialist Ribbon"},
            {"3260118", "Armored Transport Specialist Ribbon"},
            {"3260105", "Airborne Specialist Service Ribbon"},
            {"3260803", "Helo Specialist Ribbon"},

            // Cant be earned
            {"6666666", "Smoc Award. Awarded From ASP"},
            {"6666667", "General Award. Awarded From ASP"}
        };
    }
}
