using System;
using Ninject;

namespace HangFire
{
    /// <summary>
    /// HangFire Job Activator based on Ninject IoC Container.
    /// </summary>
    public class NinjectJobActivator : JobActivator
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectJobActivator"/>
        /// class with a given Ninject Kernel.
        /// </summary>
        /// <param name="kernel">Kernel that will be used to create instance
        /// of classes during job activation process.</param>
        public NinjectJobActivator(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            _kernel = kernel;
        }

        /// <inheritdoc />
        public override object ActivateJob(Type jobType)
        {
            return _kernel.Get(jobType);
        }
    }
}
