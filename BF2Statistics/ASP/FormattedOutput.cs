using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// The size of the headers. Each header column needs a value
        /// </summary>
        public int RowSize
        {
            get;
            protected set;
        }

        public FormattedOutput(List<string> Headers)
        {
            this.Headers = Headers;
            this.RowSize = Headers.Count;
        }

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
            // Return data
            StringBuilder Ret = new StringBuilder();

            // Add Headers
            Ret.AppendFormat("{0}", "H\t");
            foreach (string Item in Headers)
                Ret.AppendFormat("{0}\t", Item);

            // Get our headers in a string
            string Head = Ret.ToString().TrimEnd(new char[] { '\t' });
            Ret = new StringBuilder();

            foreach (List<string> Items in Rows)
            {
                string Row = "";
                foreach (string Item in Items)
                    Row += Item + "\t";
                
                Ret.AppendFormat("D\t{0}\n", Row.TrimEnd(new char[] { '\t' }));
            }

            string Data = Ret.ToString();
            return Head + "\n" + Data;
        }
    }
}
