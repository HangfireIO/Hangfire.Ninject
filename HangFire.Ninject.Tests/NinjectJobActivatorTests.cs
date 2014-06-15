using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;

namespace HangFire.Ninject.Tests
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
            var activator = new NinjectJobActivator(_kernel);
            Assert.IsInstanceOfType(activator, typeof(JobActivator));
        }

        [TestMethod]
        public void ActivateJob_CallsNinject()
        {
            _kernel.Bind<string>().ToConstant("called");
            var activator = new NinjectJobActivator(_kernel);

            var result = activator.ActivateJob(typeof (string));

            Assert.AreEqual("called", result);
        }

        [TestMethod]
        public void UseNinjectActivator_PassesCorrectActivator()
        {
            var configuration = new Mock<IBootstrapperConfiguration>();
            var kernel = new Mock<IKernel>();

            configuration.Object.UseNinjectActivator(kernel.Object);

            configuration.Verify(x => x.UseActivator(It.IsAny<NinjectJobActivator>()));
        }
    }
}
