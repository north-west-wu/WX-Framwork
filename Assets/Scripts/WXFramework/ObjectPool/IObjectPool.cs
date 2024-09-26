namespace WXFramework.Pool
{
    public interface IObjectPool
    {
        string PoolName { get; }

        void Release();
    }
}