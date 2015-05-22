using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Net
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up 
    /// and assigned to SocketAsyncEventArgs objects for use with each 
    /// socket I/O operation. This enables buffers to be easily reused and 
    /// guards against fragmenting heap memory.
    /// </summary>
    public class BufferManager : IDisposable
    {
        /// <summary>
        /// The total amount of bytes allocated by this buffer object
        /// </summary>
        public int BufferBlockSize 
        { 
            get { return Buffer.Length; } 
        }

        /// <summary>
        /// Our buffer object
        /// </summary>
        protected byte[] Buffer;

        /// <summary>
        /// The current running Byte offset for the next SocketAsyncEventArgs object
        /// byte block allocation
        /// </summary>
        protected int CurrentOffset = 0;

        /// <summary>
        /// The number of bytes each SocketAsyncEventArgs object gets allocated
        /// inside the Buffer for all IO operations
        /// </summary>
        protected int BytesToAllocPerEventArg;

        public BufferManager(int BufferSize, int BytesToAllocPerEventArg)
        {
            this.Buffer = new byte[BufferSize];
            this.BytesToAllocPerEventArg = BytesToAllocPerEventArg;
        }

        /// <summary>
        /// Assigns a buffer space from the buffer block to the 
        /// specified SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool AssignBuffer(SocketAsyncEventArgs args)
        {
            // Make sure we have enough room in the buffer!
            if ((BufferBlockSize - BytesToAllocPerEventArg) < CurrentOffset)
                return false;

            // Assign a new block from the buffer to this AsyncEvent object
            args.SetBuffer(Buffer, CurrentOffset, BytesToAllocPerEventArg);
            BufferDataToken Token = new BufferDataToken(CurrentOffset, BytesToAllocPerEventArg);

            // Increase the current offset for the next object
            CurrentOffset += BytesToAllocPerEventArg;

            // Set the user token property to our BufferDataToken
            args.UserToken = Token;
            return true;
        }

        /// <summary>
        /// Releases all bytes held by this buffer
        /// </summary>
        public void Dispose()
        {
            this.Buffer = null;
        }
    }
}
