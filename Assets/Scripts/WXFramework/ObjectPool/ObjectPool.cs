using System;

namespace WXFramework.Pool
{
    public class ObjectPool<T> : ObjectPoolBase
        where T : ObjectBase
    {
        public ObjectPool(string poolName) : base(poolName)
        {
        }

        public override Type ObjectType { get; }
        public override int Count { get; }
        public override void Release()
        {
            throw new NotImplementedException();
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}