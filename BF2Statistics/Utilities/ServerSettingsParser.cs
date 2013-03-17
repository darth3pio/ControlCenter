using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    /// <summary>
    /// The Server Settings Parse class is used to parse the Bf2
    /// ServerSettings.con file.
    /// </summary>
    class ServerSettingsParser
    {
        /// <summary>
        ///  A list of config items
        /// </summary>
        protected Dictionary<string, string> Items = new Dictionary<string, string>();

        public ServerSettingsParser(string FileName)
        {
            // Load the settings file
            string contents = File.ReadAllText(FileName);

            // Get all Setting Matches
            Regex Reg = new Regex(@"sv.(?:set)?(?<name>[A-Za-z]+)[\s|\t]+([""]*)(?<value>.*)(?:\1)[\n|\r|\r\n]", RegexOptions.Multiline);
            MatchCollection Matches = Reg.Matches(contents);
            
            // Add each found match to the Items Dictionary
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

            try {
                value = Items[Name];
            }
            catch (KeyNotFoundException) {
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

        /// <summary>
        /// Returns the number of found settings
        /// </summary>
        /// <returns></returns>
        public int ItemCount()
        {
            return Items.Count;
        }

        /// <summary>
        /// Returns a list of all settings
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllSettings()
        {
            return Items;
        }
    }
}
