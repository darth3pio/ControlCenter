using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BF2Statistics
{
    public partial class UpdateProgressForm : Form
    {
        private const int WS_SYSMENU = 0x80000;

        /// <summary>
        /// Our isntance of the update form
        /// </summary>
        private static UpdateProgressForm Instance;

        /// <summary>
        /// Text alignment
        /// </summary>
        public static HorizontalAlignment TextAlign = HorizontalAlignment.Center;

        /// <summary>
        /// Delegate for cross thread call to close
        /// </summary>
        private delegate void CloseDelegate();

        /// <summary>
        /// Delegate for cross thread call to update text
        /// </summary>
        private delegate void UpdateStatus();

        /// <summary>
        /// Temorary holder for update text
        /// </summary>
        private static string UpdateText;

        /// <summary>
        /// Main calling method. Opens a new instance of the form, and displays it
        /// </summary>
        /// <param name="WindowTitle"></param>
        static public void ShowScreen(string WindowTitle)
        {
            // Make sure it is currently not open and running.
            if (Instance != null && !Instance.IsDisposed)
                return;

            Instance = new UpdateProgressForm(WindowTitle);
            Thread thread = new Thread(new ThreadStart(UpdateProgressForm.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Thread.Sleep(100); // Wait for Run to work
        }

        /// <summary>
        /// Updates the status message on the form
        /// </summary>
        /// <param name="Message"></param>
        public static void Status(string Message)
        {
            try
            {
                UpdateText = Message;
                Instance.Invoke(new UpdateStatus(UpdateProgressForm.UpdateStatusMessage));
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Method called to close the update form
        /// </summary>
        public static void CloseForm()
        {
            Instance.Invoke(new CloseDelegate(UpdateProgressForm.CloseFormInternal));
        }

        /// <summary>
        /// Threaded method. Runs the form application
        /// </summary>
        private static void ShowForm()
        {
            Application.Run(Instance);
        }

        /// <summary>
        /// Method called from delegate, to close the form
        /// </summary>
        private static void CloseFormInternal()
        {
            Instance.Close();
        }

        /// <summary>
        /// Main form constructor
        /// </summary>
        /// <param name="WindowTitle"></param>
        private UpdateProgressForm(string WindowTitle)
        {
            InitializeComponent();
            this.Text = WindowTitle;
        }

        /// <summary>
        /// Updates the status messages
        /// </summary>
        private static void UpdateStatusMessage()
        {
            // Update Progress Text
            Instance.StatusText.Text = UpdateText;

            // Set the status text color to black instead of grey
            Instance.StatusText.SelectAll();
            Instance.StatusText.SelectionColor = Color.Black;

            // Align Text, and remove selection
            Instance.StatusText.SelectionAlignment = TextAlign;
            Instance.StatusText.SelectionLength = 0;

            // Push Update
            Instance.StatusText.Update();
            Instance.ProgBar.Focus();
        }

        /// <summary>
        /// Hides the Close, Minimize, and Maximize buttons
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }
    }
}
