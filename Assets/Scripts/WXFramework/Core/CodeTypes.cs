using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WXFramework.Core
{
    public class CodeTypes : Singleton<CodeTypes>
    {
        private readonly Dictionary<string, Type> _allTypes = new();
        private readonly Dictionary<Type, List<Type>> _types = new();

        protected override void Awake()
        {
            //获得程序集
            Assembly[] assemblies = GetAllAssembly();
            Dictionary<string, Type> addTypes = GetAssemblyTypes(assemblies);
            foreach ((string fullName, Type type) in addTypes)
            {
                _allTypes[fullName] = type;
                
                if (type.IsAbstract)
                {
                    continue;
                }
                
                // 记录所有的有BaseAttribute标记的的类型
                object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                foreach (object o in objects)
                {
                    Type objType = o.GetType();
                    if (!_types.ContainsKey(objType))
                    {
                        _types.Add(objType, new List<Type>());
                    }
                    
                    _types[objType].Add(type);
                }
            }
        }

        public override void Update()
        {
        }
        
        /// <summary>
        /// 获得所有对应特性的对象类型
        /// </summary>
        /// <param name="systemAttributeType">特性类型</param>
        public List<Type> GetTypes(Type systemAttributeType)
        {
            if (!_types.ContainsKey(systemAttributeType))
            {
                return new List<Type>();
            }

            return _types[systemAttributeType];
        }
        
        /// <summary>
        /// 获得所有对象类型
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Type> GetTypes()
        {
            return _allTypes;
        }
        
        /// <summary>
        /// 获得对应的对象类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type GetType(string typeName)
        {
            return _allTypes[typeName];
        }
        
        /// <summary>
        /// 获得需要的所有程序集
        /// </summary>
        private static Assembly[] GetAllAssembly()
        {
            List<Assembly> list = new List<Assembly>();
#if UNITY_EDITOR
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                string name = assembly.GetName().Name;
                if (name == "Assembly-CSharp" || name == "Assembly-CSharp-Editor")
                {
                    list.Add(assembly);
                }
            }
            return list.ToArray();
#else 
            throw new Exception("程序集未配置, 需要配置对应的程序集");
#endif
        }
        
        /// <summary>
        /// 获得程序集类型
        /// </summary>
        public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] assemblies)
        {
            Dictionary<string, Type> types = new Dictionary<string, Type>();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    types[type.FullName] = type;
                }
            }

            return types;
        }
    }
}