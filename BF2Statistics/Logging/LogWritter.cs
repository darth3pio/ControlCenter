using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;

namespace BF2Statistics.Logging
{
    public class LogWritter
    {
        /// <summary>
        /// Our Queue of log messages to be written
        /// </summary>
        private Queue<LogMessage> LogQueue;

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private FileInfo LogFile;

        /// <summary>
        /// Our Timer object for writing to the log file
        /// </summary>
        private Timer LogTimer;
        
        /// <summary>
        /// Creates a new Log Writter, Appending all messages to a logfile
        /// </summary>
        /// <param name="FileLocation">The location of the logfile. If the file doesnt exist,
        /// It will be created.</param>
        public LogWritter(string FileLocation) : this(FileLocation, false) { }

        /// <summary>
        /// Creates a new Log Writter instance
        /// </summary>
        /// <param name="FileLocation">The location of the logfile. If the file doesnt exist,
        /// It will be created.</param>
        /// <param name="Truncate">If set to true and the logfile is over 1MB, it will be truncated to 0 length</param>
        public LogWritter(string FileLocation, bool Truncate)
        {
            LogFile = new FileInfo(FileLocation);
            LogQueue = new Queue<LogMessage>();

            // Test that we are able to open and write to the file
            using (FileStream stream = LogFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                // If the file is over 2MB, and we want to truncate big files
                if (Truncate && LogFile.Length > 2097152)
                {
                    stream.SetLength(0);
                    stream.Flush();
                }
            }

            // Start a log timer, and auto write new logs every 3 seconds
            LogTimer = new Timer(3000);
            LogTimer.Elapsed += new ElapsedEventHandler(LogTimer_Elapsed);
            LogTimer.Start();
        }

        /// <summary>
        /// Event Fired every itnerval, which flushes the log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FlushLog();
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message)
        {
            // Lock the queue while writing to prevent contention for the log file
            LogMessage logEntry = new LogMessage(message);
            lock (LogQueue)
            {
                // Push to the Queue
                LogQueue.Enqueue(logEntry);
            }
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message, params object[] items)
        {
            Write(String.Format(message, items));
        }

        /// <summary>
        /// Flushes the Queue to the physical log file
        /// </summary>
        private void FlushLog()
        {
            // Only log if we have a queue
            if (LogQueue.Count > 0)
            {
                using (FileStream fs = LogFile.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter log = new StreamWriter(fs))
                {
                    while (LogQueue.Count > 0)
                    {
                        LogMessage entry = LogQueue.Dequeue();
                        log.WriteLine(String.Format("[{0}]\t{1}", entry.LogTime, entry.Message));
                    }
                }
            }
        }

        /// <summary>
        /// Destructor. Make sure we flush!
        /// </summary>
        ~LogWritter()
        {
            LogTimer.Stop();
            LogTimer.Dispose();
            FlushLog();
        }
    }
}
