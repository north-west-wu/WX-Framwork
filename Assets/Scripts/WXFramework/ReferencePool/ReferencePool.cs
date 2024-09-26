using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace WXFramework.ReferencePool
{
    public class ReferencePool
    {
        //引用队列(线程安全)
        private readonly ConcurrentQueue<IReference> _references = new();
        //引用类型
        private readonly Type _referenceType;
        //引用个数
        private int _referenceCount;

        public ReferencePool(Type referenceType)
        {
            _referenceType = referenceType;
        }

        public IReference Get()
        {
            if (_referenceCount > 0)
            {
                if (_references.TryDequeue(out IReference item))
                {
                    Interlocked.Increment(ref _referenceCount);
                    return item;
                }
            }
            
            return Activator.CreateInstance(_referenceType) as IReference;;
        }

        public void Return(IReference reference)
        {
            reference.Clear();
            if (_references.Contains(reference))
            {
                Debug.LogError("回收引用出现重复回收的情况");
                return;
            }
            _references.Enqueue(reference);
            Interlocked.Decrement(ref _referenceCount);
        }
    }
}