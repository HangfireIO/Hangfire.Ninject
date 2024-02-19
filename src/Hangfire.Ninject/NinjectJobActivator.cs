using System;
using Ninject;

namespace Hangfire
{
    /// <summary>
    /// Hangfire Job Activator based on Ninject IoC Container.
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
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        /// <inheritdoc />
        public override object ActivateJob(Type jobType)
        {
            return _kernel.Get(jobType);
        }

#if NETSTANDARD2_0
        [Obsolete("Please use the BeginScope(JobActivatorContext) method instead.")]
#endif
        public override JobActivatorScope BeginScope()
        {
            return new NinjectJobActivatorScope(_kernel);
        }

#if NETSTANDARD2_0
        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return BeginScope();
#pragma warning restore CS0618 // Type or member is obsolete
        }
#endif
    }
}
