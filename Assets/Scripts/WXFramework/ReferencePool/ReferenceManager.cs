using System;
using System.Collections.Concurrent;
using WXFramework.Core;

namespace WXFramework.Pool
{
    public class ReferenceManager : Singleton<ReferenceManager>
    {
        private ConcurrentDictionary<Type, ReferencePool> _referencePools;

        protected override void Awake()
        {
            lock (this)
            {
                _referencePools = new ConcurrentDictionary<Type, ReferencePool>();
            }
        }

        public override void Update()
        {
            
        }

        public T Get<T>() where T : class, IReference
        {
            ReferencePool pool = GetPool<T>();
            IReference reference = pool.Get();
            reference.IsInPool = false;
            return reference as T;
        }

        public void Return<T>(T reference)
            where T : IReference
        {
            //已进入池中，无需再次进池
            if (reference.IsInPool)
            {
                return;
            }

            reference.IsInPool = true;
            ReferencePool pool = GetPool<T>();
            pool.Return(reference);
        }

        private ReferencePool GetPool<T>() where T : IReference
        {
            Type type = typeof(T);
            return _referencePools.GetOrAdd(type, new ReferencePool(type));
        }
    }
}