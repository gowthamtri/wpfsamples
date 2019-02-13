using System;
using System.Reflection;
using Autodesk.REX.Framework;

namespace SamplePlugin
{
    public class RexCommon : IREXApplication4, IREXApplication3, IREXApplication2, IREXApplication
    {
        protected IREXApplication2 ApplicationRef;

        public RexCommon()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.currentDomain_AssemblyResolve);
            this.OnCreateApplication();
        }

        protected virtual void OnCreateApplication()
        {
        }

        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            REXConfiguration.Initialize(Assembly.GetExecutingAssembly());
            return REXAssemblies.Resolve(sender, args, REXConfiguration.Control.VersionName, Assembly.GetExecutingAssembly());
        }

        public bool Create(ref REXContext AppContext)
        {
            if (AppContext.Extension.Name == null || AppContext.Extension.Name == "")
                AppContext.Extension.Name = Assembly.GetExecutingAssembly().GetName().Name;
            return this.ApplicationRef.Create(ref AppContext);
        }

        public bool Modal(ref REXContext AppContext)
        {
            if (AppContext.Extension.Name == null || AppContext.Extension.Name == "")
                AppContext.Extension.Name = Assembly.GetExecutingAssembly().GetName().Name;
            return this.ApplicationRef.Modal(ref AppContext);
        }

        public void Close()
        {
            this.ApplicationRef.Close();
        }

        public object Command(ref REXCommand CommandStruct)
        {
            return this.ApplicationRef.Command(ref CommandStruct);
        }

        public object Control()
        {
            return this.ApplicationRef.Control();
        }

        public bool Create(ref REXApplicationContext AppContext)
        {
            return this.ApplicationRef.Create(ref AppContext);
        }

        public object Event(ref REXEvent EventStruct)
        {
            return this.ApplicationRef.Event(ref EventStruct);
        }

        public bool Modal(ref REXApplicationContext AppContext)
        {
            return this.ApplicationRef.Modal(ref AppContext);
        }

        public bool Show()
        {
            return this.ApplicationRef.Show();
        }

        public bool Create(REXContext AppContext)
        {
            return this.Create(ref AppContext);
        }

        public bool Modal(REXContext AppContext)
        {
            return this.Modal(ref AppContext);
        }

        public object GetService()
        {
            if (this.ApplicationRef is IREXApplication4)
                return ((IREXApplication4)this.ApplicationRef).GetService();
            return (object)null;
        }

        public Guid GetUniqueId()
        {
            if (this.ApplicationRef is IREXApplication4)
                return ((IREXApplication4)this.ApplicationRef).GetUniqueId();
            return Guid.Empty;
        }
    }
}
