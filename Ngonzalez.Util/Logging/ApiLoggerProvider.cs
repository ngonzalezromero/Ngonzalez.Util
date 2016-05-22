using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ngonzalez.Util.Logging
{
    public class ApiLoggerProvider : ILoggerProvider
    {
        private readonly IRestHelper _helper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _host;
        private readonly string _api;
        private readonly string _apiKey;
        private readonly string _system;
        private LogLevel _logLevel;


        public ApiLoggerProvider(LogLevel logLevel, IHttpContextAccessor httpContext, IRestHelper helper, string host, string api, string apiKey, string system)
        {
            _helper = helper;
            _httpContext = httpContext;
            _host = host;
            _api = api;
            _apiKey = apiKey;
            _system = system;
            _logLevel = logLevel;

        }

        public ILogger CreateLogger(string name)
        {
            return new ApiLogger(_logLevel, name, _httpContext, _helper, _host, _api, _apiKey, _system);
        }

        public void Dispose()
        {
        }

    }
}