using Microsoft.AspNetCore.Builder;
using MyWebSocketProtocol.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebSocketProtocol.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMyWebSocketProtocol(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<MyWebSocketMiddleware>();

            return applicationBuilder;
        }
    }
}
