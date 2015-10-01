using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;

namespace Hangfire.Ninject.Tests
{
    [TestClass]
    public class NinjectJobActivatorTests
    {
        private IKernel _kernel;

        [TestInitialize]
        public void SetUp()
        {
            _kernel = new StandardKernel();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsAnException_WhenKernelIsNull()
        {
// ReSharper disable once UnusedVariable
            var activator = new NinjectJobActivator(null);
        }

        [TestMethod]
        public void Class_IsBasedOnJobActivator()
        {
            var activator = CreateActivator();
            Assert.IsInstanceOfType(activator, typeof(JobActivator));
        }

        [TestMethod]
        public void ActivateJob_CallsNinject()
        {
            _kernel.Bind<string>().ToConstant("called");
            var activator = CreateActivator();

            var result = activator.ActivateJob(typeof (string));

            Assert.AreEqual("called", result);
        }

        [TestMethod]
        public void InstanceRegisteredWith_TransientScope_IsNotDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InTransientScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.IsFalse(disposable.Disposed);
            }

            Assert.IsFalse(disposable.Disposed);
        }

        [TestMethod]
        public void InstanceRegisteredWith_SingletonScope_IsNotDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InSingletonScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.IsFalse(disposable.Disposed);
            }

            Assert.IsFalse(disposable.Disposed);
        }

        [TestMethod]
        public void InBackgroundJobScope_RegistersSameServiceInstance_ForTheSameScopeInstance()
        {
            _kernel.Bind<object>().ToMethod(c => new object()).InBackgroundJobScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance1 = scope.Resolve(typeof(object));
                var instance2 = scope.Resolve(typeof(object));

                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
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

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void InstanceRegisteredWith_InBackgroundJobScope_IsDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InBackgroundJobScope();
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
            }

            Assert.IsTrue(disposable.Disposed);
        }

        [TestMethod]
        public void InstanceRegisteredWith_InNamedOrBackgroundJobScope_IsDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            var activator = CreateActivator();
            using (var scope = activator.BeginScope())
            {
            _kernel.Bind<Disposable>().ToMethod(c => disposable).InNamedOrBackgroundJobScope(ctx=>scope);

                var instance = scope.Resolve(typeof(Disposable));
            }

            Assert.IsTrue(disposable.Disposed);
        }

        [TestMethod]
        public void UseNinjectActivator_PassesCorrectActivator()
        {
            var configuration = new Mock<IBootstrapperConfiguration>();
            var kernel = new Mock<IKernel>();

            configuration.Object.UseNinjectActivator(kernel.Object);

            configuration.Verify(x => x.UseActivator(It.IsAny<NinjectJobActivator>()));
        }

        private NinjectJobActivator CreateActivator()
        {
            return new NinjectJobActivator(_kernel);
        }

        class Disposable : IDisposable
        {
            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}
