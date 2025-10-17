// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8671489ec1dc8e1058d45eaa9976891ff90ce6a731ea6df3a720b50d089ce2bf
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IServiceCollection
    {
        void AddScoped(Type serviceType);
        void AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory);
        void AddScoped(Type serviceType, Type implementationType);
        void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        void AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService;
        void AddScoped<TService>() where TService : class;
        void AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
        void AddSingleton(Type serviceType);
        void AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory);
        void AddSingleton(Type serviceType, object implementationInstance);
        void AddSingleton(Type serviceType, Type implementationType);
        void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        void AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService;
        void AddSingleton<TService>() where TService : class;
        void AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
        void AddSingleton<TService>(TService implementationInstance) where TService : class;
        void AddTransient(Type serviceType);
        void AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory);
        void AddTransient(Type serviceType, Type implementationType);
        void AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService;
        void AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        void AddTransient<TService>() where TService : class;
        void AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
    }
}
