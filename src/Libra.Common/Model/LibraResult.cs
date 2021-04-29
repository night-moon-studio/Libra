namespace Libra.Model
{
    /// <summary>
    /// 简单参数的返回类型
    /// </summary>
    /// <typeparam name="S">基元类型或者值类型</typeparam>
    public class LibraResult<S>
    {
        public S Value { get; set; }
    }
}

