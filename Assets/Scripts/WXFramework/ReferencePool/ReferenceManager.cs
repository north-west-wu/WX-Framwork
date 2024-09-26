using System;
using System.Collections.Concurrent;
using WXFramework.Core;

namespace WXFramework.ReferencePool
{
    public class ReferenceManager : Singleton<ReferenceManager>
    {
        private ConcurrentDictionary<Type, ReferencePool> _referencePools = new();

        public override void Update()
        {
            
        }

        public IReference Get<T>() where T : IReference
        {
            ReferencePool pool = GetPool<T>();
            IReference reference = pool.Get();
            reference.IsFromPool = false;
            return reference;
        }

        public void Return<T>(T reference)
            where T : IReference
        {
            //已进入池中，无需再次进池
            if (reference.IsFromPool)
            {
                return;
            }

            reference.IsFromPool = true;
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