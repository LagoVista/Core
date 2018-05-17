using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface ISender
    {
        Task SendAsync(IMessage message);
    }

    public class ServiceBusSender : ISender
    {
        IProxyConnectionSettings _settings;
        public ServiceBusSender(IProxyConnectionSettings settings)
        {
            _settings = settings;
        }

        public Task SendAsync(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
