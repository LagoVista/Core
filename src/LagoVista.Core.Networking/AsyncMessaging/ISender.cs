using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface ISender
    {
        Task SendAsync(IMessage message);
    }

    public class ServiceBusSender : ISender
    {
        ISenderConnectionSettings _settings;
        public ServiceBusSender(ISenderConnectionSettings settings)
        {
            _settings = settings;
        }

        public Task SendAsync(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
