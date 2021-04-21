using Libra.Extension.Utils;

public static class WpcStringExtension
{

    public static LibraParameterHandler WpcParam<T>(this string caller, T obj)
    {
        return new LibraParameterHandler<T>(caller, obj);
    }

    public static LibraParameterHandler NoWpcParam(this string caller)
    {
        return new LibraParameterHandler(caller);
    }

}
