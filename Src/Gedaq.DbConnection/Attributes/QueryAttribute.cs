using Gedaq.Common.Enums;
using System;

namespace Gedaq.DbConnection.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryAttribute : Attribute
    {
        public QueryAttribute(
            string query,
            Type queryMapType,
            MethodType methodType,
            string methodName,
            QueryType queryType = QueryType.Read,
            bool generate = true
            )
        {
        }
    }
}