using System;
using UnityEngine;
using WXFramwork.Core;

namespace WXFramwork
{
    public class Init : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
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