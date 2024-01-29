using System;
using Hangfire.Annotations;
using Ninject;

namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<NinjectJobActivator> UseNinjectActivator(
            [NotNull] this IGlobalConfiguration configuration, 
            [NotNull] IKernel kernel)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (kernel == null) throw new ArgumentNullException("kernel");

            return configuration.UseActivator(new NinjectJobActivator(kernel));
        }
    }
}
