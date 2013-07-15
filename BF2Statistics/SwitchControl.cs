using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace BF2Statistics
{
    public enum SwitchState
    {
        State1,
        State2
    }

    [DefaultEvent("StateChanged")]
    class SwitchControl : Control
    {
        private Rectangle boundsState1 = Rectangle.Empty;

        private Rectangle boundsSwitch = Rectangle.Empty;

        private Rectangle boundsState2 = Rectangle.Empty;

        private Rectangle boundsBorder = Rectangle.Empty;

        private System.Windows.Forms.Timer timer;

        private IContainer components;

        private int xPosition;

        private string textState1 = "ON";

        private string textState2 = "OFF";

        private Color backColorState1 = Color.FromArgb(42, 58, 86);

        private Color borderColor = Color.Gray;

        private Color backColorState2 = Color.White;

        private Color textColorState1 = Color.White;

        private Color textColorState2 = Color.DimGray;

        private int switchWidthPercent = 50;

        private Font font = new Font(SystemFonts.CaptionFont.FontFamily, 11f, FontStyle.Bold);

        public SwitchRenderer renderer = new SwitchRenderer();

        private SwitchState currentState;

        public Color BackColorState1
        {
            get
            {
                return this.backColorState1;
            }
            set
            {
                if (this.backColorState1 != value)
                {
                    this.backColorState1 = value;
                    base.Invalidate();
                }
            }
        }

        public Color BackColorState2
        {
            get
            {
                return this.backColorState2;
            }
            set
            {
                if (this.backColorState2 != value)
                {
                    this.backColorState2 = value;
                    base.Invalidate();
                }
            }
        }

        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                if (this.borderColor != value)
                {
                    this.borderColor = value;
                    base.Invalidate();
                }
            }
        }

        public SwitchState CurrentState
        {
            get
            {
                return this.currentState;
            }
            set
            {
                if (this.currentState != value)
                {
                    this.currentState = value;
                    this.OnStateChanged(this, this.CurrentState);
                    this.SetXPosByState();
                    this.CalculateBounds();
                    base.Invalidate();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(90, 25);
            }
        }

        public override Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                this.font = value;
            }
        }

        public int SwitchWidthPercent
        {
            get
            {
                return this.switchWidthPercent;
            }
            set
            {
                if (this.switchWidthPercent != value)
                {
                    this.switchWidthPercent = value;
                    if (this.switchWidthPercent <= 1)
                    {
                        this.switchWidthPercent = 1;
                    }
                    if (this.switchWidthPercent >= 100)
                    {
                        this.switchWidthPercent = 100;
                    }
                    this.SetXPosByState();
                    this.CalculateBounds();
                    base.Invalidate();
                }
            }
        }

        public Color TextColorState1
        {
            get
            {
                return this.textColorState1;
            }
            set
            {
                if (this.textColorState1 != value)
                {
                    this.textColorState1 = value;
                    base.Invalidate();
                }
            }
        }

        public Color TextColorState2
        {
            get
            {
                return this.textColorState2;
            }
            set
            {
                if (this.textColorState2 != value)
                {
                    this.textColorState2 = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue("ON")]
        [Description("Indicates text that is displayed on switch when Value property is set to true.")]
        public string TextState1
        {
            get
            {
                return this.textState1;
            }
            set
            {
                if (this.textState1 != value)
                {
                    this.textState1 = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue("OFF")]
        [Description("Indicates text that is displayed on switch when Value property is set to true.")]
        public string TextState2
        {
            get
            {
                return this.textState2;
            }
            set
            {
                if (this.textState2 != value)
                {
                    this.textState2 = value;
                    base.Invalidate();
                }
            }
        }

        public SwitchControl()
        {
            this.InitializeComponent();
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.CalculateBounds();
        }

        private void CalculateBounds()
        {
            this.boundsBorder = GraphicUtil.GetRectangle(base.ClientRectangle, 0, 1, 0, 1);
            int width = this.boundsBorder.Width * this.SwitchWidthPercent / 100;
            int num = this.xPosition;
            this.boundsState1 = new Rectangle(num, 0, this.boundsBorder.Width - width, this.boundsBorder.Height);
            this.boundsSwitch = new Rectangle(this.boundsState1.Right, 0, width, this.boundsBorder.Height);
            this.boundsState2 = new Rectangle(this.boundsSwitch.Right, 0, this.boundsState1.Width, this.boundsBorder.Height);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            base.Size = new Size(90, 25);
            base.SuspendLayout();
            this.timer.Interval = 15;
            this.timer.Tick += new EventHandler(this.timer_Tick);
            base.ResumeLayout(false);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!this.timer.Enabled)
            {
                this.timer.Start();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (base.Width <= 0)
            {
                base.Width = 1;
            }
            if (base.Height <= 0)
            {
                base.Height = 1;
            }
            GraphicsPath graphicsPath = GraphicUtil.RoundRect(this.boundsBorder, 3f);
            e.Graphics.SetClip(graphicsPath);
            this.Renderer.PaintState1(e.Graphics, this.boundsState1, this);
            this.Renderer.PaintTextState1(e.Graphics, this.boundsState1, this);
            this.Renderer.PaintState2(e.Graphics, this.boundsState2, this);
            this.Renderer.PaintTextState2(e.Graphics, this.boundsState2, this);
            e.Graphics.ResetClip();
            graphicsPath.Dispose();
            this.Renderer.PaintBorder(e.Graphics, this.boundsBorder, this);
            this.Renderer.PaintSwitch(e.Graphics, this.boundsSwitch, this);
        }

        protected override void OnResize(EventArgs e)
        {
            this.SetXPosByState();
            this.CalculateBounds();
            base.OnResize(e);
        }

        public virtual void OnStateChanged(object sender, SwitchState state)
        {
            if (this.StateChanged != null)
            {
                this.StateChanged(this, new SwitchEventArgs(this.CurrentState));
            }
        }

        private void SetXPosByState()
        {
            if (this.currentState != SwitchState.State2)
            {
                this.xPosition = 0;
            }
            else if (!base.Bounds.IsEmpty)
            {
                int width = this.boundsBorder.Width * this.SwitchWidthPercent / 100;
                this.xPosition = -(this.boundsBorder.Width - width);
                return;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int width = this.boundsState1.Width;
            if (this.currentState == SwitchState.State1)
            {
                if (this.xPosition > -width + 5)
                {
                    SwitchControl switchControl = this;
                    switchControl.xPosition = switchControl.xPosition - 10;
                }
                else
                {
                    this.xPosition = -width;
                    this.timer.Stop();
                    this.timer.Enabled = false;
                    this.currentState = SwitchState.State2;
                    this.OnStateChanged(this, this.CurrentState);
                }
            }
            else if (this.xPosition < -5)
            {
                SwitchControl switchControl1 = this;
                switchControl1.xPosition = switchControl1.xPosition + 10;
            }
            else
            {
                this.xPosition = 0;
                this.timer.Stop();
                this.timer.Enabled = false;
                this.currentState = SwitchState.State1;
                this.OnStateChanged(this, this.CurrentState);
            }
            this.CalculateBounds();
            base.Invalidate();
        }

        public event EventHandler<SwitchEventArgs> StateChanged;
    }

    public class SwitchRenderer
    {
        public SwitchRenderer()
        {
        }

        public virtual void PaintBorder(Graphics g, Rectangle bounds, SwitchControl control)
        {
            using (Pen pen = new Pen(control.BorderColor))
            {
                g.DrawPath(pen, GraphicUtil.RoundRect(bounds, 3f));
            }
        }

        public virtual void PaintState1(Graphics g, Rectangle bounds, SwitchControl control)
        {
            Rectangle rectangle = GraphicUtil.GetRectangle(bounds, 0, -1, 0, 0);
            using (SolidBrush solidBrush = new SolidBrush(control.BackColorState1))
            {
                g.FillRectangle(solidBrush, rectangle);
            }
        }

        public virtual void PaintState2(Graphics g, Rectangle bounds, SwitchControl control)
        {
            using (SolidBrush solidBrush = new SolidBrush(control.BackColorState2))
            {
                g.FillRectangle(solidBrush, bounds);
            }
        }

        public virtual void PaintSwitch(Graphics g, Rectangle bounds, SwitchControl control)
        {
            using (GraphicsPath graphicsPath = GraphicUtil.RoundRect(bounds, 3f, 3f, 3f, 3f))
            {
                g.SetClip(graphicsPath);
                using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb(238, 238, 238)))
                {
                    g.FillRectangle(solidBrush, bounds);
                }
                using (Pen pen = new Pen(Color.FromArgb(222, 222, 222)))
                {
                    g.DrawLine(pen, bounds.X + 2, bounds.Y + 1 + bounds.Height / 2, bounds.Width - 3, bounds.Y + 1 + bounds.Height / 2);
                }
                Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 2 + bounds.Height / 2, bounds.Width - 3, bounds.Height / 2 - 2);
                using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle, Color.FromArgb(216, 216, 216), Color.FromArgb(206, 206, 206), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(linearGradientBrush, rectangle);
                }
                g.ResetClip();
                using (Pen pen1 = new Pen(Color.Gray))
                {
                    g.DrawPath(pen1, graphicsPath);
                }
            }
        }

        public virtual void PaintTextState1(Graphics g, Rectangle bounds, SwitchControl control)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using (SolidBrush solidBrush = new SolidBrush(control.TextColorState1))
            {
                g.DrawString(control.TextState1, control.Font, solidBrush, bounds, stringFormat);
            }
        }

        public virtual void PaintTextState2(Graphics g, Rectangle bounds, SwitchControl control)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using (SolidBrush solidBrush = new SolidBrush(control.TextColorState2))
            {
                g.DrawString(control.TextState2, control.Font, solidBrush, bounds, stringFormat);
            }
        }
    }

    public class GraphicUtil
    {
        public GraphicUtil()
        {
        }

        public static Rectangle GetRectangle(Rectangle bounds, int inflation)
        {
            return new Rectangle(bounds.X + inflation, bounds.Y + inflation, bounds.Width - 2 * inflation, bounds.Height - 2 * inflation);
        }

        public static Rectangle GetRectangle(Rectangle bounds, int inflateLeftRight, int inflateTopBottom)
        {
            return new Rectangle(bounds.X + inflateLeftRight, bounds.Y + inflateTopBottom, bounds.Width - 2 * inflateLeftRight, bounds.Height - 2 * inflateTopBottom);
        }

        public static Rectangle GetRectangle(Rectangle bounds, int inflateLeftRight, int inflateTop, int inflateBottom)
        {
            return new Rectangle(bounds.X + inflateLeftRight, bounds.Y + inflateTop, bounds.Width - 2 * inflateLeftRight, bounds.Height - inflateBottom - inflateTop);
        }

        public static Rectangle GetRectangle(Rectangle bounds, int inflateLeft, int inflateRight, int inflateTop, int inflateBottom)
        {
            return new Rectangle(bounds.X + inflateLeft, bounds.Y + inflateTop, bounds.Width - 2 * inflateRight, bounds.Height - inflateBottom - inflateTop);
        }

        public static Image RotateImage(Image image, float angle)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Graphics graphic = Graphics.FromImage(bitmap);
            graphic.TranslateTransform((float)image.Width / 2f, (float)image.Height / 2f);
            graphic.RotateTransform(angle);
            graphic.TranslateTransform(-((float)image.Width / 2f), -((float)image.Height / 2f));
            graphic.DrawImage(image, new Point(0, 0));
            return bitmap;
        }

        public static GraphicsPath RoundRect(Rectangle r, float radius)
        {
            return GraphicUtil.RoundRect(r, radius, radius, radius, radius);
        }

        public static GraphicsPath RoundRect(Rectangle rect, float nwRadius, float neRadius, float seRadius, float swRadius)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            nwRadius = nwRadius * 2f;
            neRadius = neRadius * 2f;
            seRadius = seRadius * 2f;
            swRadius = swRadius * 2f;
            graphicsPath.AddLine((float)rect.X + nwRadius, (float)rect.Y, (float)rect.Right - neRadius, (float)rect.Y);
            if (neRadius > 0f)
            {
                graphicsPath.AddArc(RectangleF.FromLTRB((float)rect.Right - neRadius, (float)rect.Top, (float)rect.Right, (float)rect.Top + neRadius), -90f, 90f);
            }
            graphicsPath.AddLine((float)rect.Right, (float)rect.Top + neRadius, (float)rect.Right, (float)rect.Bottom - seRadius);
            if (seRadius > 0f)
            {
                graphicsPath.AddArc(RectangleF.FromLTRB((float)rect.Right - seRadius, (float)rect.Bottom - seRadius, (float)rect.Right, (float)rect.Bottom), 0f, 90f);
            }
            graphicsPath.AddLine((float)rect.Right - seRadius, (float)rect.Bottom, (float)rect.Left + swRadius, (float)rect.Bottom);
            if (swRadius > 0f)
            {
                graphicsPath.AddArc(RectangleF.FromLTRB((float)rect.Left, (float)rect.Bottom - swRadius, (float)rect.Left + swRadius, (float)rect.Bottom), 90f, 90f);
            }
            graphicsPath.AddLine((float)rect.Left, (float)rect.Bottom - swRadius, (float)rect.Left, (float)rect.Top + nwRadius);
            if (nwRadius > 0f)
            {
                graphicsPath.AddArc(RectangleF.FromLTRB((float)rect.Left, (float)rect.Top, (float)rect.Left + nwRadius, (float)rect.Top + nwRadius), 180f, 90f);
            }
            graphicsPath.CloseFigure();
            return graphicsPath;
        }
    }
}
