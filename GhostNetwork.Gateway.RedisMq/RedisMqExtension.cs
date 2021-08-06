
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GhostNetwork.Gateway.RedisMq.Events;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace GhostNetwork.Gateway.RedisMq.Extensions
{
    public static class RedisMqExtension
    {
        public static IServiceCollection AddEventSender(this IServiceCollection services, ConfigurationOptions redisConfiguration)
        {
            IDatabase redisDb = null;

            try
            {
                redisDb = ConnectionMultiplexer.Connect(redisConfiguration).GetDatabase();
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            services.AddSingleton<IEventSender>(new EventSender(redisDb));
            return services;
        }

        public static IServiceCollection AddEventSender(this IServiceCollection services, string connectionString)
        {
            IDatabase redisDb = null;

            try
            {
                redisDb = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            services.AddSingleton<IEventSender>(new EventSender(redisDb));
            return services;
        }

        public static IServiceCollection AddHostedWorkerService(this IServiceCollection services, ConfigurationOptions redisConfiguration)
        {
            services.AddHostedService(provider => new RedisHandlerHostedService(provider, redisConfiguration));
            return services;
        }

        public static IServiceCollection AddHostedWorkerService(this IServiceCollection services, string connectionString)
        {
            services.AddHostedService(provider => new RedisHandlerHostedService(provider, connectionString));
            return services;
        }
    }

    internal static class InitializeHelper
    {
        public static IEnumerable<Type> GetEventsType(this IServiceProvider serviceProvider)
        {
            var inheritingTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEventHandler).IsAssignableFrom(t));

            var typeOfHandlers = inheritingTypes.Where(types => 
                types.GetTypeInfo().ImplementedInterfaces
                    .Any(ii => ii.IsGenericType)
            );

            if (!typeOfHandlers.Any())
                throw new ArgumentException("Input type is not declared as inheritor of IEventHandler<Event>");

            var typeOfEvents = new List<Type>();
            
            foreach (var type in typeOfHandlers)
            {
                var handler = serviceProvider.GetService(type) as IEventHandler;
                    
                if (handler == null)
                {
                    continue;
                }

                typeOfEvents.Add(type.GetTypeInfo().ImplementedInterfaces.First().GetTypeInfo().GenericTypeArguments[0]);
            }

            return typeOfEvents.Distinct();
        }

        public static IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(this IServiceProvider serviceProvider) where TEvent : EventBase, new()
        {
            var inheritingTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEventHandler<TEvent>).IsAssignableFrom(t));

            var typeOfHandlers = inheritingTypes.Where(types => 
                types.GetTypeInfo().ImplementedInterfaces
                    .Any(ii => ii.IsGenericType && ii.GetTypeInfo().GenericTypeArguments.Any(arg => arg.FullName == typeof(TEvent).FullName))
            );

            if (!typeOfHandlers.Any())
                throw new ArgumentException("Input type is not declared as inheritor of IEventHandler<Event>");

            var handlers = new List<IEventHandler<TEvent>>();
            
            foreach (var type in typeOfHandlers)
            {
               var handler = serviceProvider.GetService(type) as IEventHandler<TEvent>;
                    
                if (handler == null)
                {
                    continue;
                }

                handlers.Add(handler);
            }

            return handlers;
        }

        public static IEnumerable<IEventHandler> GetHandlers(this IServiceProvider serviceProvider, Type type)
        {
            var inheritingTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEventHandler).IsAssignableFrom(t));

            var typeOfHandlers = inheritingTypes.Where(types => 
                types.GetTypeInfo().ImplementedInterfaces
                    .Any(ii => ii.IsGenericType && ii.GetTypeInfo().GenericTypeArguments.Any(arg => arg.FullName == type.FullName))
            );

            if (!typeOfHandlers.Any())
                throw new ArgumentException("Input type is not declared as inheritor of IEventHandler");

            var handlers = new List<IEventHandler>();
            
            foreach (var t in typeOfHandlers)
            {
               var handler = serviceProvider.GetService(t) as IEventHandler;
                    
                if (handler == null)
                {
                    continue;
                }

                handlers.Add(handler);
            }

            return handlers;
        }
    }
}