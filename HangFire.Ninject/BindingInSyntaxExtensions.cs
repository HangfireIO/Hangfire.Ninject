using Ninject;
using Ninject.Infrastructure;
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
            return syntax.InScope(c => JobActivatorScope.Current);
        }
        
        /// <summary>
        /// Indicates that instances activated via the binding should be re-used
        /// within the same request (if available), or within the same background
        /// job type instance.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <param name="syntax">Binding syntax.</param>
        /// <returns>The syntax to define more information.</returns>
        public static IBindingNamedWithOrOnSyntax<T> InRequestOrBackgroundJobScope<T>(this IBindingInSyntax<T> syntax)
        {
            return syntax.InScope(context => StandardScopeCallbacks.Request(context) ?? JobActivatorScope.Current);
        }
    }
}