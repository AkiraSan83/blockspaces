using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using System.IO;
using Ninject.Extensions.Logging;


namespace JollyBit.BS.Core.Utility
{
    public interface IConfigManager
    {
        void SaveConfig();
        IEnumerable<object> ConfigSections { get; }
    }

    public class ConfigManager : IConfigManager, IStartable
    {
        private readonly IKernel _kernel;
        private readonly IFileSystem _fileSystem;
        private readonly FileReference _fileReferance;
        private readonly ILogger _logger;
        public List<object> ConfigSections = new List<object>();
        public ConfigManager() { }
        [Inject]
        public ConfigManager(IKernel kernel, IFileSystem fileSystem, FileReference fileReference, ILogger logger)
        {
            _kernel = kernel;
            _fileSystem = fileSystem;
            _fileReferance = fileReference;
            _logger = logger;
        }

        public void Start()
        {
            //Load ConfigSections from file
            ConfigSections.Clear();
            using (Stream stream = _fileSystem.OpenFile(_fileReferance))
            {
                if (stream != null)
                {
                    //deserialize config manager
                    JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(ConfigManager));
                    ConfigManager manager = null;
                    try
                    {
                        manager = serializer.Deserialize(stream) as ConfigManager;
                        if (manager == null) throw new System.Exception("Failed to deserialize ConfigManager");
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Could not deserialize config. The most likely cause is that the config is malformed. A new config will be created. The old config has been renamed with a .old extension appended.");
                        stream.Close();
                        _fileSystem.Copy(_fileReferance, _fileReferance + ".old", true);
                    }
                    //Add deserialized sections to list
                    if (manager != null)
                        foreach (var configSection in manager.ConfigSections)
                            ConfigSections.Add(configSection);
                }
                else _logger.Info("Could not open config. A new config will be created.");
            }
            //Create ConfigSections that were not in file
            foreach (var type in ReflectionHelper.GetDecoratedTypes<ConfigSectionAttribute>())
            {
                if (ConfigSections.FirstOrDefault(c => c.GetType() == type) == null)
                {
                    ConfigSections.Add(_kernel.Get(type));
                }
            }
            //Register Ninject bindings for ConfigSection types
            foreach (var configSection in ConfigSections)
            {
                _kernel.Bind(configSection.GetType()).ToConstant(configSection);
            }
        }

        public void Stop()
        {
            _fileSystem.DeleteFile(_fileReferance);
            using (Stream stream = _fileSystem.CreateFile(_fileReferance))
            {
                JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(ConfigManager));
                serializer.Serialize(this, stream);
            }
        }

        public void SaveConfig()
        {
            Stop();
        }


        IEnumerable<object> IConfigManager.ConfigSections
        {
            get { return ConfigSections; }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigSectionAttribute : Attribute
    {

    }
}
