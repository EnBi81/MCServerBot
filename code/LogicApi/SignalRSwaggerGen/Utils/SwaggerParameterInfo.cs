using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SignalRSwaggerGen.Utils
{
    public class SwaggerParameterInfo : ParameterInfo
    {
        public SwaggerParameterInfo(string name, Type type, int position)
        {
            AttrsImpl = ParameterAttributes.Optional;
            ClassImpl = type;
            MemberImpl = null;
            NameImpl = name;
            PositionImpl = position;
        }
    }
}
