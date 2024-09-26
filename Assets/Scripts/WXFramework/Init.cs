using System;
using UnityEngine;
using WXFramework.Core;
using WXFramework.Event;
using WXFramework.Pool;
using WXFramework.ReferencePool;
using WXFramework.UI;

namespace WXFramwork
{
    public class Init : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            //代码类型
            GameFrameworkEntry.Instance.AddSingleton<CodeTypes>();
            //事件
            GameFrameworkEntry.Instance.AddSingleton<EventManager>();
            //引用池
            GameFrameworkEntry.Instance.AddSingleton<ReferenceManager>();
            //游戏对象池
            GameFrameworkEntry.Instance.AddSingleton<ObjectPoolManager>();
            //UI
            GameFrameworkEntry.Instance.AddSingleton<UIManager>();
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/Low").gameObject, UIGroupId.Low);
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/Mid").gameObject, UIGroupId.Mid);
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/High").gameObject, UIGroupId.High);
        }

        /// <summary>
        /// 所有模块的更新入口
        /// </summary>
        void Update()
        {
            GameFrameworkEntry.Instance.Update();
        }
    }
}