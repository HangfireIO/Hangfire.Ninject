HangFire.Ninject
================

[HangFire](http://hangfire.io) background job activator based on 
[Ninject](http://ninject.org) IoC Container.

Installation
--------------

HangFire.Ninject is available as a NuGet Package. Type the following
command into NuGet Package Manager Console window to install it:

```
Install-Package HangFire.Ninject
```

Usage
------

In order to use the library, you should register it as your
JobActivator class:

```csharp
// Global.asax.cs or other file that initializes Ninject bindings.
public partial class MyApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
		var kernel = new StandardKernel();
		/* Register types */
		/* kernel.Bind<SomeInterface>().To<SomeImplementation>(); */
		
		JobActivator.Current = new NinjectJobActivator(kernel);
    }
}
```

HTTP Request warnings
-----------------------

Services registered with `InRequestScope()` directive **will be unavailable**
during job activation, you should re-register these services without this
hint.

`HttpContext.Current` is also **not available** during the job performance. 
Don't use it!