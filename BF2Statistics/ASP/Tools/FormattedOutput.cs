using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BF2Statistics.ASP
{
    /// <summary>
    /// The HeaderDataList class is used to properly format 
    /// the official Gamespy ASP Header and Data output for
    /// Awards and player stats,
    /// </summary>
    class FormattedOutput
    {
        /// <summary>
        /// A list of header columns
        /// </summary>
        List<string> Headers;

        /// <summary>
        /// A list of rows for the Headers
        /// </summary>
        List<List<string>> Rows = new List<List<string>>();

        /// <summary>
        /// Gets or sets whether output will be tranposed
        /// (Output in an alternate, easy to read format)
        /// </summary>
        public bool Transpose = false;

        /// <summary>
        /// The size of the headers. Each header column needs a value
        /// </summary>
        public int RowSize { get; protected set; }

        /// <summary>
        /// Creates a new Instance of FormattedOutput
        /// </summary>
        /// <param name="Headers">A List of Headers</param>
        public FormattedOutput(List<string> Headers)
        {
            this.Headers = Headers;
            this.RowSize = Headers.Count;
        }

        /// <summary>
        /// Creates a new Instance of FormattedOutput
        /// </summary>
        /// <param name="Items">An Array of headers</param>
        public FormattedOutput(params object[] Items)
        {
            this.Headers = new List<string>();
            foreach (object Item in Items)
                this.Headers.Add(Item.ToString());

            this.RowSize = Headers.Count;
        }

        /// <summary>
        /// Adds a new row to the list. If some values are missing,
        /// they will be zero filled.
        /// </summary>
        /// <param name="Row"></param>
        public void AddRow(List<string> Row)
        {
            // Fill in empty values
            if (Row.Count != RowSize)
                for (int i = Row.Count; i < RowSize; i++)
                    Row.Add("0");

            Rows.Add(Row);
        }

        /// <summary>
        /// Adds a new row to the list. If some values are missing,
        /// they will be zero filled.
        /// </summary>
        /// <param name="Items"></param>
        public void AddRow(params object[] Items)
        {
            // Convert the array into a list
            List<string> Row = new List<string>();
            foreach (object Item in Items)
                Row.Add(Item.ToString());

            // Fill in empty values
            if (Row.Count != RowSize)
                for (int i = Row.Count; i < RowSize; i++)
                    Row.Add("0");

            Rows.Add(Row);
        }

        /// <summary>
        /// Converts the Headers and Data into Gamespy ASP Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Create a StringBuilder
            StringBuilder Builder = new StringBuilder();

            // Transpose is a special format used with &transpose=1
            if (Transpose)
            {
                // Start off with header... for each row, we need a \tD
                String D = "\tD";
                Builder.Append("\nH" + D.Repeat(Rows.Count));

                // Each header gets its own line, with data lines seperated by a tab
                int i = 0;
                foreach (string Header in Headers)
                {
                    string Line = "\n" + Header;
                    foreach (List<string> Row in Rows)
                        Line = String.Concat(Line, "\t", Row[i]);

                    // Append header and data line
                    Builder.Append(Line);
                    i++;
                }

                return Builder.ToString();
            }
            else
            {
                // Add Headers
                Builder.AppendFormat("\nH\t{0}", String.Join("\t", Headers));

                // Add Data
                foreach (List<string> Items in Rows)
                    Builder.AppendFormat("\nD\t{0}", String.Join("\t", Items));

                // Return lines
                return Builder.ToString();
            }
        }
    }
}
