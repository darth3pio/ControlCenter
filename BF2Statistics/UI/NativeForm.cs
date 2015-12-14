using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a window or dialog box that that can natively enable or disable itself.
    /// </summary>
    public partial class NativeForm : Form
    {
        const int GWL_STYLE = -16;
        const int WS_DISABLED = 0x08000000;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Natively disables the current form window. Using this method disables / Enables the
        /// current form without disabling the underlying controls, thus preventing the form's controls
        /// from going grey.
        /// </summary>
        /// <remarks>
        ///     By disabling this form natively, using Form.Show(this) on a new form allows it to 
        ///     display just like a dialog modal (Ex: Form.ShowDialog(this)), but without blocking the thread
        /// </remarks>
        /// <param name="enabled">Indicates whether this form is Enabled (true) or Disabled (false)</param>
        protected void SetNativeEnabled(bool enabled)
        {
            SetWindowLong(
                Handle,
                GWL_STYLE,
                GetWindowLong(Handle, GWL_STYLE) & ~WS_DISABLED | (enabled ? 0 : WS_DISABLED)
            );
        }
    }
}
