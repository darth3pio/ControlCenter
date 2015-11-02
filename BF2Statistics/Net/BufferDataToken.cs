using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Net
{
    public class BufferDataToken
    {
        /// <summary>
        /// The offset to the allocated buffer, for the associated SocketAsyncEventArgs
        /// object, in the BufferManager
        /// </summary>
        public readonly int BufferOffset;

        /// <summary>
        /// The buffer length inside the buffer manager assinged for this SocketAsyncEventArgs
        /// </summary>
        public readonly int BufferBlockSize;

        /// <summary>
        /// Creates a new instance of BufferDataToken
        /// </summary>
        /// <param name="BufferOffset">The offest in the Buffer block allocated to this object</param>
        /// <param name="BlockSize">The total size in the buffer allocated to this object</param>
        public BufferDataToken(int BufferOffset, int BlockSize)
        {
            this.BufferBlockSize = BlockSize;
            this.BufferOffset = BufferOffset;
        }
    }
}
