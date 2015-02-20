using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace BF2Statistics.Logging
{
    /// <summary>
    /// Provides an object wrapper for a file that is used to
    /// store LogMessage's into. Uses a Multi-Thread safe Queueing
    /// system, and provides full Asynchronous writing and flushing
    /// </summary>
    public class LogWritter
    {
        /// <summary>
        /// Our Queue of log messages to be written, Thread safe
        /// </summary>
        private ConcurrentQueue<LogMessage> LogQueue;

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private FileInfo LogFile;

        /// <summary>
        /// Our Timer object for writing to the log file
        /// </summary>
        private Timer LogTimer;

        /// <summary>
        /// Indicates whether this log file is currently writing
        /// </summary>
        private bool IsFlushing = false;

        /// <summary>
        /// Creates a new Log Writter instance
        /// </summary>
        /// <param name="FileLocation">The location of the logfile. If the file doesnt exist,
        /// It will be created.</param>
        /// <param name="Truncate">If set to true and the logfile is over XX size, it will be truncated to 0 length</param>
        /// <param name="TruncateLen">
        ///     If <paramref name="Truncate"/> is true, The size of the file must be at least this size, 
        ///     in bytes, to truncate it
        /// </param>
        public LogWritter(string FileLocation, bool Truncate = false, int TruncateLen = 2097152)
        {
            // Set internals
            LogFile = new FileInfo(FileLocation);
            LogQueue = new ConcurrentQueue<LogMessage>();

            // Test that we are able to open and write to the file
            using (FileStream stream = LogFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // If the file is over 2MB, and we want to truncate big files
                if (Truncate && LogFile.Length > TruncateLen)
                {
                    stream.SetLength(0);
                    stream.Flush();
                }
            }

            // Start a log timer, and auto write new logs every 3 seconds
            LogTimer = new Timer(3000);
            LogTimer.Elapsed += (s, e) => FlushLog();
            LogTimer.Start();
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message)
        {
            // Push to the Queue
            LogQueue.Enqueue(new LogMessage(message));
        }

        /// <summary>
        /// Adds a message to the queue, to be written to the log file
        /// </summary>
        /// <param name="message">The message to write to the log</param>
        public void Write(string message, params object[] items)
        {
            LogQueue.Enqueue(new LogMessage(String.Format(message, items)));
        }

        /// <summary>
        /// Flushes the Queue to the physical log file
        /// </summary>
        private async void FlushLog()
        {
            // Only log if we have a queue
            if (!IsFlushing && LogQueue.Count > 0)
            {
                // WE are flushing
                IsFlushing = true;

                // Wrap this in a task, to fire in a threadpool
                await Task.Run(async () =>
                {
                    // Append messages
                    using (FileStream fs = LogFile.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (LogQueue.Count > 0)
                        {
                            LogMessage entry;
                            if (LogQueue.TryDequeue(out entry))
                                await writer.WriteLineAsync(String.Format("[{0}]\t{1}", entry.LogTime, entry.Message));
                        }
                    }
                });

                // Done
                IsFlushing = false;
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
