using System;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class FieldChangedEventArgs : EventArgs
    {
        public Int32 FieldIndex { private get; set; }

        public String Text { private get; set; }
    }
}