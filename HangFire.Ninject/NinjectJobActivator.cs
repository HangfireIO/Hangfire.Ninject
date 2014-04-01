using System;
using Ninject;

namespace HangFire
{
    public class NinjectJobActivator : JobActivator
    {
        private readonly IKernel _kernel;

        public NinjectJobActivator(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            _kernel = kernel;
        }

        public override object ActivateJob(Type jobType)
        {
            return _kernel.Get(jobType);
        }
    }
}
