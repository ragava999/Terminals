using System;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class TextChangedEventArgs : EventArgs
    {
        public Int32 FieldIndex { get; set; }

        public String Text { get; set; }
    }
}