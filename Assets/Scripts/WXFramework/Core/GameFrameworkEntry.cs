using System;
using System.Collections.Generic;

namespace WXFramework.Core
{
    public class GameFrameworkEntry
    {
        private static GameFrameworkEntry _instance;
        
        public static GameFrameworkEntry Instance
        {
            get
            {
                return _instance ??= new GameFrameworkEntry();
            }
        }
        
        private readonly Dictionary<Type, ASingleton> _gameFrameworkModules = new Dictionary<Type, ASingleton>();

        public void Update()
        {
            //将各模块的单例进行更新
            foreach (ASingleton singleton in _gameFrameworkModules.Values)
            {
                singleton.Update();
            }
        }
        
        public T AddSingleton<T>() where T : ASingleton, new()
        {
            T singleton = new();
            Type type = singleton.GetType();
            //添加对应的单例类
            _gameFrameworkModules[type] = singleton;
            //注册
            singleton.Register();
            return singleton;
        }
    }
}