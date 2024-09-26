namespace WXFramework.ReferencePool
{
    public interface IReference
    {
        bool IsFromPool { get; set; }
        
        void Clear();
    }
}