using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRSwaggerGen.Utils
{
    internal class ListenerEqualityComparer : IEqualityComparer<SignalRListenerAttribute>
    {
        public bool Equals(SignalRListenerAttribute x, SignalRListenerAttribute y) => x.Name == y.Name;
        public int GetHashCode(SignalRListenerAttribute obj) => obj.GetHashCode();
    }
}
