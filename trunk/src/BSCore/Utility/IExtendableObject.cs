using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Utility
{
    public interface IExtendableObject
    {
        IDictionary<Type, object> Components { get; }
    }
    public static class ComponentExtensionMethods
    {
        public static T GetExtension<T>(this IExtendableObject self) where T : class
        {
            object component;
            if (self.Components.TryGetValue(typeof(T), out component))
                return component as T;
            else 
                return null;
        }
        public static void Extend<T>(this IExtendableObject self, T extender, bool replaceCurrentExtender)
        {
            if (replaceCurrentExtender)
            {
                self.Components.Remove(typeof(T));
            }
            self.Components.Add(typeof(T), extender);
        }
    }
}
