using System;
using Ninject;
using Ninject.Activation;
using Ninject.Syntax;

namespace Hangfire
{
    /// <summary>
    /// Defines extension methods for the <see cref="IKernel"/> type.
    /// </summary>
    public static class BindingSyntaxExtensions
    { 
        /// <summary>
        /// Indicates that instances activated via the binding should be re-used
        /// within the same background job type instance.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <param name="syntax">Binding syntax.</param>
        /// <returns>The syntax to define more information.</returns>
        public static IBindingNamedWithOrOnSyntax<T> InBackgroundJobScope<T>(this IBindingInSyntax<T> syntax)
        { 
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            return syntax.InScope(c => NinjectJobActivatorScope.Current);
        }

        /// <summary>
        /// Indicates that instances activated via the binding should be re-used
        /// within the same named scope (if available), or within the same background
        /// job type instance.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <param name="syntax">Binding syntax.</param>
        /// <param name="scopeCallback">A callback that returns the target scope.</param>
        /// <returns>The syntax to define more information.</returns>
        public static IBindingNamedWithOrOnSyntax<T> InNamedOrBackgroundJobScope<T>(this IBindingInSyntax<T> syntax, Func<IContext, object> scopeCallback)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            return syntax.InScope(context => scopeCallback(context) ?? NinjectJobActivatorScope.Current);
        }
    }
}