using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using System.IO;


namespace JollyBit.BS.Utility
{

    public interface IConfigSection { }

    public interface IConfigManager
    {
        T GetConfig<T>() where T : class, IConfigSection;
        void SaveConfig();
    }

    public class ConfigManager : IConfigManager
    {
        private List<IConfigSection> _configSections = new List<IConfigSection>();
        public List<IConfigSection> ConfigSections
        {
            get { return _configSections; }
            set { _configSections = value; }
        }

        public T GetConfig<T>() where T : class, IConfigSection
        {
            T configSection = _configSections.FirstOrDefault(section => section.GetType() == typeof(T)) as T;
            if (configSection == null)
            {
                configSection = BSCoreConstants.Kernel.Get<T>();
                _configSections.Add(configSection);
            }
            return configSection;
        }

        public void SaveConfig()
        {
            IFileSystem fileSystem = BSCoreConstants.Kernel.Get<IFileSystem>();
            fileSystem.DeleteFile("config.json");
            using (Stream stream = fileSystem.CreateFile("config.json"))
            {
                JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(ConfigManager));
                serializer.Serialize(this, stream);
            }
        }
    }
}
