using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Terminals.Forms
{
    public partial class ExpirationDialog : Form
    {
        private int counter;
        private Thread thread;

        public ExpirationDialog()
            : this(60)
        {
        }

        public ExpirationDialog(int seconds)
        {
            if (seconds >= 0)
            {
                this.Seconds = seconds;
            }

            this.counter = 0;
            this.TopLevel = true;
            this.Visible = true;
            this.Enabled = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private int Seconds { get; set; }

        private static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe),
                               new[] {control, propertyName, propertyValue});
            }
            else
            {
                control.GetType()
                       .InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new[] {propertyValue});
            }
        }

        private void Worker()
        {
            lock (this)
            {
                int seconds = Convert.ToInt32(this.lblSeconds.Text);

                int j = 0;

                for (int i = seconds; i >= 0; i--)
                {
                    Thread.Sleep(1000);
                    this.counter = i;

                    Application.DoEvents();
                    SetControlPropertyThreadSafe(this.lblSeconds, "Text", this.counter.ToString());
                    SetControlPropertyThreadSafe(this.progress, "Value", j);

                    float percentage = j*100f/seconds;

                    if (percentage >= 50)
                    {
                        SetControlPropertyThreadSafe(this.progress, "ForeColor", Color.Yellow);
                    }

                    if (percentage >= 75)
                    {
                        SetControlPropertyThreadSafe(this.progress, "ForeColor", Color.Red);
                    }

                    j++;
                    Application.DoEvents();
                }

                this.DialogResult = DialogResult.OK;

                this.thread.Abort();
            }
        }

        public new void Show()
        {
            this.ShowDialog();
        }

        private new DialogResult ShowDialog()
        {
            this.InitializeComponent();

            this.lblSeconds.Text = this.Seconds.ToString();

            this.progress.Minimum = 0;
            this.progress.Maximum = this.Seconds;
            this.progress.ForeColor = Color.LimeGreen;

            if (this.Seconds <= 5)
            {
                this.progress.ForeColor = Color.Red;
            }

            this.thread = new Thread(this.Worker);

            this.thread.Start();

            base.Show();

            while (this.DialogResult != DialogResult.OK)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            return DialogResult.OK;
        }

        public new void Show(IWin32Window owner)
        {
            this.ShowDialog();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return this.ShowDialog();
        }

        private delegate void SetControlPropertyThreadSafeDelegate(
            Control control, string propertyName, object propertyValue);
    }
}