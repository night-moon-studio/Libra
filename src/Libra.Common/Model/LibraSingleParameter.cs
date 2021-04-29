namespace Libra.Model
{
    /// <summary>
    /// 单个参数的包装
    /// </summary>
    /// <typeparam name="S">基元类型或者值类型</typeparam>
    public class LibraSingleParameter<S>
    {
        public S Value { get; set; }

    }
}
