using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalRSwaggerGen.Attributes
{
    /// <summary>
	/// Use this attribute to enable Swagger documentation for a listener
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class SignalRListenerAttribute : Attribute
    {
        public string Name { get; }
        public AutoDiscover AutoDiscover { get; }
        public string Summary { get; }
        public string Description { get; }

        

        public SignalRListenerAttribute(
            string name,
            AutoDiscover autoDiscover = AutoDiscover.Inherit,
            string summary = null,
            string description = null)
        {
            if (name.IsNullOrEmpty()) throw new ArgumentException("Name is null or empty", nameof(name));
            if (!_validAutoDiscoverValues.Contains(autoDiscover)) throw new ArgumentException($"Value {autoDiscover} not allowed for this attribute", nameof(autoDiscover));
            Name = name;
            AutoDiscover = autoDiscover;
            Summary = summary;
            Description = description;
        }


        private static readonly IEnumerable<AutoDiscover> _validAutoDiscoverValues = new List<AutoDiscover>
        {
            AutoDiscover.Inherit,
            AutoDiscover.None,
            AutoDiscover.Params,
        };
    }
}
