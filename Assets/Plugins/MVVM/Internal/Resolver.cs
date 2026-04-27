using System.Reflection;

namespace MVVM
{
    internal static class Resolver
    {
        public static bool TryResolve(
            MemberInfo viewMember,
            MemberInfo modelMember,
            object parentView,
            object parentModel,
            out object view,
            out object model
        )
        {
            return TryResolveViewValue(viewMember, modelMember, parentView, parentModel, out view, out model) ||
                   TryResolveViewSetter(viewMember, modelMember, parentView, parentModel, out view, out model) ||
                   TryResolveViewMethod(viewMember, modelMember, parentView, parentModel, out view, out model);
        }

        private static bool TryResolveViewValue(
            MemberInfo viewMember,
            MemberInfo modelMember,
            object parentView,
            object parentModel,
            out object view,
            out object model
        )
        {
            if (viewMember.IsDefined(typeof(DataAttribute)))
            {
                FieldInfo viewField = viewMember as FieldInfo;
                view = viewField!.GetValue(parentView);

                if (modelMember.IsDefined(typeof(DataAttribute)))
                {
                    FieldInfo modelField = modelMember as FieldInfo;
                    model = modelField!.GetValue(parentModel);
                    return true;
                }

                if (modelMember.IsDefined(typeof(MethodAttribute)))
                {
                    MethodInfo modelMethod = modelMember as MethodInfo;
                    model = Utils.CreateActionDelegate(modelMethod, parentModel);
                    return true;
                }

                if (modelMember.IsDefined(typeof(SetterAttribute)))
                {
                    PropertyInfo modelProperty = modelMember as PropertyInfo;
                    model = Utils.CreateActionDelegate(modelProperty!.SetMethod, parentModel);
                    return true;
                }

                // if (modelMember.IsDefined(typeof(GetterAttribute)))
                // {
                //     PropertyInfo modelProperty = modelMember as PropertyInfo;
                //     model = MVVMUtils.CreateFunctionDelegate(modelProperty!.GetMethod, parentModel);
                //     return true;
                // }
            }

            view = default;
            model = default;
            return false;
        }

        private static bool TryResolveViewSetter(
            MemberInfo viewMember,
            MemberInfo modelMember,
            object parentView,
            object parentModel,
            out object view,
            out object model
        )
        {
            if (viewMember.IsDefined(typeof(SetterAttribute)))
            {
                PropertyInfo viewProperty = viewMember as PropertyInfo;
                view = Utils.CreateActionDelegate(viewProperty!.SetMethod, parentView);

                if (modelMember.IsDefined(typeof(DataAttribute)))
                {
                    FieldInfo modelField = modelMember as FieldInfo;
                    model = modelField!.GetValue(parentModel);
                    return true;
                }
                //
                // if (modelMember.IsDefined(typeof(GetterAttribute)))
                // {
                //     PropertyInfo modelProperty = modelMember as PropertyInfo;
                //     model = MVVMUtils.CreateFunctionDelegate(modelProperty!.GetMethod, parentModel);
                //     return true;
                // }
            }

            view = default;
            model = default;
            return false;
        }

        private static bool TryResolveViewMethod(
            MemberInfo viewMember,
            MemberInfo modelMember,
            object parentView,
            object parentModel,
            out object view,
            out object model
        )
        {
            if (viewMember.IsDefined(typeof(MethodAttribute)))
            {
                MethodInfo viewMethod = viewMember as MethodInfo;
                view = Utils.CreateActionDelegate(viewMethod, parentView);

                if (modelMember.IsDefined(typeof(DataAttribute)))
                {
                    FieldInfo modelField = modelMember as FieldInfo;
                    model = modelField!.GetValue(parentModel);
                    return true;
                }

                // if (modelMember.IsDefined(typeof(GetterAttribute)))
                // {
                //     PropertyInfo modelProperty = modelMember as PropertyInfo;
                //     model = MVVMUtils.CreateFunctionDelegate(modelProperty!.GetMethod, parentModel);
                //     return true;
                // }
            }

            view = default;
            model = default;
            return false;
        }
    }
}