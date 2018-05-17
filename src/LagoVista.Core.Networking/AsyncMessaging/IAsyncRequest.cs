namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequest : IAsyncMessage
    {
        void AddValue(string name, object value, bool ignoreDuplicates = false);
        T GetValue<T>(string name);
        object GetValue(string name);
    }
}
