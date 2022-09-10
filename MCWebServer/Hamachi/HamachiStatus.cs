using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.Hamachi
{
    /// <summary>
    /// Data class for representing Hamachi Status
    /// </summary>
    internal class HamachiStatus
    {
        public string NickName { get; init; }
        public string Address { get; init; }
        public bool Online { get; init; }
    }
}
