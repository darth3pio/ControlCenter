using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    public static class Validator
    {
        /// <summary>
        /// Determines if the givin string is numberic, and contains only numbers
        /// </summary>
        /// <param name="line">The string to be checked</param>
        /// <returns>True is there is only numbers in the string</returns>
        public static bool IsNumeric(string line)
        {
            return Regex.IsMatch(line, @"^[0-9]+$");
        }

        /// <summary>
        /// Determines if the given string can be converted to a float
        /// </summary>
        /// <param name="line">The string to be checked</param>
        /// <returns>Returns true if the string can be converted to a float</returns>
        public static bool IsFloat(string line)
        {
            return Regex.IsMatch(line, @"^[0-9]*(?:\.[0-9]*)?$");
        }

        /// <summary>
        /// This method determines if the given string contains characters only.
        /// Will return false if any numbers are in the string
        /// </summary>
        /// <param name="line">The string to be checked</param>
        /// <returns></returns>
        public static bool IsAlphaOnly(string line)
        {
            return Regex.IsMatch(line, @"^[a-z\s\t\n\r\n]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// This method returns whether a provided email address is valid
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            try {
                var addr = new MailAddress(email);
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool IsValidPID(string pid)
        {
            return Regex.IsMatch(pid, @"^[0-9]{8,9}$");
        }

        #region BF2sConfig

        public static bool IsValidClanTag(string line)
        {
            return Regex.IsMatch(line, @"^[a-z0-9_=-\|\s\[\]]*$", RegexOptions.IgnoreCase);
        }

        public static bool IsValidPrefix(string line)
        {
            return Regex.IsMatch(line, @"^[a-z0-9._=-\[\]]*$", RegexOptions.IgnoreCase);
        }

        #endregion
    }
}
