using System;

namespace NvAPIWrapper.Native.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Delegate)]
    internal class AcceptsAttribute : Attribute
    {
        public AcceptsAttribute(params Type[] types)
        {
            Types = types;
        }

        public Type[] Types { get; set; }
    }
}