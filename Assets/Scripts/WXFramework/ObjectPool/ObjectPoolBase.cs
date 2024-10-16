using System;

namespace WXFramework.Pool
{
    public abstract class ObjectPoolBase
    {
        private string _poolName;

        public string PoolName
        {
            get => _poolName;
        }
        
        /// <summary>
        /// 获取对象池对象类型。
        /// </summary>
        public abstract Type ObjectType
        {
            get;
        }
        
        /// <summary>
        /// 获取对象池中对象的数量。
        /// </summary>
        public abstract int Count
        {
            get;
        }

        public ObjectPoolBase(string poolName)
        {
            _poolName = poolName;
        }
        
        /// <summary>
        /// 释放对象池未使用的对象
        /// </summary>
        public abstract void Release();
    }
}