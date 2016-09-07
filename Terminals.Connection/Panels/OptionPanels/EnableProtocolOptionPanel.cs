namespace Terminals.Connection.Panels.OptionPanels
{
    using Terminals.Connection.Manager;

    using Configuration.Files.Main.Settings;

    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public partial class EnableProtocolOptionPanel : OptionPanel
    {
        public virtual string DefaultProtocolName
        {
            get
            {
                return typeof(Connection).GetProtocolName();
            }
        }

        public override string Text
        {
            get
            {
                return string.Format("Enable protocols for {0} plugin.", ConnectionManager.GetProtcolNameCamalCase(this.DefaultProtocolName));
            }
        }

        public EnableProtocolOptionPanel()
        {
            this.InitializeComponent();
        }

        protected List<string> ExcludeProtocols = new List<string>();

        private bool alreadyInitializedAndLoaded = false;
        private bool protocolSkipped = false;

        public override void LoadSettings()
        {
            if (alreadyInitializedAndLoaded)
                return;

            string[] names = ConnectionManager.GetProtocols().Where(name => name != DefaultProtocolName).ToArray<string>();

            string enabledProtocols = EnabledForProtocols();

            int startlocation = -5;
            int location = startlocation;
            int left = 17;
            int counter = 1;

            foreach (string name in names)
            {
                // Number of allowed elements per row
                // After 15 elements we'll break the row.
                if (counter == 15)
                {
                    left = 135;
                    location = startlocation;
                }

                if (!ExcludeProtocols.Contains(name))
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Name = name;
                    location += 20;
                    checkBox.Location = new System.Drawing.Point(left, location);
                    checkBox.Text = name;
                    checkBox.UseVisualStyleBackColor = true;
                    checkBox.AutoSize = true;

                    alreadyInitializedAndLoaded = true;
                    
                    grpConnections.Controls.Add(checkBox);
                    counter++;
                }
                else
                    protocolSkipped = true;
            }

            if (!string.IsNullOrEmpty(enabledProtocols))
            {
                if (enabledProtocols.ToUpperInvariant() == DefaultProtocolName)
                {
                    optThis.Checked = true;
                }
                else if (!protocolSkipped && enabledProtocols.ToUpperInvariant() == "ALL")
                {
                    optAll.Checked = true;
                }
                else
                {
                    string[] protocols = EnabledForProtocols().Split(new string[] { ",", " ", ";", "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (protocols.Length >= 1)
                    {
                        optSpecific.Checked = true;

                        // This can only happen if we want to have all checkboxes but excluded one or two for example.
                        if (protocolSkipped && protocols[0] == "ALL")
                        {
                            // This is the first time ... the default value
                            foreach (CheckBox checkBox in grpConnections.Controls)
                            {
                                checkBox.Checked = true;
                            }
                        }
                        // This happens if we select a specific range or some protocols only.
                        else
                            foreach (string name in names)
                            {
                                CheckBox box =
                                (from chk in grpConnections.Controls.Cast<CheckBox>()
                                 where chk.Name == name
                                 select chk).FirstOrDefault();

                                // Null might happen if we have decided to exclude some protocols
                                if (box != null)
                                    box.Checked = protocols.FirstOrDefault(checkednames => checkednames == name) == null ? false : true;
                            }
                    }
                    else
                        optThis.Checked = true;
                }
            }
            else
                optThis.Checked = true;
        }

        public override void SaveSettings()
        {
            if (optThis.Checked)
            {
                EnabledForProtocols(DefaultProtocolName);
            }
            else if (optAll.Checked)
            {
                // If the user changed to the "ALL" checkbox, but we decided to exclude
                // some we need to explicitly specify the protocols for which we want to
                // load the connection
                if (protocolSkipped)
                {
                    // doesn't matter if either checked or unchecked.
                    // for every displayed checkbox!
                    SetSpecificProtocolNames(
                    (from control in grpConnections.Controls.Cast<CheckBox>()
                     //where control.Checked
                    select control.Name).ToArray<string>());
                }
                // If the user selected "ALL" and we don't want to skip some connections per plugin
                else
                    EnabledForProtocols("ALL");
            }
            else
            {
                // Only for checked checkboxes (for selected ones)!
                SetSpecificProtocolNames(
                (from control in grpConnections.Controls.Cast<CheckBox>()
                 where control.Checked
                 select control.Name).ToArray<string>());
            }
        }

        private void SetSpecificProtocolNames(string[] names)
        {
            string result = "";

            foreach (string name in names)
            {
                result += name + ";";
            }
            result = result.TrimEnd(';');

            if (string.IsNullOrEmpty(result))
                EnabledForProtocols(DefaultProtocolName);
            else
                EnabledForProtocols(result);
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            grpConnections.Enabled = optSpecific.Checked;
        }

        public string EnabledForProtocols(string text = null, string defaultValue = null)
        {
            return EnabledForProtocolsInternal(DefaultProtocolName, text, defaultValue);
        }

        internal static string EnabledForProtocolsInternal(string typeName, string text = null, string defaultValue = null)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(defaultValue))
                return Settings.GetPluginOption<string>(typeName + "_EnabledForProtocols");

            Settings.SetPluginOption(typeName + "_EnabledForProtocols", text, defaultValue);
            return null;
        }
    }
}