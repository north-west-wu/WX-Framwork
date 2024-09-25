using System;
using UnityEngine;
using WXFramwork.Core;
using WXFramwork.Event;
using WXFramwork.UI;

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
            //UI
            GameFrameworkEntry.Instance.AddSingleton<UIManager>();
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