using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics
{
    /// <summary>
    /// This class represents a Progressable report on a task that uses steps
    /// </summary>
    public sealed class TaskStep
    {
        /// <summary>
        /// Gets or Sets the ID of this step
        /// </summary>
        public int StepId = 0;

        /// <summary>
        /// Gets or Sets a description for this step
        /// </summary>
        public string Description = String.Empty;

        /// <summary>
        /// Indicates whether this task step had an error occur
        /// </summary>
        public bool IsFaulted = false;

        /// <summary>
        /// Gets or Sets the error that occured during this task step
        /// </summary>
        public Exception Error;

        /// <summary>
        /// Creates a new instance of TaskStep
        /// </summary>
        /// <param name="Step"></param>
        /// <param name="Description"></param>
        /// <param name="Faulted"></param>
        /// <param name="Error"></param>
        public TaskStep(int Step, string Description = "", bool Faulted = false, Exception Error = null)
        {
            this.StepId = Step;
            this.Description = Description;
            this.IsFaulted = Faulted;
            this.Error = Error;
        }
    }
}
