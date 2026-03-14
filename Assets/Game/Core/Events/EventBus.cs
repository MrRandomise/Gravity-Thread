using System;
using System.Collections.Generic;

namespace GravityThread.Core.Events
{
    /// <summary>
    /// Lightweight event bus. Decouple publishers from subscribers.
    /// </summary>
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();
            _handlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
                _handlers[type].Remove(handler);
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type)) return;
            var list = _handlers[type];
            for (int i = list.Count - 1; i >= 0; i--)
                ((Action<T>)list[i])?.Invoke(eventData);
        }
    }
}
