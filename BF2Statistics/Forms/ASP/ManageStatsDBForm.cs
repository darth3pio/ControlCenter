using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using BF2Statistics.Database;
using BF2Statistics.Web;
using FolderSelect;
using System.Threading.Tasks;

namespace BF2Statistics
{
    public partial class ManageStatsDBForm : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ManageStatsDBForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dumps the sql into executable sql files
        /// </summary>
        private void ImportSqlBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "Sql File (*.sql)|*.sql|All Files|*.*";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                string SqlFile = Dialog.FileName;
            }
        }

        /// <summary>
        /// Imports ASP created BAK files (Mysql Out FILE)
        /// </summary>
        private async void ImportASPBtn_Click(object sender, EventArgs e)
        {
            // Open File Select Dialog
            FolderSelectDialog Dialog = new FolderSelectDialog();
            Dialog.Title = "Select ASP Database Backup Folder";
            Dialog.InitialDirectory = Path.Combine(Paths.DocumentsFolder, "Database Backups");
            if (Dialog.ShowDialog())
            {
                // Get files list from path
                string path = Dialog.SelectedPath;
                string[] BakFiles = Directory.GetFiles(path, "*.bak");
                if (BakFiles.Length > 0)
                {
                    // Open the database connection
                    StatsDatabase Database = null;
                    try {
                        Database = new StatsDatabase();
                    }
                    catch (Exception Ex)
                    {
                        if (Ex is DbConnectException)
                        {
                            ExceptionForm.ShowDbConnectError(Ex as DbConnectException);
                            return;
                        }

                        MessageBox.Show(
                            "Unable to connect to database\r\n\r\nMessage: " + Ex.Message,
                            "Database Connection Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                        return;
                    }
                    finally
                    {
                        if (Database == null)
                        {
                            // Stop the ASP server, and close this form
                            HttpServer.Stop();
                            this.Close();
                        }
                    }

                    // Show task dialog
                    TaskForm.Show(this, "Importing Stats", "Importing ASP Stats Bak Files...", false);
                    this.Enabled = false;

                    // Don't block the GUI
                    await Task.Run(() => ImportFromBakup(BakFiles, Database));

                    // Alert user and close task form
                    Notify.Show("Stats imported successfully!", "Operation Successful", AlertType.Success);
                    TaskForm.CloseForm();
                    this.Enabled = true;

                    // Displose Connection
                    Database.Dispose();
                }
                else
                {
                    // Alert the user and tell them they failed
                    MessageBox.Show(
                        "Unable to locate any .bak files in this folder. Please select an ASP created database backup folder that contains backup files.", 
                        "Backup Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// This method imports a list of .Bak files into the database
        /// </summary>
        /// <param name="BakFiles">A list of Backfiles to import into the database</param>
        /// <param name="Database">The opened database connection</param>
        private void ImportFromBakup(string[] BakFiles, StatsDatabase Database)
        {
            // Clear old database records
            TaskForm.Progress.Report(new TaskProgressUpdate("Removing old stats data"));
            Database.Truncate();

            // Let the database update itself
            Thread.Sleep(500);

            // Begin transaction
            using (DbTransaction Transaction = Database.BeginTransaction())
            {
                // import each table
                foreach (string file in BakFiles)
                {
                    // Get table name
                    string table = Path.GetFileNameWithoutExtension(file);
                    TaskForm.Progress.Report(new TaskProgressUpdate("Processing stats table: " + table));

                    // Import table data
                    try
                    {
                        // Sqlite kinda sucks... no import methods
                        if (Database.DatabaseEngine == DatabaseEngine.Sqlite)
                        {
                            string[] Lines = File.ReadAllLines(file);
                            foreach (string line in Lines)
                            {
                                string[] Values = line.Split('\t');
                                Database.Execute(
                                    String.Format("INSERT INTO {0} VALUES({1})", table, "\"" + String.Join("\", \"", Values) + "\"")
                                );
                            }
                        }
                        else
                            Database.Execute(String.Format("LOAD DATA LOCAL INFILE '{0}' INTO TABLE {1};", file.Replace('\\', '/'), table));
                    }
                    catch (Exception Ex)
                    {
                        // Show exception error
                        using (ExceptionForm Form = new ExceptionForm(Ex, false))
                        {
                            Form.Message = String.Format("Failed to import data into table {0}!{2}{2}Error: {1}", table, Ex.Message, Environment.NewLine);
                            DialogResult Result = Form.ShowDialog();

                            // Rollback!
                            TaskForm.Progress.Report(new TaskProgressUpdate("Rolling back stats data"));
                            Transaction.Rollback();
                            return;
                        }
                    }
                }

                // Commit the transaction
                Transaction.Commit();
            }
        }

        /// <summary>
        /// Backs up the asp database
        /// </summary>
        private void ExportAsASPBtn_Click(object sender, EventArgs e)
        {
            // Define backup folder for this backup, and create it if it doesnt exist
            string Folder = Path.Combine(Paths.DocumentsFolder, "Database Backups", "bak_" + DateTime.Now.ToString("yyyyMMdd_HHmm"));
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            // Abortion indicator
            bool Aborted = false;

            // Open the database connection
            StatsDatabase Database;
            try
            {
                Database = new StatsDatabase();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(
                    "Unable to connect to database\r\n\r\nMessage: " + Ex.Message,
                    "Database Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );

                // Stop the ASP server, and close this form
                HttpServer.Stop();
                this.Close();
                return;
            }

            // Show loading screen
            LoadingForm.ShowScreen(this);

            // Backup each table into its own bak file
            foreach (string Table in StatsDatabase.StatsTables)
            {
                // Create file path
                string BakFile = Path.Combine(Folder, Table + ".bak");

                // Backup tables
                try
                {
                    using (Stream Str = File.Open(BakFile, FileMode.OpenOrCreate))
                    using (StreamWriter Wtr = new StreamWriter(Str))
                    {
                        // Use a memory efficient way to export this stuff
                        foreach (Dictionary<string, object> Row in Database.QueryReader("SELECT * FROM " + Table))
                            Wtr.WriteLine(String.Join("\t", Row.Values));

                        Wtr.Flush();
                    }
                }
                catch (Exception Ex)
                {
                    // Close loading form
                    LoadingForm.CloseForm();

                    // Display the Exception Form
                    using (ExceptionForm Form = new ExceptionForm(Ex, false))
                    {
                        Form.Message = "An error occured while trying to backup the \"" + Table + "\" table. "
                            + "The backup operation will now be cancelled.";
                        DialogResult Result = Form.ShowDialog();
                    }
                    Aborted = true;

                    // Try and remove backup folder
                    try
                    {
                        DirectoryInfo Dir = new DirectoryInfo(Folder);
                        Dir.Delete(true);
                    }
                    catch { }
                }

                if (Aborted) break;
            }

            // Only display success message if we didnt abort
            if (!Aborted)
            {
                // Close loading form
                LoadingForm.CloseForm();

                string NL = Environment.NewLine;
                MessageBox.Show(
                    String.Concat("Backup has been completed successfully!", NL, NL, "Backup files have been saved to:", NL, Folder),
                    "Backup Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            // Close the connection
            Database.Dispose();
        }

        /// <summary>
        /// Clears the stats database of all data
        /// </summary>
        private void ClearStatsBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to clear the stats database? This will ERASE ALL stats data, and cannot be recovered!",
                "Confirm",
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                try
                {
                    using (StatsDatabase Database = new StatsDatabase())
                    {
                        Database.Truncate();
                    }
                    Notify.Show("Database Successfully Cleared!", "All stats have successfully been cleared.", AlertType.Success);
                }
                catch (Exception E)
                {
                    MessageBox.Show(
                        "An error occured while clearing the stats database!\r\n\r\nMessage: " + E.Message, 
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
}
