using System;

namespace Terminals.Network.DNS
{
    [Serializable]
    public class Answer : ResourceRecord
    {
        public Answer(Pointer pointer) : base(pointer)
        {
        }
    }
}