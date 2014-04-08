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
    public class ServerSettings
    {
        /// <summary>
        ///  A list of config items
        /// </summary>
        protected Dictionary<string, string> Items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Full path to our ServerSettings.con file
        /// </summary>
        private FileInfo SettingsFile;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName">The full path to the settings.con file</param>
        public ServerSettings(string FileName)
        {
            // Load the settings file
            SettingsFile = new FileInfo(FileName);
            if(!SettingsFile.Exists)
                throw new Exception("Server settings file does not exist!");

            string contents;

            // Try to open the settings file with READ/WRITE access
            try
            {
                using (Stream Str = SettingsFile.Open(FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader Rdr = new StreamReader(Str))
                    contents = Rdr.ReadToEnd();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to Read/Write to the settings file: " + e.Message, e);
            }

            // Get all Setting Matches
            Regex Reg = new Regex(@"sv.(?:set)?(?<name>[A-Za-z]+)[\s|\t]+([""]*)(?<value>.*)(?:\1)");
            MatchCollection Matches = Reg.Matches(contents);
            
            // Add each found match to the Items Dictionary
            foreach (Match m in Matches)
                Items.Add(m.Groups["name"].Value, m.Groups["value"].Value);
        }

        /// <summary>
        /// Method for returning the string value of a settings item
        /// </summary>
        /// <param name="Name">The name of the config item</param>
        /// <returns>The value of the settings item</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the named item doesnt exist</exception>
        public string GetValue(string Name)
        {
            try {
                return Items[Name];
            }
            catch (KeyNotFoundException) {
                MainForm.Log("Settings Parser: Item not found \"{0}\"", Name);
                throw;
            }
        }

        /// <summary>
        /// Method for returning the string value of a settings item
        /// </summary>
        /// <param name="Name">The name of the config item</param>
        /// <param name="DefaultValue">The default value to return if the item doesnt exist</param>
        /// <returns>The value of the settings item</returns>
        public string GetValue(string Name, string DefaultValue)
        {
            return (Items.ContainsKey(Name)) ? Items[Name] : DefaultValue;
        }

        /// <summary>
        /// Sets the value for a settings item
        /// </summary>
        /// <param name="Name">Item Name</param>
        /// <param name="Value">String value of the item</param>
        public void SetValue(string Name, string Value)
        {
            if (!Items.ContainsKey(Name))
                Items.Add(Name, Value);
            else
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
        /// Saves the current settings to the Server Settings file
        /// </summary>
        public void Save()
        {
            string[] lines = new string[Items.Count];
            int i = 0;
            int dummy;

            // Write the lines one by one into an array
            foreach (KeyValuePair<string, string> Item in Items)
            {
                string Value = Item.Value.Trim();

                // Determine if the value is a string or number. Strings need wrapped in quotes
                if (!String.IsNullOrEmpty(Value) && Int32.TryParse(Value, out dummy))
                    lines[i] = String.Format("sv.{0} {1}", Item.Key, Value);
                else
                    lines[i] = String.Format("sv.{0} \"{1}\"", Item.Key, Value.Replace(Environment.NewLine, "|"));

                i++;
            }

            // Save the file
            using (Stream Str = SettingsFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(String.Join(Environment.NewLine, lines));
                Wtr.Flush();
            }
        }
    }
}
