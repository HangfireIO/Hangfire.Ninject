using Ninject;

namespace Hangfire
{
    public static class NinjectBootstrapperConfigurationExtensions
    {
        /// <summary>
        /// Tells bootstrapper to use the specified Ninject
        /// kernel as a global job activator.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="kernel">Ninject kernel that will be used to activate jobs</param>
        public static void UseNinjectActivator(
            this IBootstrapperConfiguration configuration,
            IKernel kernel)
        {
            configuration.UseActivator(new NinjectJobActivator(kernel));
        }
    }
}
