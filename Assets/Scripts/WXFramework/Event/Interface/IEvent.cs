using System;

namespace WXFramework.Event
{
    /// <summary>
    /// 事件标志
    /// </summary>
    public interface IEvent
    {
        public Type Type { get; }
    }
}