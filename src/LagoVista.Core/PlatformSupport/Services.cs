using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public static class Services
    {
        public static IImaging Imaging { get { return IOC.SLWIOC.Get<IImaging>(); } }
        public static IStorageService Storage { get { return IOC.SLWIOC.Get<IStorageService>(); } }
        public static IDispatcherServices DispatcherServices { get { return IOC.SLWIOC.Get<IDispatcherServices>(); } }
        public static ITimerFactory TimerFactory { get { return IOC.SLWIOC.Get<ITimerFactory>(); } }
        public static INetworkService Network { get { return IOC.SLWIOC.Get<INetworkService>(); } }
        public static ILogger Logger { get { return IOC.SLWIOC.Get<ILogger>(); } }
        public static IBindingHelper BindingHelper { get { return IOC.SLWIOC.Get<IBindingHelper>(); } }
        public static IDirectoryServices DirectoryServices { get { return IOC.SLWIOC.Get<IDirectoryServices>(); } }
    }
}
