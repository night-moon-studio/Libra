using Libra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class LibraMiddleware
    {
        public static async void UseLibraServer(this IApplicationBuilder app)
        {
            app.Use(async (context,next) => {
                
                var request = context.Request;
                if (request.Headers.TryGetValue("Libra", out var route))
                {

                    await LibraCaller.ExecuteAsync(route, request, context.Response).ConfigureAwait(false);
                    
                }
                else
                {
                    await next();
                }
                
            });
        }
    }
}
