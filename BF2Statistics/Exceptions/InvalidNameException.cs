using System;

namespace BF2Statistics
{
    class InvalidNameException : Exception
    {
        public InvalidNameException() : base() { }

        public InvalidNameException(string Message) : base(Message)  { }
    }
}
