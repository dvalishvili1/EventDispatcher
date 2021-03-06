﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventDispatcher.Core
{
    public class EventDispatcher<T> : IEventDispatcher<IDomainEvent>
    {
        private readonly IDictionary<Type, List<Type>> eventhandlerMaps =
            new Dictionary<Type, List<Type>>();
        public EventDispatcher() { }
        public void RegisterHandlers(Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();

            var domainEvents = assemblyTypes
                .Where(at => typeof(IDomainEvent).IsAssignableFrom(at)
                 && at.IsClass && !at.IsAbstract && !at.IsInterface);

            foreach (var domainEvent in domainEvents)
            {
                eventhandlerMaps[domainEvent] = new List<Type>();
                foreach (var assemblyType in assemblyTypes)
                {
                    var eventHandlers = assemblyType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>));
                    if (eventHandlers != null)
                    {
                        var genericarguments = eventHandlers.GetGenericArguments().FirstOrDefault(x => domainEvent == x);
                        if (genericarguments != null)
                        {
                            eventhandlerMaps[domainEvent].Add(assemblyType);
                        }
                    }
                }
            }
        }
        public void Register<TEvent, THandler>()
        {
            var type = typeof(TEvent);
            if (!eventhandlerMaps.ContainsKey(type))
                eventhandlerMaps[type] = new List<Type>();
            eventhandlerMaps[type].Add(typeof(THandler));

        }
        public void Dispatch(dynamic evnt)
        {
            var type = evnt.GetType();
            if (!eventhandlerMaps.ContainsKey(type))
                return;
            var @eventHandlers = eventhandlerMaps[type];
            foreach (var handlr in @eventHandlers)
            {
                var handlerInstance = Activator.CreateInstance(handlr);
                var ihandler = handlerInstance as dynamic;
                ihandler.Handle(evnt);
            }

        }
        public void Dispatch(List<IDomainEvent> events)
        {
            foreach (var item in events)
            {
                this.Dispatch(item);
            }
        }
    }
}
