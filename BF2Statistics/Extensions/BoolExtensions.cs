using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics
{
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts the boolean value into an int. Returns 1
        /// if the value is true, 0 otherwise
        /// </summary>
        public static int ToInt32(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
