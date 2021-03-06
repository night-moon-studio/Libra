using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Libra 类型管理
/// </summary>
public class LibraTypeManagement
{

    private readonly ConcurrentDictionary<string, string> _KeyCallerMapper;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _typeMethodCache;
    public LibraTypeManagement()
    {
        _KeyCallerMapper = new ConcurrentDictionary<string, string>();
        _typeMethodCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    }


    /// <summary>
    /// 增加映射
    /// </summary>
    /// <param name="key">对外的Key</param>
    /// <param name="mapperName">映射的"类名.方法名"</param>
    public void AddMapper(string key, string mapperName)
    {
        _KeyCallerMapper[key] = mapperName;
    }


    /// <summary>
    /// 增加映射类型,如果该类实现了 T 接口,则该类下所有的方法都可以被调用,
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    /// <param name="types"></param>
    public void AddType<T>(params Type[] types)
    {

        if (types == null)
        {
            return;
        }
        var results = types.Where(item => item.IsImplementFrom<T>());
        AddType(results);

    }


    /// <summary>
    /// 批量添加允许被调用的类
    /// </summary>
    /// <param name="types"></param>
    public void AddType(IEnumerable<Type> types)
    {

        if (types == null)
        {
            return;
        }
        foreach (var item in types)
        {
            var itemName = item.GetRuntimeName();
            if (!_typeMethodCache.ContainsKey(itemName))
            {
                _typeMethodCache[itemName] = new ConcurrentDictionary<string, bool>();
            }
            var methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                AddFlag(itemName, method);
            }
        }

    }


    /// <summary>
    /// 添加类型到类名的映射, 方便在构造时进行检查
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodInfo"></param>
    private void AddFlag(string type, MethodInfo methodInfo)
    {

        _typeMethodCache[type][methodInfo.Name] = true;

    }


    /// <summary>
    /// 获取映射名,如果不存在映射则返回原值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetTypeFromMapper(string key)
    {

        if (_KeyCallerMapper.TryGetValue(key, out var value))
        {
            return value;
        }
        return key;

    }


    /// <summary>
    /// 检查是否存在这个方法允许被调用
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public bool HasMethod(string type, string methodName)
    {

        if (_typeMethodCache.ContainsKey(type))
        {
            return _typeMethodCache[type].ContainsKey(methodName);
        }
        return false;

    }

}

