Hangfire.Ninject [![Build status](https://ci.appveyor.com/api/projects/status/79opt6sesdam48yq)](https://ci.appveyor.com/project/odinserj/hangfire-ninject)
================

[Ninject](http://www.ninject.org/) support for [Hangfire](http://hangfire.io). Provides an implementation of the `JobActivator` class and binding extensions, allowing you to use Ninject IoC container to **resolve job type instances** as well as **control the lifetime** of the related dependencies.

Installation
--------------

*Hangfire.Ninject* is available as a NuGet Package. Type the following command into NuGet Package Manager Console window to install it:

```
Install-Package Hangfire.Ninject
```

Usage
------

The package provides an extension method for the `IGlobalConfiguration` interface, so you can enable Ninject integration using the `GlobalConfiguration` class.

```csharp
var kernel = new StandardKernel();
// kernel.Bind...

GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
```

After invoking the methods above, Ninject-based implementation of the `JobActivator` class will be used to resolve job type instances and all their dependencies during the background processing.

### Re-using Dependencies

Sometimes it is necessary to re-use instances that are already created, such as database connection, unit of work, etc. Thanks to the [custom object scopes](https://github.com/ninject/Ninject/wiki/Object-Scopes) feature of Ninject, you are able to do this without having to implement anything via code.

*Hangfire.Ninject* provides a [custom scope](https://github.com/ninject/Ninject/wiki/Object-Scopes#custom-scopes) to allow you to limit the object scope to the **current background job processing**, just call the `InBackgroundJobScope` extension method in your binding logic:

```csharp
kernel.Bind<Database>().ToSelf().InBackgroundJobScope();
```

### Multiple Scopes/Bindings

It's likely that you want to define multiple scopes for your unit-of-work dependencies, one for HTTP request, etc. If you want to use one binding for both these objects and background jobs, please use the following method:

```csharp
kernel.Bind<JobClass>().ToSelf().InNamedOrBackgroundJobScope(context=> scopeObject);
```

If you are using InRequestScope and want to use one binding for both HTTP request and background job you need to add your own callback to determine if the HttpContext is still valid.

```csharp
kernel.Bind<JobClass>().ToSelf().InNamedOrBackgroundJobScope(context => context.Kernel.Components.GetAll<INinjectHttpApplicationPlugin>().Select(c => c.GetRequestScope(context)).FirstOrDefault(s => s != null));
```

If you are using other scopes in your application, you can construct your own scopes. For example, if you want to define a binding in a background job scope with fallback to thread scope, please use the Ninject's `InScope` method:

```csharp
kernel.Bind<JobClass>().ToSelf().InScope(ctx => JobActivatorScope.Current ?? StandardScopeCallbacks.Thread(ctx));
```

In this case, the instance of the `JobClass` class will be re-used within the HTTP request processing, as well as within the background job processing.

All the `IDisposable` instances of dependencies registered within `JobActivatorScope.Current` will be disposed at the end of background job processing, as written in the *Deterministic Disposal* article.

### Deterministic Disposal

All the dependencies that implement the `IDisposable` interface are disposed as soon as current background job is performed, but **only when they were registered using the `InBackgroundJobScope` method**. For other cases, Ninject itself is responsible for disposing instances, so please read the [implications of the Cache and Collect system](https://github.com/ninject/ninject/wiki/Changes-in-Ninject-2).

For most typical cases, you can call the `InBackgroundJobScope` method on a job type binding and implement the `Dispose` method that will dispose all the dependencies manually:

```csharp
public class JobClass : IDisposable
{
    public JobClass(Dependency dependency) { /* ... */ }

    public Dispose()
    {
        _dependency.Dispose();
    }
}
```

```csharp
kernel.Bind<JobClass>().ToSelf().InBackgroundJobScope();
kernel.Bind<Dependency>().ToSelf();
```

HTTP Request warnings
-----------------------

Services registered with `InRequestScope()` directive **will be unavailable** during job activation, you should re-register these services without this hint.

**`HttpContext.Current` is also not available during the job performance. Don't use it!**
