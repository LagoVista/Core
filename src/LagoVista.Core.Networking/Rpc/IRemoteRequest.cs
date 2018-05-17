namespace LagoVista.Core.Networking.Rpc
{
    public interface IRemoteRequest : IRemoteMessage
    {
        void AddValue(string name, object value, bool ignoreDuplicates = false);
        T GetValue<T>(string name);
        object GetValue(string name);
    }
}
