Hangfire.Ninject
================

[![Build status](https://ci.appveyor.com/api/projects/status/79opt6sesdam48yq)](https://ci.appveyor.com/project/odinserj/hangfire-ninject)

[Hangfire](http://hangfire.io) background job activator based on 
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

Hangfire.Ninject is available as a NuGet Package. Type the following
command into NuGet Package Manager Console window to install it:

```
Install-Package Hangfire.Ninject
```

Usage
------

The package provides an extension method for [OWIN bootstrapper](http://docs.hangfire.io/en/latest/users-guide/getting-started/owin-bootstrapper.html):

```csharp
app.UseHangfire(config =>
{
    var kernel = new StandardKernel();
    config.UseNinjectActivator(kernel);
});
```

In order to use the library outside of web application, set the static `JobActivator.Current` property:

```csharp
var kernel = new StandardKernel();
JobActivator.Current = new NinjectJobActivator(kernel);
```

HTTP Request warnings
-----------------------

Services registered with `InRequestScope()` directive **will be unavailable**
during job activation, you should re-register these services without this
hint.

`HttpContext.Current` is also **not available** during the job performance. 
Don't use it!
