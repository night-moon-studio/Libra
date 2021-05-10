using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Libra.Client.Extension
{
    public class TaskExtension
    {
        internal static class ConfigureAwaitExtensions
        {
            internal static ConfiguredTaskAwaitable operator await (Task t)
            {
                return t.ConfigureAwait(false);
            }

            internal static ConfiguredTaskAwaitable<T> operator await<T>(Task<T> t) => t.ConfigureAwait(false);
            internal static ConfiguredValueTaskAwaitable operator await   (ValueTask vt) => vt.ConfigureAwait(false);
            internal static ConfiguredValueTaskAwaitable<T> operator await<T>(ValueTask<T> vt) => vt.ConfigureAwait(false);
        }
    }
}
