using Libra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Libra配置
    /// </summary>
    public class LibraBuilder
    {
        private readonly IConfiguration _configuration;

        public LibraBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// 添加路由过滤器
        /// </summary>
        /// <param name="filterFunc">拦截方法</param>
        /// <returns></returns>
        public LibraBuilder ConfigureFilter(string domain, Func<string, string, HttpRequest,HttpResponse, ValueTask<bool>> filterFunc)
        {
            LibraDomainManagement.ConfigureFilter(domain, filterFunc);
            return this;
        }
        /// <summary>
        /// 配置默认域的路由过滤器
        /// </summary>
        /// <param name="filterFunc"></param>
        /// <returns></returns>
        public LibraBuilder ConfigureFilter(Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> filterFunc)
        {
            LibraDomainManagement.ConfigureFilter(LibraDefined.DEFAULT_DOMAIN, filterFunc);
            return this;
        }
        /// <summary>
        /// 配置 RPC 方法
        /// </summary>
        /// <param name="domain">调用域</param>
        /// <param name="optAction"></param>
        /// <returns></returns>
        public LibraBuilder ConfigureWrpcSource(string domain,Func<LibraOption, LibraOption> optAction)
        {
            optAction?.Invoke(new LibraOption(domain));
            return this;
        }
        /// <summary>
        /// 配置默认调用域的 RPC 方法
        /// </summary>
        /// <param name="optAction"></param>
        /// <returns></returns>
        public LibraBuilder ConfigureWrpcSource(Func<LibraOption, LibraOption> optAction)
        {
            optAction?.Invoke(new LibraOption(LibraDefined.DEFAULT_DOMAIN));
            return this;
        }
    }
}
