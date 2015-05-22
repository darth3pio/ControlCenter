using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace BF2Statistics.Net
{
    /// <summary>
    /// This class represents a thread safe pool of SocketAsyncEventArgs objects
    /// with the specifed capacity
    /// </summary>
    public class SocketAsyncEventArgsPool
    {
        // Pool of reusable SocketAsyncEventArgs objects.        
        private Stack<SocketAsyncEventArgs> pool;

        /// <summary>
        /// Initializes the object pool with the specified size
        /// </summary>
        /// <param name="capacity">Initial capacity of objects</param>
        public SocketAsyncEventArgsPool(int capacity)
        {
            this.pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// Indicates the number of SocketAsyncEventArgs instances in the pool
        /// </summary>
        public int Count
        {
            get { return this.pool.Count; }
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs instance from the pool.
        /// </summary>
        /// <returns>A SocketAsyncEventArgs removed from the pool</returns>
        public SocketAsyncEventArgs Pop()
        {
            lock (this.pool)
                return this.pool.Pop();
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool. 
        /// </summary>
        /// <param name="item">A SocketAsyncEventArgs instance to add to the pool</param>
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (this.pool)
                this.pool.Push(item);
        }
    }
}
