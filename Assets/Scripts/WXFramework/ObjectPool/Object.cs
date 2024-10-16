namespace WXFramework.Pool
{
    public class Object<T> : IReference
        where T : ObjectBase, new()
    {
        private T _objectBase;
                
        /// <summary>
        /// 是否在池中
        /// </summary>
        private bool _inPool;

        public T ObjBase
        {
            get => _objectBase;
            set => _objectBase = value;
        }
        
        public bool IsInPool
        {
            get => _inPool;
            set => _inPool = value;
        }
        
        public void Clear()
        {
            _objectBase.OnClear();
            _objectBase = null;
        }
    }
}