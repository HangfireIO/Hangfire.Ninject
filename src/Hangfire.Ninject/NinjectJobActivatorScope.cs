using System;
#if !NET45
using System.Threading;
#endif
using Ninject;
using Ninject.Activation.Caching;

namespace Hangfire
{
    public sealed class NinjectJobActivatorScope : JobActivatorScope
    {
#if !NET45
        private static readonly AsyncLocal<NinjectJobActivatorScope> CurrentScope = new AsyncLocal<NinjectJobActivatorScope>();
#endif
        private readonly IKernel _kernel;

        public NinjectJobActivatorScope(IKernel kernel)
        {
            _kernel = kernel;
#if !NET45
            CurrentScope.Value = this;
#endif
        }

#if !NET45
        public new static NinjectJobActivatorScope Current => CurrentScope.Value;
#else
        public new static JobActivatorScope Current => JobActivatorScope.Current;
#endif

        public override object Resolve(Type type)
        {
            return _kernel.Get(type);
        }

        public override void DisposeScope()
        {
            _kernel.Components.Get<ICache>().Clear(Current);
#if !NET45
            CurrentScope.Value = null;
#endif
        }
    }
}