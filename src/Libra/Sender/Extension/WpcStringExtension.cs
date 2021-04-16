public static class WpcStringExtension
{

    public static LibraProtocalWrapper<T> WpcParam<T>(this string caller, T obj)
    {
        return new LibraProtocalWrapper<T>(caller, obj);
    }

    public static LibraProtocalWrapper NoWpcParam(this string caller)
    {
        return new LibraProtocalWrapper(caller);
    }

}
