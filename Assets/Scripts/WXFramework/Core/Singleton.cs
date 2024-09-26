namespace WXFramework.Core
{
    /// <summary>
    /// 单例类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ASingleton
        where T: Singleton<T>
    {
        private static T _instance;
        private bool _isDisposed;
        
        public static T Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public override void Register()
        {
            Instance = (T)this;
            this.Awake();
        }
        
        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _isDisposed = true;

            this.Destroy();
            
            Instance = null;
        }
        
        protected virtual void Awake()
        {
        }
        
        protected virtual void Destroy()
        {
        }
    }
}