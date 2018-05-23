using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /* This is used so we can somewhere create a class that has connnections settings specific to service bus (or whatever */
    public interface ISenderConnectionSettings
    {
        string ServiceBusConnectionString { get; set; } 
        string DestinationEntityPath { get; set; }

        IConnectionSettings ConnectionSettings { get; set; }
    }

    public interface IListenerConnectionSettings
    {
        string ServiceBusConnectionString { get; set; }
        string SourceEntityPath { get; set; }
        string SubscriptionPath { get; set; }

        IConnectionSettings ConnectionSettings { get; set; }
    }


    public interface IRequestBrokerConnectionSettings
    {
        IConnectionSettings ConnectionSettings { get; set; }
    }
}
