using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ToolingLib.Configuration
{
    public class PressInstance : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Id), IsKey = true, IsRequired = true)]
        public int Id
        {
            get { return (int)base[nameof(Id)]; }

            set { base[nameof(Id)] = value; }
        }

        [ConfigurationProperty(nameof(PressBar), IsRequired = false, DefaultValue = 1000)]
        public int PressBar
        {
            get { return (int)base[nameof(PressBar)]; }

            set { base[nameof(PressBar)] = value; }
        }
    }

    public class MagazineInstance : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Id), IsKey = true, IsRequired = true)]
        public int Id
        {
            get { return (int)base[nameof(Id)]; }

            set { base[nameof(Id)] = value; }
        }

        [ConfigurationProperty(nameof(FileLoader), IsRequired = false, DefaultValue = "C:\\Users\\etagliani\\source\\Recipes\\tools.json")]
        public string FileLoader
        {
            get { return (string)base[nameof(FileLoader)]; }

            set { base[nameof(FileLoader)] = value; }
        }
    }

    public class ServerConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(nameof(BaseUrl), IsRequired = false, DefaultValue = "localhost")]
        public string BaseUrl
        {
            get { return (string)base[nameof(BaseUrl)]; }

            set { base[nameof(BaseUrl)] = value; }
        }

        [ConfigurationProperty(nameof(Port), IsRequired = false, DefaultValue = "8080")]
        public string Port
        {
            get { return (string)base[nameof(Port)]; }

            set { base[nameof(Port)] = value; }
        }

        [ConfigurationProperty(nameof(ServiceEndpoint), IsRequired = false, DefaultValue = "Tooling")]
        public string ServiceEndpoint
        {
            get { return (string)base[nameof(ServiceEndpoint)]; }

            set { base[nameof(ServiceEndpoint)] = value; }
        }
    }

    public class JsonRecipeConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(nameof(FolderPath), IsRequired = true)]
        public string FolderPath
        {
            get { return (string)base[nameof(FolderPath)]; }

            set { base[nameof(FolderPath)] = value; }
        }
    }

    public class DataBaseConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(nameof(ConnectionString), IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base[nameof(ConnectionString)]; }

            set { base[nameof(ConnectionString)] = value; }
        }
    }

    public class SQLiteConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(nameof(SQLiteConnectionString), IsRequired = true)]
        public string SQLiteConnectionString
        {
            get { return (string)base[nameof(SQLiteConnectionString)]; }

            set { base[nameof(SQLiteConnectionString)] = value; }
        }
    }

    public class PressCollection : ConfigurationElementCollection
    {
        public PressInstance this[int index]
        {
            get
            {
                return (PressInstance)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new PressInstance this[string key]
        {
            get
            {
                return (PressInstance)BaseGet(key);
            }
            set
            {
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PressInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PressInstance)element).Id;
        }
    }

    public class MagazineCollection : ConfigurationElementCollection
    {
        public MagazineInstance this[int index]
        {
            get
            {
                return (MagazineInstance)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new MagazineInstance this[string key]
        {
            get
            {
                return (MagazineInstance)BaseGet(key);
            }
            set
            {
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MagazineInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MagazineInstance)element).Id;
        }
    }

    public class ToolingLibraryConfig : ConfigurationSection
    {
        [ConfigurationProperty(nameof(ServerConfiguration))]
        public ServerConfiguration ServerConfiguration
        {
            get
            {
                return (ServerConfiguration)this[nameof(ServerConfiguration)];
            }
        }

        [ConfigurationProperty(nameof(JsonRecipeConfiguration))]
        public JsonRecipeConfiguration JsonRecipeConfiguration
        {
            get
            {
                return (JsonRecipeConfiguration)this[nameof(JsonRecipeConfiguration)];
            }
        }

        [ConfigurationProperty(nameof(DataBaseConfiguration))]
        public DataBaseConfiguration DataBaseConfiguration
        {
            get
            {
                return (DataBaseConfiguration)this[nameof(DataBaseConfiguration)];
            }
        }

        [ConfigurationProperty(nameof(SQLiteConfiguration))]
        public SQLiteConfiguration SQLiteConfiguration
        {
            get
            {
                return (SQLiteConfiguration)this[nameof(SQLiteConfiguration)];
            }
        }

        [ConfigurationProperty(nameof(PressInstance))]
        [ConfigurationCollection(typeof(PressCollection))]
        public PressCollection PressCollection
        {
            get
            {
                return (PressCollection)this[nameof(PressInstance)];
            }
        }

        [ConfigurationProperty(nameof(MagazineInstance))]
        [ConfigurationCollection(typeof(MagazineCollection))]
        public MagazineCollection MagazineCollection
        {
            get
            {
                return (MagazineCollection)this[nameof(MagazineInstance)];
            }
        }
    }
}