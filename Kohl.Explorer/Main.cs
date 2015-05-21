using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kohl.Explorer
{
    public partial class Main : Form
    {
        public string ExplorerDirectory
        {
            get { return multiplorer1.FirstControl.InitDirectory; }
            set
            {
                multiplorer1.FirstControl.InitDirectory = value;
                multiplorer1.FirstControl.NavigateToInitDirectory();
            }
        }

        public string ExplorerDirectoryDual
        {
            get { return multiplorer1.SecondControl.InitDirectory; }
            set
            {
                multiplorer1.SecondControl.InitDirectory = value;
                multiplorer1.SecondControl.NavigateToInitDirectory();
            }
        }

        public string ExplorerDirectoryTripple
        {
            get { return multiplorer1.ThirdControl.InitDirectory; }
            set
            {
                multiplorer1.ThirdControl.InitDirectory = value;
                multiplorer1.ThirdControl.NavigateToInitDirectory();
            }
        }

        public string ExplorerDirectoryQuad
        {
            get { return multiplorer1.FourthControl.InitDirectory; }
            set
            {
                multiplorer1.FourthControl.InitDirectory = value;
                multiplorer1.FourthControl.NavigateToInitDirectory();
            }
        }

        public Main(string[] args)
        {
            InitializeComponent();
            multiplorer1.Show();

            multiplorer1.FirstControl.NavigateToSpecialFolder("Desktop");

            if (args == null)
                return;

            ExplorerBrowser.ControlStyle controlStyle = ExplorerBrowser.ControlStyle.Single;

            if (args.Length >= 1)
                // .NET 4.0
                // Enum.TryParse<ExplorerBrowser.ControlStyle>(args[0].Trim('"').Trim('\''), out controlStyle);
                // .NET 2.0
                try
                {
                    controlStyle = (ExplorerBrowser.ControlStyle)Enum.Parse(typeof(ExplorerBrowser.ControlStyle), args[0].Trim('"').Trim('\''));
                }
                catch {}

            multiplorer1.ExplorerStyle = controlStyle;

            switch (args.Length)
            {
                case 2:
                    ExplorerDirectory = args[1].Trim('"').Trim('\'');
                    break;
                case 3:
                    ExplorerDirectory = args[1].Trim('"').Trim('\'');
                    ExplorerDirectoryDual = args[2].Trim('"').Trim('\'');
                    break;
                case 4:
                    ExplorerDirectory = args[1].Trim('"').Trim('\'');
                    ExplorerDirectoryDual = args[2].Trim('"').Trim('\'');
                    ExplorerDirectoryTripple = args[3].Trim('"').Trim('\'');
                    break;
                case 5:
                    ExplorerDirectory = args[1].Trim('"').Trim('\'');
                    ExplorerDirectoryDual = args[2].Trim('"').Trim('\'');
                    ExplorerDirectoryTripple = args[3].Trim('"').Trim('\'');
                    ExplorerDirectoryQuad = args[4].Trim('"').Trim('\'');
                    break;
            }
        }
    }
}
