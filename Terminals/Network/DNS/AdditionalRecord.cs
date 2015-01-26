using System;

namespace Terminals.Network.DNS
{
    [Serializable]
    public class AdditionalRecord : ResourceRecord
    {
        public AdditionalRecord(Pointer pointer) : base(pointer)
        {
        }
    }
}