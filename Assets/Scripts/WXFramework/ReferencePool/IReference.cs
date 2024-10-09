namespace WXFramework.Pool
{
    public interface IReference
    {
        bool IsInPool { get; set; }
        
        void Clear();
    }
}