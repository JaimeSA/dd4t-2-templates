using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DD4T.Templates.Base;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Tridion.Logging;

namespace DD4T.Templates
{
    [TcmTemplateTitle("Override DD4T")]
    [TcmTemplateParameterSchema("resource:DD4T.Templates.Resources.Schemas.Override DD4T Behavior.xsd")]
    public class Override: ITemplate
    {
        TemplatingLogger Log = TemplatingLogger.GetLogger(typeof(Override));

        private Package _package;
        private Engine _engine;

        public Package Package
        {
            get { return _package; }

        }

        public Engine Engine
        {
            get { return _engine; }
        }



        public void Transform(Engine engine, Package package)
        {
            _package = package;
            _engine = engine;

            TemplatingLogger.GetLogger(typeof(Override));
            //1. Get the DLLs in the package

            String className = Package.GetValue("CustomBuildManager");

            if (!String.IsNullOrEmpty(className))
            {
                package.PushItem("CustomBuildManager.Class", Package.CreateStringItem(ContentType.String, className));

                Log.Debug(string.Format("Using Custom Build Manager: {0}.", className));
            }
            else
            {
                Log.Debug("Using default DD4T Build Manager");
            }
        }
    }
}
