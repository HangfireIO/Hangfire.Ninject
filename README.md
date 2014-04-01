HangFire.Ninject
================

[HangFire](http://hangfire.io) background job activator based on 
[Ninject](http://ninject.org) IoC Container. It allows you to use instance
methods of classes that define parametrized constructors:

```csharp
public class EmailService
{
	private DbContext _context;
    private IEmailSender _sender;
	
	public EmailService(DbContext context, IEmailSender sender)
	{
		_context = context;
		_sender = sender;
	}
	
	public void Send(int userId, string message)
	{
		var user = _context.Users.Get(userId);
		_sender.Send(user.Email, message);
	}
}	

// Somewhere in the code
BackgroundJob.Enqueue<EmailService>(x => x.Send(1, "Hello, world!"));
```

Improve the testability of your jobs without static factories!

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