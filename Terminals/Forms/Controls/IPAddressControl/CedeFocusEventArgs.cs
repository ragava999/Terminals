using System;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class CedeFocusEventArgs : EventArgs
    {
        public Int32 FieldIndex { get; set; }

        public IPAddressControlAction IPAddressControlAction { get; set; }

        public IPAddressControlDirection IPAddressControlDirection { get; set; }

        public IPAddressControlSelection IPAddressControlSelection { get; set; }
    }
}