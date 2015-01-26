using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using BF2Statistics.Properties;

namespace BF2Statistics
{
    public enum AlertType
    {
        Info, Success, Warning
    }

    public enum AlertPopupStyle
    {
        BottomToTop,
        RightToLeft,
        FadeIn,
        None
    }

    public enum AlertCloseStyle
    {
        TopToBottom,
        LeftToRight,
        FadeOut,
        None
    }

    public partial class AlertForm : Form
    {
        // Window params
        const int WS_EX_NOACTIVATE = 0x08000000;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_TOPMOST = 0x00000008;

        /// <summary>
        /// The total hieght of all the active alerts, with padding, 
        /// for stacking purposes, to prevent overlapping
        /// </summary>
        protected static int TotalHieght = 0;

        /// <summary>
        /// Form Position X
        /// </summary>
        protected int PosRight = Screen.PrimaryScreen.WorkingArea.Width;

        /// <summary>
        /// Form Position Y
        /// </summary>
        protected int PosBottom = Screen.PrimaryScreen.WorkingArea.Height;

        /// <summary>
        /// Target hieght of the alert
        /// </summary>
        protected int TargetHeight = 0;

        /// <summary>
        /// Target Width for the form
        /// </summary>
        protected int TargetWidth = 0;

        /// <summary>
        /// Gets or sets the alert type icon
        /// </summary>
        public AlertType AlertType = AlertType.Info;

        /// <summary>
        /// Gets or sets the Alert Popup Direction
        /// </summary>
        public AlertPopupStyle PopupStyle = AlertPopupStyle.BottomToTop;

        /// <summary>
        /// Gets or sets the Alert Close Direction
        /// </summary>
        public AlertCloseStyle CloseStyle = AlertCloseStyle.TopToBottom;

        /// <summary>
        /// Gets or sets the Alert Message
        /// </summary>
        public string AlertMessage
        {
            get { return labelHeader.Text; }
            set { labelHeader.Text = value; }
        }

        /// <summary>
        /// Gets or sets the Alert Contents
        /// </summary>
        public string AlertSubText
        {
            get { return labelDetails.Text; }
            set { labelDetails.Text = value; }
        }

        /// <summary>
        /// Gets or sets the Dock time for the alert (How long it stays open) in milliseconds
        /// </summary>
        public int DockTime
        {
            get { return CloseTimer.Interval; }
            set { CloseTimer.Interval = value; }
        }

        /// <summary>
        /// Indicates whether the Alert is closing
        /// </summary>
        public bool IsClosing { get; protected set; }

        /// <summary>
        /// Indicates whether the alert is collapsing
        /// after another alert, in a lower position,
        /// has closed
        /// </summary>
        protected bool IsCollapsing = false;

        /// <summary>
        /// Collapse distance
        /// </summary>
        protected int CollapsingDistance = 0;

        /// <summary>
        /// To prevent the OnFormClosed event from calling
        /// the AlertClosed event more then once
        /// </summary>
        protected bool CloseCalled = false;

        /// <summary>
        /// Closing delegate
        /// </summary>
        /// <param name="FormHeight"></param>
        protected delegate void OnAlertClosed(int FormHeight);

        /// <summary>
        /// Closing event
        /// </summary>
        protected static event OnAlertClosed AlertClosed;

        /// <summary>
        /// Prevents window from stealing focus (in some cases)
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        /// <summary>
        /// Prevent window from stealing focus (in most other cases)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
                return param;
            }
        }

        /// <summary>
        /// Constrctor
        /// </summary>
        public AlertForm()
        {
            InitializeComponent();

            // Variable setup
            IsClosing = false;

            // Register for close events
            AlertClosed += new OnAlertClosed(AlertForm_AlertClosed);
        }

        #region Show Methods

        new public void Show()
        {
            BuildForm();
            base.Show();
        }

        new public void Show(IWin32Window Owner)
        {
            BuildForm();
            base.Show(Owner);
        }

        new public DialogResult ShowDialog()
        {
            throw new Exception("Alerts cannot use the ShowDialog method to display");
        }

        new public DialogResult ShowDialog(IWin32Window Owner)
        {
            throw new Exception("Alerts cannot use the ShowDialog method to display");
        }

        #endregion

        protected void BuildForm()
        {
            // Remove details label to remove margin
            if (String.IsNullOrWhiteSpace(this.labelDetails.Text))
            {
                this.Controls.Remove(this.labelDetails);
                //this.labelHeader.Margin = new Padding(3, 0, 3, 12);
            }

            // Rounded form edges
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 1, 1));

            // Set heights so multiple alerts can show
            TargetHeight = TotalHieght + this.Height;
            TotalHieght += this.Height;

            // Get form start position
            if (PopupStyle == AlertPopupStyle.FadeIn)
            {
                this.Opacity = 0;
                PosRight -= this.Width - 5; // Add alittle margin from screen
                PosBottom -= TargetHeight;  // Set form top location
            }
            else if (PopupStyle == AlertPopupStyle.RightToLeft)
            {
                PosRight -= 5; // Add alittle margin from screen
                PosBottom -= TargetHeight;  // Set form top location
            }
            else if (PopupStyle == AlertPopupStyle.None)
            {
                PosRight -= this.Width - 5; // Add alittle margin from screen
                PosBottom -= TargetHeight;  // Set form top location
                OpenTimer.Enabled = false;
                CloseTimer.Enabled = true;
            }

            // Set target width after PosRight has been determined
            TargetWidth = PosRight - this.Width - 5;

            // Set initial position
            this.Location = new Point(PosRight, PosBottom);

            // Load icon image
            switch (this.AlertType)
            {
                default:
                    AlertIconBox.Image = Resources.InfoAlert;
                    break;
                case AlertType.Success:
                    AlertIconBox.Image = Resources.AlertSuccess;
                    break;
                case AlertType.Warning:
                    AlertIconBox.Image = Resources.AlertWarning;
                    break;
            }
        }


        /// <summary>
        /// Creates the rounded corner for the control
        /// </summary>
        /// <param name="nLeftRect">X coordinate of upper-left corner</param>
        /// <param name="nTopRect">Y coordinate of upper-left corner</param>
        /// <param name="nRightRect">X coordinate of lower-right corner</param>
        /// <param name="nBottomRect">Y coordinate of lower-right corner</param>
        /// <param name="nWidthEllipse">width of ellipse</param>
        /// <param name="nHeightEllipse">height of ellipse</param>
        /// <returns></returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
         );

        /// <summary>
        /// Calculates the correct Increment when animating the form
        /// </summary>
        /// <param name="CurrentLocation"></param>
        /// <param name="DesiredLocation"></param>
        /// <param name="CurrIncrement"></param>
        /// <returns></returns>
        private static int GetIncrement(int CurrentLocation, int DesiredLocation, int CurrIncrement)
        {
            // DL = 40; CurI = 20; CurL = 50;
            if ((CurrentLocation - CurrIncrement) < DesiredLocation)
                return (CurrentLocation - DesiredLocation);
            else
                return CurrIncrement;
        }

        /// <summary>
        /// Calculates the correct Decrement when animating the form
        /// </summary>
        /// <param name="CurrentLocation"></param>
        /// <param name="DesiredLocation"></param>
        /// <param name="CurrIncrement"></param>
        /// <returns></returns>
        private static int GetDecrement(int CurrentLocation, int DesiredLocation, int CurrIncrement)
        {
            // DL = 50; CurI = 20; CurL = 40;
            if ((CurrentLocation + CurrIncrement) > DesiredLocation)
                return (DesiredLocation - CurrentLocation);
            else
                return CurrIncrement;
        }

        /// <summary>
        /// Draws the animate when opening
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTimer_Tick(object sender, EventArgs e)
        {
            int Increment;

            switch (PopupStyle)
            {
                case AlertPopupStyle.BottomToTop:
                    double D = TargetHeight / 18;
                    Increment = GetIncrement(this.Location.Y, PosBottom - TargetHeight, (int)Math.Round(D, 0));
                    if (Increment > 0)
                    {
                        this.Location = new Point(TargetWidth, this.Location.Y - Increment);
                    }
                    else
                    {
                        CloseTimer.Enabled = true;
                        OpenTimer.Enabled = false;
                    }
                    break;
                case AlertPopupStyle.RightToLeft:
                    Increment = GetIncrement(this.Location.X, PosRight - this.Width, 10);
                    if (this.Location.X > PosRight - this.Width)
                    {
                        this.Location = new Point(this.Location.X - Increment, this.Location.Y);
                    }
                    else
                    {
                        CloseTimer.Enabled = true;
                        OpenTimer.Enabled = false;
                    }
                    break;
                case AlertPopupStyle.FadeIn:
                    if (this.Opacity < 0.90)
                    {
                        this.Opacity += 0.050;
                    }
                    else
                    {
                        CloseTimer.Enabled = true;
                        OpenTimer.Enabled = false;
                        this.Refresh();
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws the animate when closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            // If form is moving, try again later
            if (IsCollapsing)
            {
                CloseTimer.Interval = 500;
                return;
            }

            // For future Queue messages, Reduce the stack hieght
            if (!IsClosing)
            {
                TotalHieght -= this.Height;
                IsClosing = true;
            }

            switch (CloseStyle)
            {
                case AlertCloseStyle.TopToBottom:
                    if (this.Location.Y < PosBottom && this.Opacity > 0)
                    {
                        CloseTimer.Interval = 20;
                        this.Location = new Point(this.Location.X, this.Location.Y + 6);
                        this.Opacity -= 0.050;
                    }
                    else
                        this.Close();
                    break;
                case AlertCloseStyle.LeftToRight:
                    if (this.Location.X < PosRight && this.Opacity > 0)
                    {
                        CloseTimer.Interval = 10;
                        this.Location = new Point(this.Location.X + 15, this.Location.Y);
                        this.Opacity -= 0.050;
                    }
                    else
                        this.Close();
                    break;
                case AlertCloseStyle.FadeOut:
                    if (this.Opacity > 0)
                    {
                        CloseTimer.Interval = 10;
                        this.Opacity -= 0.050;
                    }
                    else
                    {
                        CloseTimer.Enabled = true;
                        OpenTimer.Enabled = false;
                    }
                    break;
                case AlertCloseStyle.None:
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// Draws the collapsing of the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapseTimer_Tick(object sender, EventArgs e)
        {
            int Decrement = GetDecrement(this.Location.Y, this.Location.Y + CollapsingDistance, 10);
            if (Decrement > 0)
            {
                Invoke((Action)delegate
                {
                    this.Location = new Point(TargetWidth, this.Location.Y + Decrement);
                    this.CollapsingDistance -= Decrement;
                });
            }
            else
            {
                Invoke((Action)delegate
                {
                    CollapseTimer.Enabled = false;
                    IsCollapsing = false;
                });
            }
        }

        /// <summary>
        /// Creates the background gradient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertForm_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.Black,
                Color.DimGray,
                120F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        /// <summary>
        /// On slower machines, its required to Refresh() the gui each time its moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertForm_LocationChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// When an alert is closed, we collapse the alerts on top of it
        /// </summary>
        /// <param name="FormHeight">The closed forms hieght, so we know how far to collapse</param>
        private void AlertForm_AlertClosed(int FormHeight)
        {
            if (!this.IsClosing)
            {
                IsCollapsing = true;
                CollapsingDistance += FormHeight;
                CollapseTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Each time an alert form is closed, we trigger the AlertClosed to collapse the other 
        /// active alerts
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnFormClosed(FormClosedEventArgs e)
        {
            if (!CloseCalled && AlertClosed != null)
            {
                CloseCalled = true;
                AlertClosed(this.Height);
            }
 	        base.OnFormClosed(e);
        }
    }
}
