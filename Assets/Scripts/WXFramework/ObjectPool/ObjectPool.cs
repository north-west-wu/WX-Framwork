using System.Collections.Generic;
using UnityEngine;

namespace WXFramework.Pool
{
    public class ObjectPool<T> : IObjectPool where T : Object
    {
        private List<PoolObject<T>> _poolObjects;
        private Object _obj;
        private string _poolName;
        private string _path;
        private int _count;
        
        /// <summary>
        /// 池名称
        /// </summary>
        public string PoolName
        {
            get => _poolName;
        }

        /// <summary>
        /// 池内对象数量
        /// </summary>
        public int Count
        {
            get => _count;
        }
        
        public ObjectPool(string poolName, string path)
        {
            _poolObjects = new List<PoolObject<T>>();
            _poolName = poolName;
            _path = path;
            _count = 0;
        }
        
        public T Get(Transform parentTransform = null)
        {
            PoolObject<T> po = null;
            foreach (var poolObject in _poolObjects)
            {
                if (!poolObject.isUse)
                {
                    po = poolObject;
                    break;
                }
            }

            if (po == null)
            {
                po = CreatePoolObject();
                _count++;
            }
            
            po.isUse = true;
            if (po.poolObject is GameObject go)
            {
                go.gameObject.SetActive(true);
                go.transform.SetParent(parentTransform, false);
            }
            
            return po.poolObject;
        }

        
        public void Recycle(T target)
        {
            foreach (var po in _poolObjects)
            {
                if (po.poolObject == target)
                {
                    po.isUse = false;
                    if (po.poolObject is GameObject go)
                    {
                        go.gameObject.SetActive(false);
                    }
                }
            }
        }

        private PoolObject<T> CreatePoolObject()
        {
            if (_obj == null)
                _obj = Resources.Load<T>(_path);
            PoolObject<T> po = new PoolObject<T>();
            po.poolObject = Object.Instantiate(_obj) as T;
            po.poolObjectName = _poolName;
            po.isUse = false;

            if (po.poolObject is GameObject go)
            {
                go.gameObject.SetActive(false);
            }
            
            _poolObjects.Add(po);
            return po;
        }
        
        public void Release()
        {
            foreach (var poolObject in _poolObjects)
            {
                poolObject.poolObjectName = "";
                poolObject.isUse = false;
                Object.Destroy(poolObject.poolObject);
            }
            _poolObjects.Clear();
            if (typeof(GameObject) != typeof(T)
                && !typeof(Component).IsSubclassOf(typeof(T)))
            {
                Resources.UnloadAsset(_obj);
            }
        }
    }
}