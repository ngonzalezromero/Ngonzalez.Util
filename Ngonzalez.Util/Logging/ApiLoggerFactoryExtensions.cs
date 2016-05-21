using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ngonzalez.Util.Logging
{
    public static class ApiLoggerFactoryExtensions
    {
        public static ILoggerFactory AddApiLogger(this ILoggerFactory factory, LogLevel minimumLogLevel, IHttpContextAccessor httpContext, IRestHelper helper, string host, string api, string apiKey, string system)
        {   
            
            Func<string, LogLevel, bool> logFilter = delegate (string loggerName, LogLevel logLevel)
            {
                if (logLevel < minimumLogLevel) { return false; }
               
                return true;
            };
            
            factory.AddProvider(new ApiLoggerProvider(logFilter, httpContext, helper, host, api, apiKey, system));
            return factory;
        }

    }
}