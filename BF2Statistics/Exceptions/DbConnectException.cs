using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics
{
    public class DbConnectException : Exception
    {
        public DbConnectException(string Message, Exception Inner) : base(Message, Inner) { }
    }
}
