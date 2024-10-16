using System;
using System.Collections.Generic;

namespace WXFramework.Pool
{
    public class ObjectPool<T> : ObjectPoolBase
        where T : ObjectBase, new()
    {
        private Queue<Object<T>> _objects;

        public override Type ObjectType => typeof(T);

        public override int Count => _objects.Count;
        
        public ObjectPool(string poolName) : base(poolName)
        {
            _objects = new Queue<Object<T>>();
        }

        public T Get()
        {
            if (Count > 0)
            {
                Object<T> obj = _objects.Dequeue();
                obj.IsInPool = false;
                obj.ObjBase.OnInit();
                return obj.ObjBase;
            }
            
            Object<T> internalObject = ReferenceManager.Instance.Get<Object<T>>();
            internalObject.ObjBase = new T();
            internalObject.IsInPool = false;
            internalObject.ObjBase.CreateObj();
            
            return internalObject.ObjBase;
        }

        public bool Return(T obj)
        {
            return true;
        }
        
        public override void Release()
        {
            foreach (var obj in _objects)
            {
                obj.Clear();
            }
            
            _objects.Clear();
            _objects = null;
        }
    }
}