using System;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.Attributes
{
    [AttributeUsage(AttributeTargets.Delegate)]
    internal class FunctionIdAttribute : Attribute
    {
        public FunctionIdAttribute(FunctionId functionId)
        {
            FunctionId = functionId;
        }

        public FunctionId FunctionId { get; set; }
    }
}