using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BF2Statistics.Logging
{
    class LogMessage
    {
        public string Message;
        public int LogTimestamp { get; protected set; }
        public DateTime LogTime { get; protected set; }

        public LogMessage(string Message)
        {
            this.Message = Message;
            this.LogTimestamp = DateTime.UtcNow.ToUnixTimestamp();
            this.LogTime = DateTime.Now;
        }

        public LogMessage(string Message, params object[] Items)
        {
            this.Message = String.Format(Message, Items);
            this.LogTimestamp = DateTime.UtcNow.ToUnixTimestamp();
            this.LogTime = DateTime.Now;
        }
    }
}
