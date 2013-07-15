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
using FolderSelect;

namespace BF2Statistics
{
    public partial class ManageStatsDBForm : Form
    {
        /// <summary>
        /// The stats database object
        /// </summary>
        protected StatsDatabase Db = ASP.ASPServer.Database;

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportASPBtn_Click(object sender, EventArgs e)
        {
            // Open File Select Dialog
            FolderSelectDialog Dialog = new FolderSelectDialog();
            Dialog.Title = "Select ASP Database Backup Folder";
            Dialog.InitialDirectory = Path.Combine(Paths.DocumentsFolder, "Backups");
            if (Dialog.ShowDialog())
            {
                // Get files list from path
                string path = Dialog.SelectedPath;
                string[] BakFiles = Directory.GetFiles(path, "*.bak");
                if (BakFiles.Length > 0)
                {
                    // Show task dialog
                    TaskForm.Show(this, "Importing Stats", "Importing ASP Stats Bak Files...", false);
                    TaskForm.UpdateStatus("Removing old stats data");

                    // Clear old database records
                    Db.Truncate();
                    Thread.Sleep(500);

                    // To prevent packet size errors
                    if(Db.Driver.DatabaseEngine == DatabaseEngine.Mysql)
                        Db.Driver.Execute("SET GLOBAL max_allowed_packet=51200");

                    // Begin transaction
                    DbTransaction Transaction = Db.Driver.BeginTransaction();

                    // import each table
                    foreach (string file in BakFiles)
                    {
                        // Get table name
                        string table = Path.GetFileNameWithoutExtension(file);

                        // Update progress
                        TaskForm.UpdateStatus("Processing stats table: " + table);

                        // Import table data
                        try
                        {
                            // Sqlite kinda sucks... no import methods
                            if (Db.Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                            {
                                string[] Lines = File.ReadAllLines(file);
                                foreach (string line in Lines)
                                {
                                    string[] Values = line.Split('\t');
                                    Db.Driver.Execute(
                                        String.Format("INSERT INTO {0} VALUES({1})", table, "\"" + String.Join("\", \"", Values) + "\"")
                                    );
                                }
                            }
                            else
                                Db.Driver.Execute(String.Format("LOAD DATA INFILE '{0}' INTO TABLE {1};", file.Replace('\\', '/'), table));
                        }
                        catch (Exception Ex)
                        {
                            // Show exception error
                            ExceptionForm Form = new ExceptionForm(Ex, false);
                            Form.Message = String.Format("Failed to import data into table {0}!{2}{2}Error: {1}", table, Ex.Message, Environment.NewLine);
                            DialogResult Result = Form.ShowDialog();

                            // Rollback!
                            TaskForm.UpdateStatus("Rolling back stats data");
                            Transaction.Rollback();

                            // Update message
                            TaskForm.CloseForm();
                            return;
                        }
                    }

                    // Commit the transaction, and alert the user
                    Transaction.Commit();
                    TaskForm.CloseForm();
                    Notify.Show("Stats imported successfully!", AlertType.Success);
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
        /// Backs up the asp database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportAsASPBtn_Click(object sender, EventArgs e)
        {
            // Define backup folder for this backup, and create it if it doesnt exist
            string Folder = Path.Combine(Paths.DocumentsFolder, "Backups", "bak_" + DateTime.Now.ToString("yyyyMMdd_HHmm"));
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            // Abortion indicator
            bool Aborted = false;

            // Show loading screen
            LoadingForm.ShowScreen(this);

            // Backup each table into its own bak file
            foreach (string Table in StatsDatabase.GetStatsTables())
            {
                // Create file path
                string BakFile = Path.Combine(Folder, Table + ".bak");

                // Backup tables
                try
                {
                    // fetch the data from the table
                    if (Db.Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                    {
                        // Use a memory efficient way to export this stuff
                        StringBuilder Data = new StringBuilder();
                        foreach(Dictionary<string, object> Row in Db.Driver.QueryReader("SELECT * FROM " + Table))
                            Data.AppendLine(String.Join("\t", Row.Values));

                        // Write to file
                        File.AppendAllText(BakFile, Data.ToString());
                    }
                    else
                        Db.Driver.Execute(String.Format("SELECT * INTO OUTFILE '{0}' FROM {1}", BakFile.Replace('\\', '/'), Table));
                }
                catch (Exception Ex)
                {
                    // Close loading form
                    LoadingForm.CloseForm();

                    // Display the Exception Form
                    ExceptionForm Form = new ExceptionForm(Ex, false);
                    Form.Message = "An error occured while trying to backup the \"" + Table + "\" table. "
                        + "The backup operation will now be cancelled.";
                    DialogResult Result = Form.ShowDialog();
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
        }

        /// <summary>
        /// Clears the stats database of all data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    Db.Truncate();
                    Notify.Show("Database Successfully Cleared!", AlertType.Success);
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
