using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BF2Statistics.Gamespy
{
    public static class GamespyUtils
    {
        /*
        [DllImport("GamespyUtils.dll")]
        public static extern string EncodePassword(string str);

        [DllImport("GamespyUtils.dll")]
        public static extern string DecodePassword(string str);
         * */

        
        public static string EncodePassword(string Password)
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.UseShellExecute = false;
            Info.CreateNoWindow = true;
            Info.RedirectStandardOutput = true;
            Info.Arguments = string.Format("e \"{0}\"", Password);
            Info.FileName = Path.Combine(MainForm.Root, "gspassenc.exe");

            Process gsProcess = Process.Start(Info);
            return gsProcess.StandardOutput.ReadToEnd();
        }

        public static string DecodePassword(string Password)
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.UseShellExecute = false;
            Info.CreateNoWindow = true;
            Info.RedirectStandardOutput = true;
            Info.Arguments = string.Format("d \"{0}\"", Password);
            Info.FileName = Path.Combine(MainForm.Root, "gspassenc.exe");

            Process gsProcess = Process.Start(Info);
            return gsProcess.StandardOutput.ReadToEnd();
        }
        
    }
}
