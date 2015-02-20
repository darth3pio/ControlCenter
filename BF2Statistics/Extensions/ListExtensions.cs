using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics
{
    static class ListExtensions
    {
        /// <summary>
        /// Shuffles the list's item's in random order
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp/1262619#1262619</remarks>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
