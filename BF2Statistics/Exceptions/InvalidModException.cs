using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics
{
    class InvalidModException : Exception
    {
        public InvalidModException() : base() { }

        public InvalidModException(string Message) : base(Message)  { }
    }
}
