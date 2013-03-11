using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database
{
    public enum ValueMode
    {
        Set,
        Add,
        Subtract,
        Divide,
        Multiply
    }

    public class SqlUpdateDictionary : Dictionary<string, SqlUpdateItem>
    {
        public void Add(string ColName, object Value, bool Quote, ValueMode Mode)
        {
            Add(ColName, new SqlUpdateItem(Value, Quote, Mode));
        }

        public void Add(string ColName, object Value, bool Quote)
        {
            Add(ColName, new SqlUpdateItem(Value, Quote, ValueMode.Set));
        }
    }

    public class SqlUpdateItem
    {
        public object Value = null;
        public bool Quote = true;
        public ValueMode Mode = ValueMode.Set;

        public SqlUpdateItem(object Value, bool Quote, ValueMode Mode)
        {
            this.Value = Value;
            this.Quote = Quote;
            this.Mode = Mode;
        }
    }
}
