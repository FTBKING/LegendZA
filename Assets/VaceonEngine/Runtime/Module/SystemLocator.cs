using System;
using System.Collections.Generic;

namespace VaceonEngine
{
    public class SystemLocator
    {
        //类型-对象映射字典
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();
        //泛型注册方法
        public static void Register<T>(T service) where T : class
        {
            //反射获取类对象
            var key = typeof(T);
            if (_services == null) throw new Exception("服务不能为空");
            //对应类型注册服务对象
            _services[key] = service;
        }
        //泛型删除方法
        public static bool Unregister<T>() where T : class
        {
            return _services.Remove(typeof(T));
        } 
        //泛型获取服务方法
        public static T Get<T>() where T : class
        {
            //如果字典里存在，就拿出来，别忘了转换
            if (_services.TryGetValue(typeof(T), out var service)) return service as T;
            //不存在直接抛异常
            throw new Exception($"未注册服务{typeof(T)},请先调用ServiceLocator.Register注册");
        }
        //判断是否注册了服务方法
        public static bool IsRegistered<T>() where T : class => _services.ContainsKey(typeof(T));
        //一键清空所有注册服务
        public static void Clear() => _services.Clear();
    }
}