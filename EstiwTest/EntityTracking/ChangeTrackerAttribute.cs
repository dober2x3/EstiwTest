using System;

namespace Lx.Data.Repository
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ChangeTrackerAttribute : Attribute
    { }
}
