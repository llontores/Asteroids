using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MVVM
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodAttribute : MemberAttribute
    {
        public MethodAttribute(object id) : base(id)
        {
        }
    }
}