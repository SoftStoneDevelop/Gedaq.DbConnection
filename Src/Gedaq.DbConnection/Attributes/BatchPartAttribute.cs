using System;

namespace Gedaq.DbConnection.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class BatchPartAttribute : Attribute
    {
        public BatchPartAttribute(
            string methodName,
            string batchName,
            int position
            )
        {
        }
    }
}