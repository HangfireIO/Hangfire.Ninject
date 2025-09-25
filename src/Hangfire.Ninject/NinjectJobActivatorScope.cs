using System;
#if !NET45
using System.Threading;
#endif
using Ninject;
using Ninject.Infrastructure.Disposal;

namespace Hangfire
{
    public class NinjectJobActivatorScope : JobActivatorScope, INotifyWhenDisposed
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

        public bool IsDisposed { get; private set; }
        public event EventHandler Disposed;

        public override object Resolve(Type type)
        {
            return _kernel.Get(type);
        }

        public sealed override void DisposeScope()
        {
            IsDisposed = true;

            try
            {
                PerformDeterministicDisposal();
            }
            finally
            {
#if !NET45
                CurrentScope.Value = null;
#endif
            }
        }

        protected virtual void PerformDeterministicDisposal()
        {
            // Locking on the scope object is a hack to solve deadlock problems:
            //     a. The Cache class may create deadlocks: https://github.com/ninject/Ninject/issues/124
            //     b. Deadlock between ICache.Clear() and TryGet(IContext context): https://github.com/ninject/Ninject/issues/182
            // Deadlock may occur due to different lock acquisition order when Ninject.Extensions.Interception
            // package is involved with deterministic disposal:
            // Actor 1:
            //     1. Calls Cache.Forget method, where Cache.entries field is locked;
            //     2. Pipeline.Deactivate method is called that might call interceptors;
            //     3. Interceptors call IKernel.Resolve that locks on Context.cachedScope field;
            // Actor 2:
            //     1. Calls IKernel.Resolve that locks on Context.cachedScope field;
            //     2. Calls Cache.TryGet that locks on Cache.entries field.
            // So, locks on Context.cachedScope and Cache.entries fields are taken in different order,
            // and this lock statement is intended to enforce the acquisition of the Context.cachedScope
            // field first, and scope instance depends on the scope registration. Right now, we handle
            // InSingletonScope registrations by locking its scope instance, IKernel.
            lock (_kernel)
            {
                // Deterministic disposal implementation, please see
                // https://mono.software/2016/04/21/Ninject-ambient-scope-and-deterministic-dispose/
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}