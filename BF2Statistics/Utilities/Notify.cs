using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BF2Statistics
{
    public enum AlertType
    {
        Info, Success, Warning
    }

    /// <summary>
    /// Notify is a class that queues and shows Alert "toast" messages
    /// to the user, which spawn just above the task bar, in the lower
    /// right hand side of the screen
    /// </summary>
    class Notify
    {
        /// <summary>
        /// A queue of alerts to display. Alerts are added here to prevent
        /// too many alerts from showing at once
        /// </summary>
        protected static Queue<NotifyOptions> Alerts = new Queue<NotifyOptions>();

        /// <summary>
        /// Returns the number of open / active alerts
        /// </summary>
        public static bool AlertsShowing { get; protected set; }

        /// <summary>
        /// Gets or Sets the default dock time of an alert, if one is not specified in
        /// the ShowAlert method
        /// </summary>
        public static int DefaultDockTime = 3000;

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Notify()
        {
            AlertsShowing = false;
            MainForm.SysIcon.BalloonTipClosed += new EventHandler(AlertClosed);
        }

        /// <summary>
        /// Displays a generic Toast message with the Info icon
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="SubText"></param>
        public static void Show(string Message, string SubText)
        {
            Alerts.Enqueue(new NotifyOptions(Message, SubText, AlertType.Info));
            CheckAlerts();
        }

        /// <summary>
        /// Shows a custom Toast message with the specified icon
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="SubText"></param>
        /// <param name="Type"></param>
        public static void Show(string Message, string SubText, AlertType Type)
        {
            Alerts.Enqueue(new NotifyOptions(Message, SubText, Type));
            CheckAlerts();
        }

        /// <summary>
        /// This method is called internally to determine if a new alert
        /// can be shown from the alert queue.
        /// </summary>
        protected static void CheckAlerts()
        {
            if (!AlertsShowing && Alerts.Count > 0)
            {
                AlertsShowing = true;
                NotifyOptions Opt = Alerts.Dequeue();
                MainForm.Instance.Invoke((Action)delegate
                {
                    MainForm.SysIcon.ShowBalloonTip(DefaultDockTime, Opt.Message, Opt.SubText, Opt.Icon);
                });
            }
        }

        /// <summary>
        /// Event is fired whenever a Toast alert is closed. This method is responsible
        /// for displaying the next queued alert message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AlertClosed(object sender, EventArgs e)
        {
            AlertsShowing = false;
            CheckAlerts();
        }

        /// <summary>
        /// An internal structure used to descibe the details on an Alert message
        /// to be displayed in the future
        /// </summary>
        internal struct NotifyOptions
        {
            public string Message;
            public string SubText;
            public ToolTipIcon Icon;

            public NotifyOptions(string Message, string SubText, AlertType Type)
            {
                this.Message = Message;
                this.SubText = SubText;

                switch (Type)
                {
                    default:
                        Icon = ToolTipIcon.Info;
                        break;
                    case AlertType.Warning:
                        Icon = ToolTipIcon.Warning;
                        break;
                }
            }
        }
    }
}
