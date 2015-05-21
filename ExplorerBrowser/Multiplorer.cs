using System.ComponentModel;
using System.Windows.Forms;

namespace ExplorerBrowser
{
    public partial class Multiplorer : UserControl
    {
        private Explorer firstControl = null;
        private Explorer secondControl = null;
        private Explorer thirdControl = null;
        private Explorer fourthControl = null;

        /// <summary>
        /// Returns the first explorer window.
        /// </summary>
        /// <returns>
        /// The first window of type <see cref="T:ExplorerBrowser.Explorer"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Explorer FirstControl
        {
            get { return (Explorer)(this.controlStyler1.FirstControl = this.firstControl); }
        }

        /// <summary>
        /// Returns the second explorer window.
        /// </summary>
        /// <returns>
        /// The second window of type <see cref="T:ExplorerBrowser.Explorer"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Explorer SecondControl
        {
            get { return (Explorer)(this.controlStyler1.SecondControl = this.secondControl); }
        }

        /// <summary>
        /// Returns the third explorer window.
        /// </summary>
        /// <returns>
        /// The third window of type <see cref="T:ExplorerBrowser.Explorer"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Explorer ThirdControl
        {
            get { return (Explorer)(this.controlStyler1.ThirdControl = this.thirdControl); }
        }

        /// <summary>
        /// Returns the fourth explorer window.
        /// </summary>
        /// <returns>
        /// The fourth window of type <see cref="T:ExplorerBrowser.Explorer"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Explorer FourthControl
        {
            get { return (Explorer)(this.controlStyler1.FourthControl = this.fourthControl); }
        }

        [Browsable(true)]
        public ControlStyle ExplorerStyle
        {
            get
            {
                return this.controlStyler1.ControlStyle;
            }
            set
            {
                this.controlStyler1.ControlStyle = value;
            }
        }

        public new void Show()
        {
            this.Dock = DockStyle.Fill;

            System.Collections.Generic.List<System.Threading.Thread> threads = new System.Collections.Generic.List<System.Threading.Thread>();

            threads.Add(new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                this.controlStyler1.FirstControl = firstControl = new Explorer() { Dock = DockStyle.Fill };
            }));

            threads.Add(new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                this.controlStyler1.SecondControl = secondControl = new Explorer() { Dock = DockStyle.Fill };
            }));

            threads.Add(new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                this.controlStyler1.ThirdControl = thirdControl = new Explorer() { Dock = DockStyle.Fill };
            }));

            threads.Add(new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                this.controlStyler1.FourthControl = fourthControl = new Explorer() { Dock = DockStyle.Fill };
            }));

            for (int i = 0; i < threads.Count; i++)
                threads[i].Start();

            do
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(300);

                int isAliveCounter = 0;

                for (int i = 0; i < threads.Count; i++)
                    if (!threads[i].IsAlive)
                        isAliveCounter++;

                if (isAliveCounter == threads.Count)
                    break;
            } while (true);

            this.controlStyler1.Show();
        }

        public Multiplorer()
        {
            InitializeComponent();
        }
    }
}
