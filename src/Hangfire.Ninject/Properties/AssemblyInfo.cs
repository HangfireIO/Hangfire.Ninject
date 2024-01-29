using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Hangfire.Ninject")]
[assembly: AssemblyDescription("Ninject IoC Container support for Hangfire (background job framework for .NET applications).")]
[assembly: Guid("91f25a4e-65e7-4d9c-886e-33a6c82b14c4")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("Hangfire.Ninject.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]