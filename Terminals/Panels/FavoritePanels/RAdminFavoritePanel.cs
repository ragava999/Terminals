using System;

using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Connection.Manager;
using Terminals.Connection.Panels.FavoritePanels;
using Terminals.Connections;

namespace Terminals.Panels.FavoritePanels
{
    public partial class RAdminFavoritePanel : FavoritePanel
    {
        public override string[] ProtocolName
        {
            get
            {
                return new[] { typeof(RAdminConnection).GetProtocolName() };
            }
        }

        public RAdminFavoritePanel()
        {
            this.InitializeComponent();
            
        }

        public override void FillControls(FavoriteConfigurationElement favorite)
        {
            this.txtRAdminPhonebookPath.Text = favorite.RAdminPhonebookPath;

            this.rdoRAdminStandardConnectionMode.Checked = favorite.RAdminStandardConnectionMode;
            this.rdoRAdminTelnetMode.Checked = favorite.RAdminTelnetMode;
            this.rdoRAdminViewOnly.Checked = favorite.RAdminViewOnlyMode;
            this.rdoRAdminFileTransferMode.Checked = favorite.RAdminFileTransferMode;
            this.rdoRAdminShutdown.Checked = favorite.RAdminShutdown;
            this.rdoRAadminChatMode.Checked = favorite.RAdminChatMode;
            this.rdoRAdminVoiceChatMode.Checked = favorite.RAdminVoiceChatMode;
            this.rdoRAdminSendTextMessageMode.Checked = favorite.RAdminSendTextMessageMode;

            this.chkRAdminUseFullScreen.Checked = favorite.RAdminUseFullScreen;

            this.chkRAdminThrough.Checked =
                this.txtRAdminThroughServerName.Enabled = this.txtRAdminThroughPort.Enabled = favorite.RAdminThrough;

            this.txtRAdminThroughServerName.Text = favorite.RAdminThroughServerName;
            this.txtRAdminThroughPort.Text = favorite.RAdminThroughPort;

            this.nudRAdminUpdates.Value = favorite.RAdminUpdates;

            try
            {
                RAdminConnection.ColorDepth colorDepth =
                    (RAdminConnection.ColorDepth)
                    Enum.Parse(typeof (RAdminConnection.ColorDepth), favorite.RAdminColorMode, true);

                switch (colorDepth)
                {
                    case RAdminConnection.ColorDepth.Bits1:
                        this.rdoRAdminColorMode1Bit.Checked = true;
                        break;
                    case RAdminConnection.ColorDepth.Bits2:
                        this.rdoRAdminColorMode2Bits.Checked = true;
                        break;
                    case RAdminConnection.ColorDepth.Bits4:
                        this.rdoRAdminColorMode4Bits.Checked = true;
                        break;
                    case RAdminConnection.ColorDepth.Bits8:
                        this.rdoRAdminColorMode8Bits.Checked = true;
                        break;
                    case RAdminConnection.ColorDepth.Bits16:
                        this.rdoRAdminColorMode16Bits.Checked = true;
                        break;
                    default:
                        this.rdoRAdminColorMode24Bits.Checked = true;
                        break;
                }
            }
            catch
            {
                /* don't parse */
            }
        }

        public override void FillFavorite(FavoriteConfigurationElement favorite)
        {
            favorite.RAdminPhonebookPath = this.txtRAdminPhonebookPath.Text;

            favorite.RAdminStandardConnectionMode = this.rdoRAdminStandardConnectionMode.Checked;
            favorite.RAdminTelnetMode = this.rdoRAdminTelnetMode.Checked;
            favorite.RAdminViewOnlyMode = this.rdoRAdminViewOnly.Checked;
            favorite.RAdminFileTransferMode = this.rdoRAdminFileTransferMode.Checked;
            favorite.RAdminShutdown = this.rdoRAdminShutdown.Checked;
            favorite.RAdminChatMode = this.rdoRAadminChatMode.Checked;
            favorite.RAdminVoiceChatMode = this.rdoRAdminVoiceChatMode.Checked;
            favorite.RAdminSendTextMessageMode = this.rdoRAdminSendTextMessageMode.Checked;

            favorite.RAdminUseFullScreen = this.chkRAdminUseFullScreen.Checked;

            favorite.RAdminThrough = this.chkRAdminThrough.Checked;
            favorite.RAdminThroughServerName = this.txtRAdminThroughServerName.Text;
            favorite.RAdminThroughPort = this.txtRAdminThroughPort.Text;

            favorite.RAdminUpdates = Convert.ToInt32(this.nudRAdminUpdates.Value);

            RAdminConnection.ColorDepth colorDepth = RAdminConnection.ColorDepth.Bits24;

            if (this.rdoRAdminColorMode1Bit.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits1;

            if (this.rdoRAdminColorMode2Bits.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits2;

            if (this.rdoRAdminColorMode4Bits.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits4;

            if (this.rdoRAdminColorMode8Bits.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits8;

            if (this.rdoRAdminColorMode16Bits.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits16;

            if (this.rdoRAdminColorMode24Bits.Checked)
                colorDepth = RAdminConnection.ColorDepth.Bits24;

            favorite.RAdminColorMode = colorDepth.ToString();
        }
    }
}