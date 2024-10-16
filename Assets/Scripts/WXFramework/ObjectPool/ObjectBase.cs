using UnityEngine;

namespace WXFramework.Pool
{
    public abstract class ObjectBase
    {
        /// <summary>
        /// 对象
        /// </summary>
        private object _target;

        public object Target
        {
            get => _target;
        }
        
        public void CreateObj()
        {
            _target = CreateObject();

            //创建时，进行初始化
            OnInit();
        }

        public abstract object CreateObject();
        
        public virtual void OnInit() { }
        
        public virtual void OnClear() { }
    }
}