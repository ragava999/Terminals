using System.Windows.Forms;
using Terminals.Connection.Manager;

namespace Terminals.Connection.Panels.OptionPanels
{
    public class OptionPanel : IOptionPanel
    {
        public new virtual string Text
        {
            get
            {
                return string.Format("{0} options", ConnectionManager.GetProtcolNameCamalCase(this.Name));
            }
        }

        public new string Name
        {
            get
            {
                return this.GetType().Name.Replace(typeof(OptionPanel).Name, "");
            }
            set
            {
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OptionPanel
            // 
            this.Name = "OptionPanel";
            this.Size = new System.Drawing.Size(514, 332);
            this.ResumeLayout(false);
        }
    }
}
