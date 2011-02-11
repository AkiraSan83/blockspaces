using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace JollyBit.BS.Core.Utility
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetLoadedTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    yield return type;
        }
        public static bool Inherits<T>(this Type type)
        {
            return type.Inherits(typeof(T));
        }
        public static bool Inherits(this Type type, Type subclass)
        {
            return type.IsSubclassOf(subclass);
        }
        public static bool Implements<T>(this Type type)
        {
            return type.Implements(typeof(T));
        }
        public static bool Implements(this Type type, Type implementedInterface)
        {
            return type.IsAssignableFrom(implementedInterface);
        }
        public static IEnumerable<Type> GetSubclasses(this Type type)
        {
            return GetLoadedTypes().Where(t => t.Inherits(type));
        }
        public static IEnumerable<Type> GetImplementers(this Type type)
        {
            return GetLoadedTypes().Where(t => t.Implements(type));
        }
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true)
                .FirstOrDefault() as T;
        }
        public static IEnumerable<T> GetAttributes<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true)
                .Select(a => a as T);
        }
        public static IEnumerable<Type> GetDecoratedTypes<T>() where T : Attribute
        {
            return GetLoadedTypes().Where(t => t.IsDefined(typeof(T), true));
        }
    }
}
