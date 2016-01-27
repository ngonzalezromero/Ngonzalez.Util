using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;

namespace Ngonzalez.Util.Logging
{
    public static class ApiLoggerFactoryExtensions
    {
        public static ILoggerFactory AddApiLogger(this ILoggerFactory factory, IHttpContextAccessor httpContext, IRestHelper helper, string host, string api, string apiKey, string system)
        {
            factory.AddProvider(new ApiLoggerProvider(factory.MinimumLevel, httpContext, helper, host, api, apiKey, system));
            return factory;
        }

    }
}
