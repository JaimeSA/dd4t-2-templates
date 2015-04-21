using DD4T.ContentModel.Contracts.Serializing;
using DD4T.Serialization;
using DD4T.Templates.Base.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using DD4T.Templates.Base.Contracts;

namespace DD4T.Templates.Base
{
    public abstract class BaseTemplate : ITemplate
    {
        public BaseTemplate()
        {
            Log = TemplatingLogger.GetLogger(typeof(BaseTemplate));
        }

        public BaseTemplate(TemplatingLogger log)
        {
            Log = log;
        }

        protected TemplatingLogger Log { get; set; }

        private BaseSerializerService _serializerService = null;
        protected ISerializerService SerializerService
        {
            get
            {
                if (_serializerService == null)
                {
                    if (Package == null)
                    {
                        throw new Exception("Template has not been properly initialized (Package is not set)");
                    }
                    Item serializerType = Package.GetByName("serializer-type");
                    if (serializerType != null) // another template has set a serializertype, let's use that!
                    {
                        Type t = Type.GetType(serializerType.GetAsString());
                        _serializerService = Activator.CreateInstance(t) as BaseSerializerService;
                    }
                    else
                    {
                        _serializerService = FindBestService();
                        serializerType = Package.CreateStringItem(ContentType.Text, _serializerService.GetType().FullName);
                        Package.PushItem("serializer-type", serializerType);
                    }
                    Item compressionEnabled = Package.GetByName("compression-enabled");
                    if (compressionEnabled != null) // another template has set a compression mode, let's use that!
                    {
                        _serializerService.SerializationProperties = new SerializationProperties() { CompressionEnabled = "yes" == compressionEnabled.GetAsString() };
                    }
                    else
                    {
                        _serializerService.SerializationProperties = new SerializationProperties() { CompressionEnabled = Manager.BuildProperties.CompressionEnabled };
                        compressionEnabled = Package.CreateStringItem(ContentType.Text, Manager.BuildProperties.CompressionEnabled ? "yes" : "no");
                        Package.PushItem("compression-enabled", compressionEnabled);
                    }
                }
                return _serializerService;
            }
        }

        private BaseSerializerService FindBestService()
        {
            BaseSerializerService s;
            if (Manager.BuildProperties.SerializationFormat == SerializationFormat.JSON)
            {
                s = new JSONSerializerService();
                if (s.IsAvailable())
                {
                    return s;
                }
            }

            if (Manager.BuildProperties.SerializationFormat == SerializationFormat.XML)
            {
                s = new XmlSerializerService();
                if (s.IsAvailable())
                {
                    return s;
                }
            }

            // service for the configured serialization format is unavailable, pick one of the available services
            s = new JSONSerializerService();
            if (s.IsAvailable())
            {
                return s;
            }

            s = new XmlSerializerService();
            if (s.IsAvailable())
            {
                return s;
            }

            throw new Exception("Unsupported serialization format: " + Manager.BuildProperties.SerializationFormat);
        }

        protected Package Package { get; set; }
        protected Engine Engine { get; set; }

        private IBuildManager _buildManager = null;
        public IBuildManager Manager
        {
            get
            {
                
                if (_buildManager == null)
                {
                    _buildManager = new BuildManager(Package, Engine);
                    
                    var bm = CheckOverride();
                    if (bm != null)
                    {
                        _buildManager = bm;
                    }
                    _buildManager.SerializerService = SerializerService;
                    
                }
                return _buildManager;
            }
            set
            {
                _buildManager = value;
            }
        }

        public abstract void Transform(Engine engine, Package package);

        protected bool HasPackageValue(Package package, string key)
        {
            foreach (KeyValuePair<string, Item> kvp in package.GetEntries())
            {
                if (kvp.Key.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }

        public static string DD4TContextVariableKey = "RenderedByDD4T";

        public IBuildManager CheckOverride()
        {
            String className = Package.GetValue("CustomBuildManager.Class");
            Log.Debug("Checking Override...");
            if (!String.IsNullOrEmpty(className))
            {
                Log.Debug("Loading object " + className);
                DD4T.Templates.Base.Contracts.IBuildManager bm=null;
                try
                {
                        
                    //Assembly customDll = Assembly.Load(ms.ToArray());
                    Type type = Type.GetType(className);
                    if (type != null)
                    {
                        bm = Activator.CreateInstance(type) as DD4T.Templates.Base.Contracts.IBuildManager;
                        if (bm != null)
                        {
                            Log.Info("Using Build Manager: " + bm.GetType().Name);
                            bm.Initialize(Package, Engine);
                        }
                        else
                        {
                            Log.Warning("Build Manager is null: " + className);
                        }
                    }
                    else
                    {
                        Log.Warning("Class Not found: " + className);
                    }
                        
                }
                catch (Exception ex)
                {
                    if (ex is System.Reflection.ReflectionTypeLoadException)
                    {
                        var typeLoadException = ex as ReflectionTypeLoadException;
                        var loaderExceptions = typeLoadException.LoaderExceptions;
                        Log.Error(typeLoadException.Message);
                        foreach (var e in loaderExceptions)
                        {
                            Log.Error(e.Message);
                        }
                    }
                    else
                    {
                        Log.Error(ex.Message);
                    }
                }
                return bm;
            }
            return null;
        }
    }


}
