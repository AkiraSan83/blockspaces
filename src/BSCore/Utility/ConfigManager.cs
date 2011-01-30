using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using System.IO;


namespace JollyBit.BS.Core.Utility
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
                configSection = Constants.Kernel.Get<T>();
                _configSections.Add(configSection);
            }
            return configSection;
        }

        public void SaveConfig()
        {
            IFileSystem fileSystem = Constants.Kernel.Get<IFileSystem>();
            fileSystem.DeleteFile("ClientConfig.json");
            using (Stream stream = fileSystem.CreateFile("ClientConfig.json"))
            {
                JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(ConfigManager));
                serializer.Serialize(this, stream);
            }
        }
    }
}
