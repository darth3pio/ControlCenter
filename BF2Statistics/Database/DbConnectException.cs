using System;

namespace BF2Statistics
{
    public class DbConnectException : Exception
    {
        public DbConnectException(string Message, Exception Inner) : base(Message, Inner) { }
    }
}
