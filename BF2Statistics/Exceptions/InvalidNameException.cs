using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics
{
    class InvalidNameException : Exception
    {
        public InvalidNameException() : base() { }

        public InvalidNameException(string Message) : base(Message)  { }
    }
}
