using System;

namespace BF2Statistics
{
    class InvalidMapException : Exception
    {
        public InvalidMapException() : base() { }

        public InvalidMapException(string Message) : base(Message)  { }

        public InvalidMapException(string Message, Exception Inner) : base(Message, Inner) { }
    }
}
