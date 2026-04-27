using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVVM
{
    internal static class Scanner
    {
        public static IReadOnlyDictionary<object, MemberInfo> ScanMembers(object target)
        {
            Dictionary<object, MemberInfo> members = new Dictionary<object, MemberInfo>();

            Type type = target.GetType();
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy
            );

            foreach (var field in fields)
            {
                MemberAttribute attribute = field.GetCustomAttribute<MemberAttribute>();
                if (attribute != null)
                {
                    members[attribute.id] = field;
                }
            }

            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy
            );

            foreach (MethodInfo method in methods)
            {
                MemberAttribute attribute = method.GetCustomAttribute<MemberAttribute>();
                if (attribute != null)
                {
                    members[attribute.id] = method;
                }
            }

            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy
            );

            foreach (PropertyInfo property in properties)
            {
                MemberAttribute attribute = property.GetCustomAttribute<MemberAttribute>();
                if (attribute != null)
                {
                    members[attribute.id] = property;
                }
            }

            EventInfo[] events = type.GetEvents(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy
            );

            foreach (var eventInfo in events)
            {
                MemberAttribute attribute = eventInfo.GetCustomAttribute<MemberAttribute>();
                if (attribute != null)
                {
                    members[attribute.id] = eventInfo;
                }
            }

            return members;
        }
    }
}