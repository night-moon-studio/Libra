using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebServiceProvider.Service;
using WebServiceProvider.Services;

namespace WebServiceProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLibraWpc()
                //.ConfigureFilter((route, req, rsp) => 
                //{
                //    var heads = req.Headers;
                //    foreach (var item in heads)
                //    {
                //        Console.WriteLine($"{item.Key}:{item.Value}");
                //    }
                //    return true;
                //})
                .ConfigureJson(json => { json.PropertyNameCaseInsensitive = true; })
                .ConfigureLibra(opt => opt
                    .AllowAssembly(Assembly.GetEntryAssembly()) //允许该程序集内所有的类型被远程调用
                    .CallerMapper("Hello7", "TeacherService.Hello6") //当远程传来 Hello7 时默认路由到 TeacherService.Hello6

                );
            services.AddScoped<IStudent, Student>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseLibraService();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}