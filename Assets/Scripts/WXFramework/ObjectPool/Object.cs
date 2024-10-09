namespace WXFramework.Pool
{
    public class Object<T> : IReference
        where T : ObjectBase
    {
        private T _objectBase;

        public object Target
        {
            get => _objectBase.Target;
        }
        
        public bool IsInPool
        {
            get => _objectBase.IsInPool;
            set => _objectBase.IsInPool = value;
        }

        public static void Create(T obj, object target)
        {
            Object<T> internalObject = ReferenceManager.Instance.Get<Object<T>>();
            _objectBase.Init(target);
        }
        
        public void Clear()
        {
            _objectBase.Clear();
            _objectBase = null;
        }
    }
}