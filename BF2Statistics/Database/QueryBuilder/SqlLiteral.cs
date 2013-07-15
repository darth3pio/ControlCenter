using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database.QueryBuilder
{
    class SqlLiteral
    {
        public string Value { get; protected set; }

        public SqlLiteral(string Value)
        {
            this.Value = Value;
        }
    }
}
