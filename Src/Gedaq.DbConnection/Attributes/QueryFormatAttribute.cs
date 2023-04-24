using System;

namespace Gedaq.DbConnection.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryFormatAttribute : Attribute
    {
        public QueryFormatAttribute(
            string methodName,
            int position = 0,
            string parametrName = null
            )
        {
        }
    }
}