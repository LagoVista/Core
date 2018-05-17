using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface ISender
    {
        Task SendAsync(IAsyncMessage message);
    }

    public class ServiceBusSender : ISender
    {
        ISenderConnectionSettings _settings;
        public ServiceBusSender(ISenderConnectionSettings settings)
        {
            _settings = settings;
        }

        public Task SendAsync(IAsyncMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
