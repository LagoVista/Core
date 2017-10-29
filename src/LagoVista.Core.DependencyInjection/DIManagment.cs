using Microsoft.Extensions.DependencyInjection;
using System;
using LagoVista.Core.Interfaces;

namespace LagoVista.Core.DependencyInjection
{
    public class DIManagement : LagoVista.Core.Interfaces.IServiceCollection
    {
        Microsoft.Extensions.DependencyInjection.IServiceCollection _serviceCollection;

        public DIManagement(Microsoft.Extensions.DependencyInjection.IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }
        
        public void AddTransient(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            _serviceCollection.AddTransient(serviceType, implementationFactory);
        }

        public void AddTransient(Type serviceType, Type implementationType)
        {
            _serviceCollection.AddTransient(serviceType, implementationType);
        }

        public void AddScoped<TService>() where TService : class
        {
            _serviceCollection.AddScoped<TService>();
        }


        public void AddScoped(Type serviceType, Type implementationType)
        {
            _serviceCollection.AddScoped(serviceType, implementationType);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in serviceType with a factory specified
        //     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddScoped(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
            _serviceCollection.AddScoped(serviceType, implementationFactory);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in TService with an implementation
        //     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
             _serviceCollection.AddScoped<TService, TImplementation>();
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in serviceType to the specified Microsoft.Extensions.DependencyInjection.v.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register and the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddScoped(Type serviceType)
        {
             _serviceCollection.AddScoped(serviceType);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in TService with a factory specified
        //     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
             _serviceCollection.AddScoped<TService>(implementationFactory);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddScoped<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
             _serviceCollection.AddScoped<TImplementation, TImplementation>(implementationFactory);

        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
             _serviceCollection.AddSingleton<TService, TImplementation>(implementationFactory);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with a factory specified
        //     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
             _serviceCollection.AddSingleton<TService>(implementationFactory);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton<TService>() where TService : class
        {
             _serviceCollection.AddSingleton<TService>();

        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType to the specified
        //     Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register and the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton(Type serviceType)
        {
             _serviceCollection.AddSingleton(serviceType);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an implementation
        //     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
             _serviceCollection.AddSingleton<TService, TImplementation>();

        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with a factory
        //     specified in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public void AddSingleton(Type serviceType, Func<IServiceProvider, object> implementationFactory)
        {
             _serviceCollection.AddSingleton(serviceType, implementationFactory);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with an implementation
        //     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationType:
        //     The implementation type of the service.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddSingleton(Type serviceType, Type implementationType)
        {
             _serviceCollection.AddSingleton(serviceType, implementationType);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an instance specified
        //     in implementationInstance to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationInstance:
        //     The instance of the service.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddSingleton<TService>(TService implementationInstance) where TService : class
        {
             _serviceCollection.AddSingleton<TService>(implementationInstance);
        }

        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with an instance
        //     specified in implementationInstance to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationInstance:
        //     The instance of the service.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddSingleton(Type serviceType, object implementationInstance)
        {
             _serviceCollection.AddSingleton(serviceType, implementationInstance);
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddTransient<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
             _serviceCollection.AddTransient<TService, TImplementation>(implementationFactory);
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in TService with a factory specified
        //     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
             _serviceCollection.AddTransient<TService>(implementationFactory);
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddTransient<TService>() where TService : class
        {
             _serviceCollection.AddTransient<TService>();
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in serviceType to the specified
        //     Microsoft.Extensions.DependencyInjection.void.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.void to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register and the implementation to use.
        //
        // s:
        //     A reference to this instance after the operation has completed.
        public  void AddTransient(Type serviceType)
        {
             _serviceCollection.AddTransient(serviceType);
        }

        public void AddTransient<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            _serviceCollection.AddTransient<TService, TImplementation>();
        }
    }
}
