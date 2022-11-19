using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidValuesAttribute : Attribute
    {
        public string[] ValidValues { get; }

        public ValidValuesAttribute(params string[] validValues)
        {
            ValidValues = validValues;
        }
    }
}
