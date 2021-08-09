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
        private static IDatabase database;
        private static string _connectionString;
        private static ConfigurationOptions _redisConfiguration;

        public static IServiceCollection AddEventSender(this IServiceCollection services, ConfigurationOptions redisConfiguration)
        {
            if (_redisConfiguration == null)
            {
                _redisConfiguration = redisConfiguration;
            }

            try
            {
                if (database == null)
                {
                    database = ConnectionMultiplexer.Connect(redisConfiguration).GetDatabase();
                }
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            services.AddSingleton<IEventSender>(new EventSender(database));
            return services;
        }

        public static IServiceCollection AddEventSender(this IServiceCollection services, string connectionString)
        {
            if (_connectionString == null)
            {
                _connectionString = connectionString;
            }

            try
            {
                if (database == null)
                {
                    database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
                }
            }
            catch (RedisConnectionException)
            {
                throw new ApplicationException("Redis server is unavailable");
            }

            services.AddSingleton<IEventSender>(new EventSender(database));
            return services;
        }

        public static IServiceCollection AddHostedWorkerService(this IServiceCollection services, 
            ConfigurationOptions redisConfiguration, 
            params Func<IServiceProvider, IEventWorker>[] args)
        {
            if (args == null && !args.Any())
            {
                throw new ApplicationException("Don't have reserved workers!");
            }
                        
            services.AddHostedService(provider => new RedisHandlerHostedService(provider, redisConfiguration, args));
            return services;
        }

        public static IServiceCollection AddHostedWorkerService(this IServiceCollection services, 
            string connectionString, 
            params Func<IServiceProvider, IEventWorker>[] args)
        {
            if (args == null && !args.Any())
            {
                throw new ApplicationException("Don't have reserved workers!");
            }
            
            services.AddHostedService(provider => new RedisHandlerHostedService(provider, connectionString, args));
            return services;
        }

        public static IEventWorker<TEvent> Subscribe<TEvent>(this IServiceProvider serviceProvider) where TEvent : EventBase, new()
        {
            if (database == null)
            {
                database = _connectionString != null ?
                        ConnectionMultiplexer.Connect(_connectionString).GetDatabase() :
                    _redisConfiguration != null ?
                        ConnectionMultiplexer.Connect(_redisConfiguration).GetDatabase() : 
                        throw new ApplicationException("Connection config is unavailable");
            }

            return new EventWorker<TEvent>(database, serviceProvider);
        }
    }

    internal static class InitializeHelper
    {
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
    }
}