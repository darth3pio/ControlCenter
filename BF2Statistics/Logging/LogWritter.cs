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
        private string LogFile;

        /// <summary>
        /// Our Timer object for writing to the log file
        /// </summary>
        private Timer LogTimer;
        
        public LogWritter(string LogFile)
        {
            this.LogFile = LogFile;
            LogQueue = new Queue<LogMessage>();

            // Create file if it doesnt exist
            if (!File.Exists(LogFile))
                File.Create(LogFile).Close();

            // Start a log timer, and auto write new logs every 10 seconds
            LogTimer = new Timer(10000);
            LogTimer.Elapsed += new ElapsedEventHandler(LogTimer_Elapsed);
            LogTimer.Start();
        }

        public LogWritter(string LogFile, int UpdateInterval)
        {
            this.LogFile = LogFile;
            LogQueue = new Queue<LogMessage>();

            // Create file if it doesnt exist
            if (!File.Exists(LogFile))
                File.Create(LogFile).Close();

            // Start a log timer, and auto write new logs every X seconds
            LogTimer = new Timer(UpdateInterval);
            LogTimer.Elapsed += new ElapsedEventHandler(LogTimer_Elapsed);
            LogTimer.Start();
        }

        /// <summary>
        /// Event Fired every itnerval, which flushes the log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogTimer_Elapsed(object sender, ElapsedEventArgs e)
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
                using (FileStream fs = File.Open(LogFile, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter log = new StreamWriter(fs))
                    {
                        while (LogQueue.Count > 0)
                        {
                            LogMessage entry = LogQueue.Dequeue();
                            log.WriteLine(string.Format("[{0}]\t{1}", entry.LogTime, entry.Message));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Destructor. Make sure we flush!
        /// </summary>
        ~LogWritter()
        {
            FlushLog();
        }
    }
}
