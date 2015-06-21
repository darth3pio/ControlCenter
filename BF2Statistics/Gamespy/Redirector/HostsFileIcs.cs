using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Gamespy.Redirector
{
    class HostsFileIcs : HostsFileAbstract
    {
        /// <summary>
        /// Direct Filepath to the hosts file
        /// </summary>
        public static string FilePath = Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts.ics");

        /// <summary>
        /// If CanRead or CanWrite are false, the exception that was thrown
        /// will be stored here
        /// </summary>
        public static Exception LastException { get; protected set; }

        public HostsFileIcs() : base()
        {
            // We dont know?
            try
            {
                // Get the Hosts file object
                HostFile = new FileInfo(FilePath);

                // If HOSTS file is readonly, remove that attribute!
                if (HostFile.IsReadOnly)
                {
                    try
                    {
                        HostFile.IsReadOnly = false;
                    }
                    catch (Exception e)
                    {
                        Program.ErrorLog.Write("HOSTS.ics file is READONLY, Attribute cannot be removed: " + e.Message);
                        LastException = e;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("Program cannot access HOSTS.ics file in any way: " + e.Message);
                LastException = e;
                return;
            }
        }
    }
}
