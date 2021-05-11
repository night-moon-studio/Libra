using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Libra.Client.Multicast
{
    public struct LibraMulticastTask<T>
    {
        private LibraMulticastTask(int taskCount) 
        {
            _resultCount = 0;
            _onceShut = 0;
            _taskCount = taskCount;
            _taskNotify = new TaskCompletionSource<T>();
        }

        public static LibraMulticastTask<T> Create(int taskCount)
        {
            return new LibraMulticastTask<T>(taskCount);
        }

        private int _resultCount;
        private int _onceShut;
        private readonly int _taskCount;
        private readonly TaskCompletionSource<T> _taskNotify;

        public bool IsCompleted
        {
            get { return _taskNotify.Task.IsCompleted; }
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <returns></returns>
        public ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter GetAwaiter()
        {
            return _taskNotify.Task.ConfigureAwait(false).GetAwaiter();
        }


        /// <summary>
        /// 设置一次性结果
        /// </summary>
        /// <param name="value"></param>
        public void SetOnceResult(T value)
        {
            if (Interlocked.CompareExchange(ref _onceShut, 1, 0) == 0)
            {
                _taskNotify.SetResult(value);
            }
        }

        /// <summary>
        /// 设置结果,当填充次数 = taskCount 时返回结果
        /// </summary>
        /// <param name="value"></param>
        public void FillResult(T value)
        {
            Interlocked.Increment(ref _resultCount);
            if (Interlocked.CompareExchange(ref _resultCount, 1, _taskCount) == _taskCount)
            {
                _taskNotify.SetResult(value);
            }
        }

    }
}
