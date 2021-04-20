using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public static class LibraTypeManagement
{
    private static readonly ConcurrentDictionary<string, string> _flagMapper;
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, bool>> _typeMethodCache;
    static LibraTypeManagement()
    {
        _flagMapper = new ConcurrentDictionary<string, string>();
        _typeMethodCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, bool>>();
    }

    public static void AddMapper(string flag, string mapperName)
    {
        _flagMapper[flag] = mapperName;
    }


    public static void AddType<T>(params Type[] types)
    {

        if (types == null)
        {
            return;
        }
        var results = types.Where(item => item.IsImplementFrom<T>());
        AddType(results);

    }


    public static void AddType(IEnumerable<Type> types)
    {

        if (types == null)
        {
            return;
        }
        foreach (var item in types)
        {
            var methods = item.GetMethods();
            foreach (var method in methods)
            {
                AddFlag(item, method);
            }
        }

    }


    private static void AddFlag(Type type, MethodInfo methodInfo)
    {

        if (!_typeMethodCache.ContainsKey(type))
        {
            _typeMethodCache[type] = new ConcurrentDictionary<string, bool>();
        }
        _typeMethodCache[type][methodInfo.Name] = true;

    }


    public static string GetTypeFromMapper(string key)
    {

        if (_flagMapper.TryGetValue(key, out var value))
        {
            return value;
        }
        return key;

    }


    public static bool HasMethod(Type type, string methodName)
    {

        if (_typeMethodCache.ContainsKey(type))
        {
            return _typeMethodCache[type].ContainsKey(methodName);
        }
        return false;

    }

}

