using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IRequestListener
    {
        void Start();
    }

    public class SbRequestListener: IRequestListener
    {
        public SbRequestListener(IRequestBroker requestBroker, IConnectionSettings connectionSettings, ILogger logger)
        {

        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
