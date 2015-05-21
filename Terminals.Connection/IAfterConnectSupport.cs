using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminals.Connection
{
    public interface IAfterConnectSupport
    {
        bool IsAfterConnectEnabled { get; }

        bool IsInAfterConnectMode { get; set; }
    }
}
