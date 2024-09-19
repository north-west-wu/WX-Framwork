using System;
using System.Collections.Generic;
using UnityEngine;
using WXFramwork.Core;

namespace WXFramwork.Event
{
    public class EventManager : Singleton<EventManager>
    {
        private readonly Dictionary<Type, List<IEvent>> _allEvents = new();
        
        protected override void Awake()
        {
            CodeTypes codeTypes = CodeTypes.Instance;
            foreach (Type type in codeTypes.GetTypes(typeof (EventAttribute)))
            {
                IEvent obj = Activator.CreateInstance(type) as IEvent;
                if (obj == null)
                {
                    throw new Exception($"type not is AEvent: {type.Name}");
                }
                
                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);
                foreach (object attr in attrs)
                {
                    EventAttribute eventAttribute = attr as EventAttribute;

                    if (eventAttribute == null)
                    {
                        throw new Exception($"当前类型 {type.Name} 有除了 Event 特性以外的其他特性，需要删除");
                    }
                
                    Type eventType = obj.Type;
                    
                    if (!_allEvents.ContainsKey(eventType))
                    {
                        _allEvents.Add(eventType, new List<IEvent>());
                    }
                    _allEvents[eventType].Add(obj);
                }
            }

            //Debug.Log($"事件数量 ：{_allEvents.Count}");
        }

        public override void Update()
        {
        }
        
        /// <summary>
        /// 发送事件
        /// </summary>
        public void Publish<TS>(TS args) where TS: struct
        {
            List<IEvent> iEvents;
            if (!_allEvents.TryGetValue(typeof (TS), out iEvents))
            {
                return;
            }
            
            foreach (IEvent iEvent in iEvents)
            {
                if (!(iEvent is AEvent<TS> aEvent))
                {
                    Debug.LogError($"event error: {iEvent.GetType().FullName}");
                    continue;
                }
                
                aEvent.Run(args);
            }
        }
    }
}