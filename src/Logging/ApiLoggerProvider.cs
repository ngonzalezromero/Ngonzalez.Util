using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;

namespace Ngonzalez.Util.Logging
{
    public class ApiLoggerProvider : ILoggerProvider
    {
        private readonly IRestHelper _helper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly LogLevel _minimumLevel;
        private readonly string _host;
        private readonly string _api;
        private readonly string _apiKey;
        private readonly string _system;

        public ApiLoggerProvider(LogLevel minimumLevel, IHttpContextAccessor httpContext, IRestHelper helper, string host, string api, string apiKey, string system)
        {
            _helper = helper;
            _httpContext = httpContext;
            _minimumLevel = minimumLevel;
            _host = host;
            _api = api;
            _apiKey = apiKey;
            _system = system;
        }

        public ILogger CreateLogger(string name)
        {
            return new ApiLogger(name, _minimumLevel, _httpContext, _helper,_host,_api,_apiKey,_system);
        }

        public void Dispose()
        {
        }

    }
}
