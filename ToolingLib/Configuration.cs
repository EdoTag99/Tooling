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

    public class MagazineToolInstance : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Id), IsKey = true, IsRequired = false)]
        public int Id
        {
            get { return (int)base[nameof(Id)]; }

            set { base[nameof(Id)] = value; }
        }

        [ConfigurationProperty(nameof(Width), IsRequired = false, DefaultValue = 50)]
        public int Width
        {
            get { return (int)base[nameof(Width)]; }

            set { base[nameof(Width)] = value; }
        }

        [ConfigurationProperty(nameof(Count), IsRequired = false, DefaultValue = 0)]
        public int Count
        {
            get { return (int)base[nameof(Count)]; }

            set { base[nameof(Count)] = value; }
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

        [ConfigurationProperty(nameof(FileLoader), IsRequired = true)]
        public string FileLoader
        {
            get { return (string)base[nameof(FileLoader)]; }

            set { base[nameof(FileLoader)] = value; }
        }

        [ConfigurationProperty(nameof(MagazineToolCollection))]
        [ConfigurationCollection(typeof(MagazineToolCollection), AddItemName = nameof(MagazineToolInstance))]
        public MagazineToolCollection MagazineToolCollection
        {
            get
            {
                return (MagazineToolCollection)this[nameof(MagazineToolCollection)];
            }
        }
    }

    public class MagazineToolCollection : ConfigurationElementCollection
    {
        public MagazineToolInstance this[int index]
        {
            get
            {
                return (MagazineToolInstance)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new MagazineToolInstance this[string key]
        {
            get
            {
                return (MagazineToolInstance)BaseGet(key);
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
            return new MagazineToolInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MagazineToolInstance)element).Id;
        }
    }

    public class ServiceConfiguration : ConfigurationElement
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

    public class ToolInstance : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Id), IsKey = true, IsRequired = false)]
        public int Id
        {
            get { return (int)base[nameof(Id)]; }

            set { base[nameof(Id)] = value; }
        }

        [ConfigurationProperty(nameof(Width), IsRequired = false, DefaultValue = 50)]
        public int Width
        {
            get { return (int)base[nameof(Width)]; }

            set { base[nameof(Width)] = value; }
        }

        [ConfigurationProperty(nameof(Position), IsRequired = false, DefaultValue = 0)]
        public int Position
        {
            get { return (int)base[nameof(Position)]; }

            set { base[nameof(Position)] = value; }
        }
    }

    public class ToolCollection : ConfigurationElementCollection
    {
        public ToolInstance this[int index]
        {
            get
            {
                return (ToolInstance)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new ToolInstance this[string key]
        {
            get
            {
                return (ToolInstance)BaseGet(key);
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
            return new ToolInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ToolInstance)element).Id;
        }
    }

    public class RecipeInstance : ConfigurationElement
    {
        [ConfigurationProperty(nameof(Id), IsKey = true, IsRequired = true)]
        public int Id
        {
            get { return (int)base[nameof(Id)]; }

            set { base[nameof(Id)] = value; }
        }

        [ConfigurationProperty(nameof(RecipeName), IsRequired = true)]
        public string RecipeName
        {
            get { return (string)base[nameof(RecipeName)]; }

            set { base[nameof(RecipeName)] = value; }
        }

        [ConfigurationProperty(nameof(ToolCollection))]
        [ConfigurationCollection(typeof(ToolCollection), AddItemName = nameof(ToolInstance))]
        public ToolCollection ToolCollection
        {
            get
            {
                return (ToolCollection)this[nameof(ToolCollection)];
            }
        }
    }

    public class RecipeCollection : ConfigurationElementCollection
    {
        public RecipeInstance this[int index]
        {
            get
            {
                return (RecipeInstance)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new RecipeInstance this[string key]
        {
            get
            {
                return (RecipeInstance)BaseGet(key);
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
            return new RecipeInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecipeInstance)element).Id;
        }
    }

    public class ToolingLibraryConfig : ConfigurationSection
    {
        [ConfigurationProperty(nameof(ServiceConfiguration))]
        public ServiceConfiguration ServiceConfiguration
        {
            get
            {
                return (ServiceConfiguration)this[nameof(ServiceConfiguration)];
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

        [ConfigurationProperty(nameof(PressCollection))]
        [ConfigurationCollection(typeof(PressCollection), AddItemName = nameof(PressInstance))]
        public PressCollection PressCollection
        {
            get
            {
                return (PressCollection)this[nameof(PressCollection)];
            }
        }

        [ConfigurationProperty(nameof(MagazineCollection))]
        [ConfigurationCollection(typeof(MagazineCollection), AddItemName = nameof(MagazineInstance))]
        public MagazineCollection MagazineCollection
        {
            get
            {
                return (MagazineCollection)this[nameof(MagazineCollection)];
            }
        }

        [ConfigurationProperty(nameof(RecipeCollection))]
        [ConfigurationCollection(typeof(RecipeCollection), AddItemName = nameof(RecipeInstance))]
        public RecipeCollection RecipeCollection
        {
            get
            {
                return (RecipeCollection)this[nameof(RecipeCollection)];
            }
        }
    }
}