using System;
using System.Collections.Generic;

namespace ColorFall.Game
{
    public static class EventManager
    {
        private static readonly Dictionary<Type, Action<GameEvent>> SEvents = new();
        private static readonly Dictionary<Delegate, Action<GameEvent>> SEventLookups = new();

        public static void AddListener<T>(Action<T> evt) where T : GameEvent
        {
            if (!SEventLookups.ContainsKey(evt))
            {
                void NewAction(GameEvent e) => evt((T)e);
                SEventLookups[evt] = NewAction;

                if (SEvents.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                    SEvents[typeof(T)] = internalAction += NewAction;
                else
                    SEvents[typeof(T)] = NewAction;
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
        {
            if (SEventLookups.TryGetValue(evt, out var action))
            {
                if (SEvents.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                        SEvents.Remove(typeof(T));
                    else
                        SEvents[typeof(T)] = tempAction;
                }

                SEventLookups.Remove(evt);
            }
        }

        public static void Broadcast(GameEvent evt)
        {
            if (SEvents.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }

        public static void Clear()
        {
            SEvents.Clear();
            SEventLookups.Clear();
        }
    }
}
