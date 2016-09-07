using System;
using System.Text;
using System.Windows.Forms;

namespace WalburySoftware
{
    public partial class TerminalEmulator : Control
    {
        private void DispatchMessage(Object sender, string strText)
        {
            if (this.XOFF || this.OnDataRequested == null)
            {
                // store the characters in the outputbuffer
                this.OutBuff += strText;
            }
            else
            {
                if (this.OutBuff != String.Empty)
                {
                    strText = this.OutBuff + strText;
                    this.OutBuff = String.Empty;
                }

                if (strText == '\r'.ToString())
                {
                    this.history.Add(this.keyboardBuffer);
                    this.keyboardBuffer = String.Empty;
                }
                else if (strText == "Paste")
                {
                    strText = string.Empty;
                    this.DispatchMessage(this, Clipboard.GetText());
                }
                else
                {
                    if (this.Keyboard.UpArrow)
                    {
                        // wipe the current input
                        // replace it with the history index -1
                    }
                    else
                    {
                        this.keyboardBuffer += strText;
                    }
                }

                this.OnDataRequested(Encoding.Default.GetBytes(strText));
            }
        }
    }
}