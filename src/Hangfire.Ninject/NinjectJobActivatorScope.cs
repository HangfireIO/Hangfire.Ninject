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
            // Deterministic disposal implementation, please see
            // https://mono.software/2016/04/21/Ninject-ambient-scope-and-deterministic-dispose/
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}