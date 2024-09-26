using System;

namespace WXFramework.Event
{
    public abstract class AEvent<TS> : IEvent
        where TS : struct
    {
        public Type Type => typeof (TS);

        /// <summary>
        /// 执行事件
        /// </summary>
        public abstract void Run(TS args);
    }
}