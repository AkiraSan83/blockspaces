using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using System.Xml.Serialization;
using System.IO;

namespace JollyBit.BS.Utility
{

    public interface IConfigSection { }

    public interface IConfigManager
    {
        T GetConfig<T>() where T : class, IConfigSection;
        void AddConfig(IConfigSection config);
        void SaveConfig();
    }

    public class ConfigManager : IConfigManager
    {
        [XmlElement("ConfigSections")]
        public List<ConfigSectionHeader> ConfigSectionsHeaders
        {
            get { return _configSections.Select(section => new ConfigSectionHeader(section)).ToList(); }
            set { _configSections = value.Select(section => section.ConfigSection).ToList(); }
        }

        private List<IConfigSection> _configSections = new List<IConfigSection>();
        [XmlIgnore]
        public List<IConfigSection> ConfigSections
        {
            get { return _configSections; }
            set { _configSections = value; }
        }

        public T GetConfig<T>() where T : class, IConfigSection
        {
            return _configSections.FirstOrDefault(section => section.GetType() == typeof(T)) as T;
        }

        public void AddConfig(IConfigSection config)
        {
            _configSections.Add(config);
        }

        public void SaveConfig()
        {
            IFileSystem fileSystem = BSCoreConstants.Kernel.Get<IFileSystem>();
            fileSystem.DeleteFile("config.xml");
            using (Stream stream = fileSystem.CreateFile("config.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigManager));
                serializer.Serialize(stream, this);
            }
        }
    }

    public class ConfigSectionHeader : IXmlSerializable
    {
        public IConfigSection ConfigSection;

        public ConfigSectionHeader() { }
        public ConfigSectionHeader(IConfigSection configSection)
        {
            ConfigSection = configSection;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            Type configSectionType = Type.GetType(reader.GetAttribute("ConfigSectionType"));
            XmlSerializer serializer = new XmlSerializer(configSectionType);
            ConfigSection = serializer.Deserialize(reader) as IConfigSection;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("ConfigSectionType", ConfigSection.GetType().FullName);
            XmlSerializer serializer = new XmlSerializer(ConfigSection.GetType());
            serializer.Serialize(writer, ConfigSection);
        }
    }
}
