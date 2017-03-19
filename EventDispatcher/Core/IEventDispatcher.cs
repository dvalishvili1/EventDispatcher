using System.Collections.Generic;
using System.Reflection;

namespace EventDispatcher.Core
{
    public interface IEventDispatcher<TDomainEvent>
    {
        void RegisterHandlers(Assembly assembly);
        void Register<TEvent, THandler>();
        void Dispatch(dynamic evnt);
        void Dispatch(List<TDomainEvent> domainEvents);
    }
}
