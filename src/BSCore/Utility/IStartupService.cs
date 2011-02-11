using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace JollyBit.BS.Core.Utility
{
    public interface IStartupService
    {
        /// <summary>
        /// A list of types to be activated by Ninject on program startup.
        /// The types are activated in the order they occur in the list.
        /// </summary>
        IList<Type> StartupTypes { get; }
        /// <summary>
        /// Makes Ninject activate all the types found in the StartupTypes list.
        /// </summary>
        void ActivateStartupTypes();
    }
    internal class StartupService : IStartupService
    {
        IKernel _kernel;
        public StartupService(IKernel kernel)
        {
            _kernel = kernel;
            StartupTypes = new List<Type>();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _kernel.Dispose();
        }
        public IList<Type> StartupTypes { get; private set; }
        public void ActivateStartupTypes()
        {
            foreach (Type type in StartupTypes) 
                _kernel.Get(type);
        }
    }
    public static class StartupServiceExtensions
    {
        /// <summary>
        /// Registers a type to be activated by Ninject when the program starts.
        /// </summary>
        /// <param name="startupService">The type to be activated by Ninject at startup.</param>
        /// <param name="typeToRegister"></param>
        public static void RegisterStartupType(this IStartupService startupService, Type typeToRegister)
        {
            startupService.StartupTypes.Add(typeToRegister);
        }
        /// <summary>
        /// Registers a type to be activated by Ninject when the program starts.
        /// </summary>
        /// <typeparam name="T">The type to be activated by Ninject at startup.</typeparam>
        /// <param name="startupService"></param>
        public static void RegisterStartupType<T>(this IStartupService startupService)
        {
            startupService.StartupTypes.Add(typeof(T));
        }
    }
}
