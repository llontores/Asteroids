using System;

namespace MVVM
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SetterAttribute : MemberAttribute
    {
        public SetterAttribute(object id) : base(id)
        {
        }
    }
}