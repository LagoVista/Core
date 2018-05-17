using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface IRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the IsRemotableAttribute attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        void RegisterSubject<T>(T subject) where T : class;

        Task StartAsync();

        Task StopAsync();
    }
}
