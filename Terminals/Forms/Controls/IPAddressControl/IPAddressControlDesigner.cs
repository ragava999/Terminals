using System.Collections;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Terminals.Forms.Controls.IPAddressControl
{
    public class IPAddressControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                IPAddressControl control = (IPAddressControl)this.Control;

                if (control.AutoHeight)
                    return SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.LeftSizeable |
                           SelectionRules.RightSizeable;
                return SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
            }
        }

        public override IList SnapLines
        {
            get
            {
                IPAddressControl control = (IPAddressControl)this.Control;

                IList snapLines = base.SnapLines;

                snapLines.Add(new SnapLine(SnapLineType.Baseline, control.Baseline));

                return snapLines;
            }
        }
    }
}