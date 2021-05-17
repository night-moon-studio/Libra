using System;

namespace Libra.Model
{

    public struct LibraResult<S> 
    {
#if NET5_0_OR_GREATER
        public S Value;
#else
        public S Value { get;set; }
#endif
    }


    /*
    /// <summary>
    /// 简单参数的返回类型
    /// </summary>
    /// <typeparam name="S">基元类型或者值类型</typeparam>
    public class LibraResult<S>
    {
        public LibraResult(S value)
        {
#if NET5_0_OR_GREATER
            Value = value;
#else
            _value = value;
#endif
        }

#if NET5_0_OR_GREATER
        public readonly S Value;
#else
        private readonly S _value;
        public S Value { get { return _value; } }
#endif

    }
    */
}

