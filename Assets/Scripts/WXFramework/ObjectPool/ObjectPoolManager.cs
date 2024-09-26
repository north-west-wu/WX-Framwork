using System.Collections.Generic;
using UnityEngine;
using WXFramework.Core;

namespace WXFramework.Pool
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private Dictionary<string, IObjectPool> _poolDic = new();
        
        public override void Update()
        {
            
        }
        
        public ObjectPool<T> GetObjectPool<T>(string poolName, string path)
            where T : Object
        {
            if (_poolDic.ContainsKey(poolName))
            {
                return _poolDic[poolName] as ObjectPool<T>;
            }

            ObjectPool<T> objPool = new ObjectPool<T>(poolName, path);
            _poolDic.Add(poolName, objPool);
            return objPool;
        }
        
        public void DestroyPool(string poolName)
        {
            if (_poolDic.ContainsKey(poolName))
            {
                _poolDic[poolName].Release();
                _poolDic.Remove(poolName);
            }
        }

        public void DestroyPool(IObjectPool pool)
        {
            DestroyPool(pool.PoolName);
        }
    }
}