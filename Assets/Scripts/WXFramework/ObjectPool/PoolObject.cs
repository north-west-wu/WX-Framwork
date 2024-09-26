using UnityEngine;

namespace WXFramework.Pool
{
    public class PoolObject<T>
        where T : Object
    {
        public T poolObject;
        public string poolObjectName;
        public bool isUse;
    }
}