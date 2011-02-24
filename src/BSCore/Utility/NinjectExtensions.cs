using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Parameters;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Planning.Targets;
using Ninject.Syntax;

namespace JollyBit.BS.Core.Utility
{
    public static class NinjectExtensions
    {
        /// <summary>
        /// Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The value for the parameter.</returns>
        public static object GetValue(this IParameter parameter, IContext context)
        {
            return parameter.GetValue(context, context.Request.Target);
        }
        /// <summary>
        /// Binds to existing binding
        /// </summary>
        public static IBindingWhenInNamedWithOrOnSyntax<T> ToBinding<T, TImplementation>(this IBindingToSyntax<T> syntax) where TImplementation : T
        {
            return syntax.ToMethod(context => context.Kernel.Get<TImplementation>());
        }
        /// <summary>
        /// Binds if and only if there is a parameter with the specified name
        /// </summary>
        public static IBindingInNamedWithOrOnSyntax<T> WhenExistsParameterNamed<T>(this IBindingWhenSyntax<T> syntax, string parameterName)
        {
            return syntax.When((request) => { return request.Parameters.FirstOrDefault(parm => parm.Name == parameterName) != null; });
        }
        /// <summary>
        /// Binds if and only if there is a parameter with the specified type
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <typeparam name="PT">The type of the parameter. Each parameter is tested if it is of type IParameter<PT>.</typeparam>
        public static IBindingInNamedWithOrOnSyntax<T> WhenExistsParameterWithType<T>(this IBindingWhenSyntax<T> syntax, Type parameterType)
        {
            return syntax.When((request) => { return request.Parameters.FirstOrDefault(parm => parm.GetValue(request.ParentContext).GetType() == parameterType) != null; });
        }
    }
}
