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
            //添加 UI 组
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/Low").gameObject, UIGroupId.Low);
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/Mid").gameObject, UIGroupId.Mid);
            UIManager.Instance.AddUIGroup(transform.Find("UIManager/High").gameObject, UIGroupId.High);
            
            //打开 UI
            UIManager.Instance.OpenUIWindow(UIGroupId.Mid, UIWindowId.Main);
            // UIManager.Instance.OpenUIWindow(UIGroupId.Mid, UIWindowId.Main2);
            // UIManager.Instance.RefocusUIWindow(UIGroupId.Mid, UIWindowId.Main);
            // UIManager.Instance.RefocusUIWindow(UIGroupId.Mid, UIWindowId.Main2);
            // UIManager.Instance.CloseUIWindow(UIGroupId.Mid, UIWindowId.Main2);
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