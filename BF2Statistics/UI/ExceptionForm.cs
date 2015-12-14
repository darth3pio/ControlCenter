using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RazorEngine.Templating;

namespace BF2Statistics
{
    public partial class ExceptionForm : Form
    {
        const int WS_SYSMENU = 0x80000;

        /// <summary>
        /// Gets or sets whether the details are hidden or shown when the form is displayed
        /// </summary>
        public bool DetailsExpanded
        {
            get { return panelDetails.Visible; }
            set
            {
                if (value == true)
                    panelDetails.Show();
                else
                    panelDetails.Hide();
            }
        }

        /// <summary>
        /// Gets or sets the Header text of the window
        /// </summary>
        public string HeaderText
        {
            get { return InstructionText.Text; }
            set { InstructionText.Text = value; }
        }

        /// <summary>
        /// Gets or sets the message to be displayed in the form
        /// </summary>
        public string Message
        {
            get { return labelContent.Text; }
            set { labelContent.Text = value; }
        }

        /// <summary>
        /// Gets or Sets the window title
        /// </summary>
        public string WindowTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        /// <summary>
        /// Gets or sets the Error Icon
        /// </summary>
        public Bitmap ImgIcon
        {
            get { return ErrorIcon.Image as Bitmap; }
            set { ErrorIcon.Image = value; }
        }

        /// <summary>
        /// The exception
        /// </summary>
        protected Exception ExceptionObj;

        protected string _logFile = "";
        /// <summary>
        /// Full path to the generated trace file
        /// </summary>
        public string TraceLog
        {
            get { return _logFile; }
            set
            {
                this._logFile = value;
                if (!String.IsNullOrWhiteSpace(value))
                    buttonViewLog.Show();
                else
                    buttonViewLog.Hide();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="E">The exception object</param>
        /// <param name="Recoverable">Defines if the exception is recoverable</param>
        public ExceptionForm(Exception E, bool Recoverable)
        {
            InitializeComponent();

            // Hide details at the start
            panelDetails.Hide();
            ExceptionDetails.Text = "StackTrace: " + Environment.NewLine + E.StackTrace;
            ExceptionObj = E;

            // Preform form layout based on recoverability
            if (!Recoverable)
            {
                buttonContinue.Hide();
                buttonViewLog.Location = new Point(225, 12);
            }

            // Reset log button
            TraceLog = _logFile;
        }

        new public DialogResult ShowDialog()
        {
            // Append Exception Message
            labelContent.Text = String.Concat(labelContent.Text, Environment.NewLine, Environment.NewLine, ExceptionObj.Message);
            return base.ShowDialog();
        }

        new public DialogResult ShowDialog(IWin32Window owner)
        {
            // Append Exception Message
            labelContent.Text = String.Concat(labelContent.Text, Environment.NewLine, Environment.NewLine, ExceptionObj.Message);
            return base.ShowDialog(owner);
        }

        public DialogResult ShowDialog(bool AppendExceptionMessage)
        {
            if (AppendExceptionMessage)
                return this.ShowDialog();
            else
                return base.ShowDialog();
        }

        /// <summary>
        /// Displays a custom version of this form to display database connection errors
        /// </summary>
        /// <param name="e"></param>
        public static void ShowDbConnectError(DbConnectException e)
        {
            using (ExceptionForm F = new ExceptionForm(e, true))
            {
                F.WindowTitle = "Database Connect Error";
                F.HeaderText = "Database Connect Error";
                F.Message = "Unable to establish a connection to the database.";
                F.ImgIcon = Properties.Resources.vistaWarning;
                F.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a custom version of this form to display Razor template compile errors
        /// </summary>
        public static DialogResult ShowTemplateError(TemplateCompilationException E, string TemplateFile)
        {
            using (ExceptionForm F = new ExceptionForm(E, true))
            {
                // Get our relative path from the program root
                string fileRelativePath = TemplateFile.Replace(Program.RootPath, "");

                // Set the window properties
                F.WindowTitle = "Compile Error";
                F.HeaderText = "Template Compile Error";
                F.Message = "An error occured while trying to compile the file \"" + Path.GetFileName(fileRelativePath) + "\"";
                F.ImgIcon = Properties.Resources.vistaWarning;

                if (E.CompilerErrors.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();

                    // Append each error's details into the Details stringbuilder
                    foreach (RazorEngineCompilerError error in E.CompilerErrors)
                    {
                        builder.AppendLine("Compile Error:");
                        builder.AppendLine(error.ErrorText);
                        builder.AppendLine();
                        builder.AppendLine("Error #: " + error.ErrorNumber);
                        builder.AppendLine("File: " + fileRelativePath);
                        builder.AppendLine("Line: " + error.Line);
                        builder.AppendLine("Column: " + error.Column);
                        builder.AppendLine();
                    }

                    // Set the Details pane contents
                    F.ExceptionDetails.Text = builder.ToString();
                }
                else
                {
                    F.ExceptionDetails.Text = E.Message;
                }

                return F.ShowDialog(false);
            }
        }

        /// <summary>
        /// Expands / Hides the exception details panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDetails_Click(object sender, EventArgs e)
        {
            DetailsExpanded = !panelDetails.Visible;
        }

        /// <summary>
        /// Opens the Trace Log in a text editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonViewLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(_logFile))
                Process.Start(_logFile);
            else
                MessageBox.Show("The program was unable to create the debug log!", "Error");
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
