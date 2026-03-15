  Example usage in Startup.ConfigureServices:

        services.AddRabbitMqMessageQueue(builder =>
        {
            builder
                .AddService("billing", accountStorageSettings.AccountTransactionPublisher, new BillingMessageQueueTopology())
                .AddMessageType<EventEnvelope<TransactionIngested>>("billing");
        });

        Then inject IMessageQueuePublisher anywhere in the app.

        Notes:
        - The CLR contract type is used to select the owning MQ service.
        - The IMessageQueueTopology for that service resolves DestinationName and RouteKey.
        - Duplicate type registrations throw during startup.