namespace LagoVista.Core.Networking.Rpc
{
    public interface IRequest : IMessage
    {
        void AddValue(string name, object value, bool ignoreDuplicates = false);
        T GetValue<T>(string name);
        object GetValue(string name);
    }
}
