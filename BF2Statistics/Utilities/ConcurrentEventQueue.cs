using System;
using System.Collections.Concurrent;

namespace BF2Statistics
{
    /// <summary>
    /// Represents a thread-safe first in-first out (FIFO) collection.
    /// </summary>
    class ConcurrentEventQueue<T> : ConcurrentQueue<T>
    {
        /// <summary>
        /// Event fired when an item is queued
        /// </summary>
        public event EventHandler ItemEnqueued;

        /// <summary>
        /// Event fired when an item is Dequeued
        /// </summary>
        public event EventHandler ItemDequeued;

        /// <summary>
        /// Adds an object to the end of the ConcurrentQueue<T>
        /// </summary>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnEnqueue();
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        public new bool TryDequeue(out T item)
        {
            if(base.TryDequeue(out item))
            {
                OnDequeue();
                return true;
            }

            return false;
        }

        protected virtual void OnEnqueue()
        {
            if (ItemEnqueued != null)
                ItemEnqueued(this, EventArgs.Empty);
        }

        protected virtual void OnDequeue()
        {
            if (ItemDequeued != null)
                ItemDequeued(this, EventArgs.Empty);
        }
    }
}
