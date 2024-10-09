using UnityEngine;

namespace WXFramework.Pool
{
    public abstract class ObjectBase
    {
        /// <summary>
        /// 对象
        /// </summary>
        private object _target;
        
        /// <summary>
        /// 是否在池中
        /// </summary>
        private bool _inPool;

        public object Target
        {
            get => _target;
        }


        public bool IsInPool
        {
            get => _inPool;
            set => _inPool = value;
        }

        public void Init(object target)
        {
            Init(target.GetType().Name, target);
        }
        
        public void Init(string name, object target)
        {
            if (target == null)
            {
                Debug.LogError("当前初始化对象失败，target 为 null，" + name);
                return;
            }
            
            _target = target;
        }
        
        public virtual void Clear()
        {
            _target = null;
            _inPool = false;
        }
    }
}