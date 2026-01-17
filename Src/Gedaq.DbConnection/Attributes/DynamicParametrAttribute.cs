using System;

namespace Gedaq.DbConnection.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DynamicParametrAttribute : Attribute
    {
        public DynamicParametrAttribute()
        {
        }
    }
}
