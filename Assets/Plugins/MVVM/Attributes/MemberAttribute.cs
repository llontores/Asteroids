using JetBrains.Annotations;

namespace MVVM
{
    [MeansImplicitUse]
    public abstract class MemberAttribute : System.Attribute
    {
        internal readonly object id;

        protected MemberAttribute(object id)
        {
            this.id = id;
        }
    }
}