using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using JollyBit.BS.Core.Utility;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace JollyBit.BS.Core.World.Actors
{
    public interface IActorComponent
    {
        IActor Actor { get; }
    }
    public interface IActor
    {
        int ActorId { get; }
    }
    public static class ActorExtensions
    {
        public static IActorComponent Get(this IActor actor, Type serviceType)
        {
            return (IActorComponent)Constants.Kernel.Get(serviceType, new Parameter("actor", actor, true));
        }
        public static T Get<T>(this IActor actor) where T : IActorComponent
        {
            return (T)actor.Get(typeof(T));
        }
        public static T Get<T>(this IActorComponent actor) where T : IActorComponent
        {
            return actor.Get<T>();
        }
        public static IBindingNamedWithOrOnSyntax<T> InActorScope<T>(this IBindingWhenInNamedWithOrOnSyntax<T> s) where T : IActorComponent
        {
            return s.InScope(context => (IActor)context.Parameters.First(parm => parm.Name == "actor").GetValue(context, context.Request.Target));
        }

    }
}
