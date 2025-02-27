using WXFramework.Core;

namespace WXFramwork.Resource
{
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        private LoadResourceConfig _config;
        
        protected override void Awake()
        {
#if UNITY_EDITOR
            _config = new LoadResourceConfig(ResourceLoadMode.Develop);
#else
            _config = new LoadResourceConfig(ResourceLoadMode.Build);
#endif
        }

        public override void Update()
        {
        }
    }
}