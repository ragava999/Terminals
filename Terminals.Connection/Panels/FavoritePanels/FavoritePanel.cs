using System.Drawing;
using System.Windows.Forms;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using System;
using Terminals.Connection.Panels.OptionPanels;

namespace Terminals.Connection.Panels.FavoritePanels
{
    public class FavoritePanel : UserControl
    {
        private string[] enabledForProtocols = null;

        public virtual string DefaultProtocolName
        {
            get
            {
                return typeof(Connection).GetProtocolName();
            }
        }

        public virtual string[] ProtocolName
        {
            get
            {
                string protocols = EnabledForProtocols();

                if (!string.IsNullOrEmpty(protocols))
                    enabledForProtocols = protocols.Split(new string[] { ",", " ", ";", "|" }, StringSplitOptions.RemoveEmptyEntries);
            
                if (string.IsNullOrEmpty(EnabledForProtocols()))
                    return new string[]{DefaultProtocolName};

                if (enabledForProtocols.Length < 1)
                    return new string[] { DefaultProtocolName };

                // Return all protocols
                if (enabledForProtocols.Length == 1 && enabledForProtocols[0].ToUpperInvariant() == "ALL")
                    return ConnectionManager.GetProtocols();

                if (enabledForProtocols.Length == 1 && enabledForProtocols[0].ToUpperInvariant() == DefaultProtocolName)
                    return new string[] { DefaultProtocolName };

                return enabledForProtocols;
            }
        }

        public string EnabledForProtocols(string text = null, string defaultValue = null)
        {
            return EnableProtocolOptionPanel.EnabledForProtocolsInternal(DefaultProtocolName, text, defaultValue);
        }

        protected FavoritePanel()
        {
            System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            InitializeComponent();
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, System.ResolveEventArgs args)
        {
        	return DependencyResolver.ResolveAssembly(sender, args);
        }

        private string text = null;
        private string name = null;
        public new virtual string Name
        {
            get { return name ?? (name = this.GetType().Name); }
            set { name = value; }
        }

        public new virtual string Text
        {
            get { return text ?? (text = this.GetType().Name.Replace(typeof(FavoritePanel).Name, "")); }
            set { text = value; }
        }

        new public Form ParentForm { get; set; }

        public virtual void HandleSelectionChanged(string protocol, bool isEditing)
        {

        }

        public virtual void FillControls(FavoriteConfigurationElement favorite)
        {
        }

        public virtual void FillFavorite(FavoriteConfigurationElement favorite)
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FavoritePanel
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Size = new System.Drawing.Size(512, 322);
            this.ResumeLayout(false);
        }
    }
}