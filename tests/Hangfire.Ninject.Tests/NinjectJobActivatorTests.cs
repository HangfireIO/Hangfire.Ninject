using System;
using Moq;
using Ninject;
using Xunit;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Hangfire.Ninject.Tests
{
    public class NinjectJobActivatorTests
    {
        private readonly IKernel _kernel = new StandardKernel();

        [Fact]
        public void Ctor_ThrowsAnException_WhenKernelIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new NinjectJobActivator(null));
            Assert.Equal("kernel", exception.ParamName);
        }

        [Fact]
        public void Class_IsBasedOnJobActivator()
        {
            var activator = CreateActivator();
            Assert.IsAssignableFrom<JobActivator>(activator);
        }

        [Fact]
        public void ActivateJob_CallsNinject()
        {
            _kernel.Bind<string>().ToConstant("called");
            var activator = CreateActivator();

            var result = activator.ActivateJob(typeof (string));

            Assert.Equal("called", result);
        }

        [Fact]
        public void InstanceRegisteredWith_TransientScope_IsNotDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InTransientScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.NotNull(instance);
                Assert.False(disposable.Disposed);
            }

            Assert.False(disposable.Disposed);
        }

        [Fact]
        public void InstanceRegisteredWith_SingletonScope_IsNotDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InSingletonScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.NotNull(instance);
                Assert.False(disposable.Disposed);
            }

            Assert.False(disposable.Disposed);
        }

        [Fact]
        public void InBackgroundJobScope_RegistersSameServiceInstance_ForTheSameScopeInstance()
        {
            _kernel.Bind<object>().ToMethod(c => new object()).InBackgroundJobScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance1 = scope.Resolve(typeof(object));
                var instance2 = scope.Resolve(typeof(object));

                Assert.Same(instance1, instance2);
            }
        }

        [Fact]
        public void InBackgroundJobScope_RegistersDifferentServiceInstances_ForDifferentScopeInstances()
        {
            _kernel.Bind<object>().ToMethod(c => new object()).InBackgroundJobScope();
            var activator = CreateActivator();

            object instance1;
            using (var scope1 = activator.BeginScope())
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = activator.BeginScope())
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void InstanceRegisteredWith_InBackgroundJobScope_IsDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InBackgroundJobScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.NotNull(instance);
            }

            Assert.True(disposable.Disposed);
        }

        [Fact]
        public void InstanceRegisteredWith_InNamedOrBackgroundJobScope_IsDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            var activator = CreateActivator();
            using (var scope = activator.BeginScope())
            {
                // ReSharper disable once AccessToDisposedClosure
                _kernel.Bind<Disposable>().ToMethod(c => disposable).InNamedOrBackgroundJobScope(ctx => scope);

                var instance = scope.Resolve(typeof(Disposable));
                Assert.NotNull(instance);
            }

            Assert.True(disposable.Disposed);
        }

#if !NET6_0
#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void UseNinjectActivator_PassesCorrectActivator()
        {
            var configuration = new Mock<IBootstrapperConfiguration>();
            var kernel = new Mock<IKernel>();

            configuration.Object.UseNinjectActivator(kernel.Object);

            configuration.Verify(x => x.UseActivator(It.IsAny<NinjectJobActivator>()));
        }
#pragma warning restore CS0618 // Type or member is obsolete
#endif

        private NinjectJobActivator CreateActivator()
        {
            return new NinjectJobActivator(_kernel);
        }

        private sealed class Disposable : IDisposable
        {
            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
