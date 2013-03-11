using System;
using System.Runtime.InteropServices;

namespace BF2Statistics.Gamespy
{
    public static class GamespyUtils
    {
        [DllImport("GamespyUtils.dll")]
        public static extern string EncodePassword(string str);

        [DllImport("GamespyUtils.dll")]
        public static extern string DecodePassword(string str);
    }
}
