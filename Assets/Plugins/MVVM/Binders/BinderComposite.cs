using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace MVVM
{
    public class BinderComposite : IBinder
    {
        private readonly List<IBinder> children;

        public BinderComposite(List<IBinder> children)
        {
            this.children = children;
        }

        public virtual void Bind()
        {
            for (int i = 0, count = this.children.Count; i < count; i++)
            {
                IBinder child = this.children[i];
                child.Bind();
            }
        }

        public virtual void Unbind()
        {
            for (int i = 0, count = this.children.Count; i < count; i++)
            {
                IBinder child = this.children[i];
                child.Unbind();
            }
        }
    }
}