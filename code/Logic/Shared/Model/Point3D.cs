using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    /// <summary>
    /// Represents a single three dimensional point
    /// </summary>
    public interface IPoint3D
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        public double X { get; }
        /// <summary>
        /// Y coordinate
        /// </summary>
        public double Y { get; }
        /// <summary>
        /// Z coordinate
        /// </summary>
        public double Z { get; }
    }
}
