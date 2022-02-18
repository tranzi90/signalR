using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebSocketProtocol.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddMyWebSocketProtocol(this IServiceCollection services)
        {
            services.AddSingleton<MyConnectionManager>();

            return services;
        }
    }
}
