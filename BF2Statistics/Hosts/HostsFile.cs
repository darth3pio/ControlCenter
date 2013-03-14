using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.AccessControl;

namespace BF2Statistics
{
    class HostsFile
    {
        /// <summary>
        /// Direct Filepath to the hosts file
        /// </summary>
        public static readonly string FilePath = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");

        /// <summary>
        /// Hosts file security object
        /// </summary>
        protected FileSecurity Fs;

        /// <summary>
        /// Each line of the hosts file stored in a list. ALl redirects are removed
        /// from this list before being stored here.
        /// </summary>
        public List<string> OrigContents;

        /// <summary>
        /// A list of "hostname" => "IPAddress" in the hosts file.
        /// </summary>
        public Dictionary<string, string> Lines;

        public HostsFile()
        {
            // Get the hosts file access control
            Fs = File.GetAccessControl(FilePath);

            // Make sure we can read the file amd write to it!
            try {
                UnLock(); // Unlock
                OrigContents = new List<string>(File.ReadAllLines(FilePath));
            }
            catch (Exception e) {
                MainForm.ErrorLog.Write("Unable to READ the HOST file! " + e.Message);
                string message = "Unable to READ the HOST file! Please make sure this program is being ran as an administrator, or "
                    + "modify your HOSTS file permissions, allowing this program to read/modify it."
                    + Environment.NewLine + Environment.NewLine
                    + "Error Message: " + e.Message;
                throw new Exception(message);
            }

            // Check that we can write to the hosts file
            FileStream Stream;
            try
            {
                Stream = File.Open(FilePath, FileMode.Open, FileAccess.Write);
                if (!Stream.CanWrite)
                {
                    Stream.Close();
                    throw new Exception("Hosts file cannot be written to!");
                }
            }
            catch
            {
                string message = "HOSTS file is not WRITABLE! Please make sure this program is being ran as an administrator, or "
                    + "modify your HOSTS file permissions, allowing this program to read/modify it.";
                throw new Exception(message);
            }

            Stream.Close();

            // Backup our current HOSTS content for later replacement
            Backup();
        }

        public void UnLock()
        {
            // Allow ReadData
            Fs.RemoveAccessRule(new FileSystemAccessRule("users", FileSystemRights.ReadData, AccessControlType.Deny));
            Fs.AddAccessRule(new FileSystemAccessRule("users", FileSystemRights.ReadData, AccessControlType.Allow));
            File.SetAccessControl(FilePath, Fs);
        }

        public void Lock()
        {
            Fs.RemoveAccessRule(new FileSystemAccessRule("users", FileSystemRights.ReadData, AccessControlType.Allow));
            Fs.AddAccessRule(new FileSystemAccessRule("users", FileSystemRights.ReadData, AccessControlType.Deny));
            File.SetAccessControl(FilePath, Fs);
        }


        /// <summary>
        /// Adds lines to the hosts file
        /// </summary>
        /// <param name="lines">An array of [hostname, IP Address]</param>
        public void AppendLines(Dictionary<string, string> add)
        {
            try
            {
                // First, add the lines
                foreach (KeyValuePair<String, String> line in add)
                {
                    if (Lines.ContainsKey(line.Key))
                    {
                        Lines[line.Key] = line.Value;
                        continue;
                    }

                    Lines.Add(line.Key, line.Value);
                }

                // Convert the dictionary of lines to a list of lines
                List<string> lns = new List<string>();
                foreach (KeyValuePair<String, String> line in Lines)
                    lns.Add( String.Format("{0}\t{1}", line.Value, line.Key) );

                File.WriteAllLines(FilePath, lns);
            }
            catch (Exception e)
            {
                MainForm.ErrorLog.Write("Error writing to hosts file! Reason: " + e.Message);
            }
        }

        /// <summary>
        /// Grabs all the data in the hosts file for later restoration
        /// </summary>
        private void Backup()
        {
            Lines = new Dictionary<string, string>();
            foreach (string line in OrigContents)
            {
                // Dont add empty lines or comments
                string cLine = line.Trim();
                if (String.IsNullOrWhiteSpace(cLine) || cLine.StartsWith("#"))
                    continue;

                // Add line if we have a valid address and hostname
                Match M = Regex.Match(cLine,
                    @"^([\s|\t]+)?(?<address>[a-z0-9\.:]+)[\s|\t]+(?<hostname>[a-z0-9\.\-_\s]+)$", 
                    RegexOptions.IgnoreCase);
                if (M.Success)
                    Lines.Add(M.Groups["hostname"].Value.ToLower().Trim(), M.Groups["address"].Value.Trim());
            }

            // Remove old dirty redirects from the Backup
            for (int i = OrigContents.Count - 1; i >= 0; i--)
            {
                if (OrigContents[i].Contains("bf2web.gamespy.com"))
                    OrigContents.RemoveAt(i);
                else if (OrigContents[i].Contains("gpcm.gamespy.com"))
                    OrigContents.RemoveAt(i);
                else if (OrigContents[i].Contains("gpsp.gamespy.com"))
                    OrigContents.RemoveAt(i);
            }

            // Make sure we have a localhost loopback!
            if (!Lines.ContainsKey("localhost"))
            {
                OrigContents.Add("127.0.0.1\tlocalhost");
                Lines.Add("localhost", System.Net.IPAddress.Loopback.ToString());
            }
        }

        /// <summary>
        /// Restores the HOSTS file original contents, also removing the redirects
        /// </summary>
        public void Revert()
        {
            try
            {
                File.WriteAllLines(FilePath, OrigContents);
            }
            catch (Exception e)
            {
                MainForm.ErrorLog.Write("Error writing to hosts file! Reason: " + e.Message);
                throw e;
            }
        }
    }
}
