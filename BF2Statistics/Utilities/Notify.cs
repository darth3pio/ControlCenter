using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BF2Statistics
{
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
        protected static Queue<AlertOptions> Alerts = new Queue<AlertOptions>();

        /// <summary>
        /// Returns the number of open / active alerts
        /// </summary>
        public static int OpenAlerts { get; protected set; }

        /// <summary>
        /// Gets or Sets the default Popup style of an alert, if one is not specified in
        /// the ShowAlert method
        /// </summary>
        public static AlertPopupStyle DefaultPopupStyle = AlertPopupStyle.BottomToTop;

        /// <summary>
        /// Gets or Sets the default Close style of an alert, if one is not specified in
        /// the ShowAlert method
        /// </summary>
        public static AlertCloseStyle DefaultCloseStyle = AlertCloseStyle.TopToBottom;

        /// <summary>
        /// Gets or Sets the default dock time of an alert, if one is not specified in
        /// the ShowAlert method
        /// </summary>
        public static int DefaultDockTime = 5000;

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Notify()
        {
            OpenAlerts = 0;
        }

        public static void Show(string Message)
        {
            Alerts.Enqueue(new AlertOptions(Message, null, AlertType.Info, DefaultDockTime, DefaultPopupStyle, DefaultCloseStyle));
            CheckAlerts();
        }

        public static void Show(string Message, string SubText)
        {
            Alerts.Enqueue(new AlertOptions(Message, SubText, AlertType.Info, DefaultDockTime, DefaultPopupStyle, DefaultCloseStyle));
            CheckAlerts();
        }

        public static void Show(string Message, AlertType Type)
        {
            Alerts.Enqueue(new AlertOptions(Message, null, Type, DefaultDockTime, DefaultPopupStyle, DefaultCloseStyle));
            CheckAlerts();
        }

        public static void Show(string Message, string SubText, AlertType Type)
        {
            Alerts.Enqueue(new AlertOptions(Message, SubText, Type, DefaultDockTime, DefaultPopupStyle, DefaultCloseStyle));
            CheckAlerts();
        }

        public static void Show(string Message, string SubText, AlertType Type, int DockTime)
        {
            Alerts.Enqueue(new AlertOptions(Message, SubText, Type, DockTime, DefaultPopupStyle, DefaultCloseStyle));
            CheckAlerts();
        }

        public static void Show(string Message, string SubText, AlertType Type, int DockTime, AlertPopupStyle PopupStyle, AlertCloseStyle CloseStyle)
        {
            Alerts.Enqueue(new AlertOptions(Message, SubText, Type, DockTime, PopupStyle, CloseStyle));
            CheckAlerts();
        }

        /// <summary>
        /// This method is called internally to determine if a new alert
        /// can be shown from the alert queue.
        /// </summary>
        protected static void CheckAlerts()
        {
            if (Alerts.Count > 0 && OpenAlerts < 4)
            {
                // Add open alert
                OpenAlerts++;

                // Create form
                AlertOptions Opt = Alerts.Dequeue();
                AlertForm Form = new AlertForm();
                Form.AlertMessage = Opt.Message;
                Form.AlertSubText = Opt.SubText;
                Form.AlertType = Opt.Type;
                Form.DockTime = Opt.DockTime;
                Form.PopupStyle = Opt.PopupStyle;
                Form.CloseStyle = Opt.CloseStyle;
                Form.FormClosed += new FormClosedEventHandler(Form_FormClosed);

                // Use the main thread to display it
                MainForm.Instance.Invoke((MethodInvoker)delegate { Form.Show(); });
            }
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpenAlerts--;
            CheckAlerts();
        }

        internal struct AlertOptions
        {
            public string Message;
            public string SubText;
            public AlertType Type;
            public int DockTime;
            public AlertPopupStyle PopupStyle;
            public AlertCloseStyle CloseStyle;

            public AlertOptions(string Message, string SubText, AlertType Type, int DockTime, AlertPopupStyle PopupStyle, AlertCloseStyle CloseStyle)
            {
                this.Message = Message;
                this.SubText = SubText;
                this.Type = Type;
                this.DockTime = DockTime;
                this.PopupStyle = PopupStyle;
                this.CloseStyle = CloseStyle;
            }
        }
    }
}
