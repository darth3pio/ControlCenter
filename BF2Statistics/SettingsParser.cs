using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    class SettingsParser
    {
        /// <summary>
        ///  A list of config items
        /// </summary>
        public Dictionary<string, string> Items { get; protected set; }

        public SettingsParser(string FileName)
        {
            Items = new Dictionary<string, string>();

            string contents = File.ReadAllText(FileName);

            Regex Reg = new Regex(@"sv.(?:set)?(?<name>[A-Za-z]+)[\s|\t]+([""]*)(?<value>.*)(?:\1)[\n|\r|\r\n]", RegexOptions.Multiline);
            MatchCollection Matches = Reg.Matches(contents);
            

            foreach (Match m in Matches)
                Items.Add(m.Groups["name"].Value, m.Groups["value"].Value);

        }

        /// <summary>
        /// Method for returning the string value of a settings item
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>The value, or Null if unset</returns>
        public string GetValue(string Name)
        {
            string value;

            try
            {
                value = Items[Name];
            }
            catch (KeyNotFoundException)
            {
                MainForm.Log("Settings Parser: Item not found \"{0}\"", Name);
                value = null;
            }

            return value;
        }

        /// <summary>
        /// Sets the value for a settings item
        /// </summary>
        /// <param name="Name">Item Name</param>
        /// <param name="Value">String value of the item</param>
        public void SetValue(string Name, string Value)
        {
            Items[Name] = Value;
        }
    }
}
