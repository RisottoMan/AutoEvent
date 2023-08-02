using System;

namespace AutoEvent.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DisabledFeaturesAttribute : Attribute
    {
    }
}
