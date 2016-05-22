using System;
using System.Globalization;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ngonzalez.Util.Logging
{
    public class ApiLogger : ILogger
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _loggerName;
        private readonly string _urlHost;
        private readonly string _urlApi;
        private readonly IRestHelper _helper;
        private readonly string _apiKey;
        private readonly string _system;
        private readonly LogLevel _lvl;



        public ApiLogger(LogLevel lvl, string loggerName, IHttpContextAccessor httpHttpContext, IRestHelper helper, string urlHost, string urlApi, string apiKey, string system)
        {
            _loggerName = loggerName;
            _lvl = lvl;
            _urlHost = urlHost;
            _urlApi = urlApi;
            _httpContext = httpHttpContext;
            _helper = helper;
            _apiKey = apiKey;
            _system = system;

        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            string message = formatter(state, exception);

            var ipAddress = GetIpAddress();
            var culture = CultureInfo.CurrentCulture.Name;
            var url = GetRequestUrl();
            var method = GetMethod();
            var thread = Thread.CurrentThread.ManagedThreadId.ToString();
            var logLev = logLevel.ToString();

            _helper.UrlHost(_urlHost).UrlApi(_urlApi).HttpMethod(RestMethod.Post).RequestBody(new
            {
                apiKey = _apiKey,
                system = _system,
                ipAddress = ipAddress,
                culture = culture,
                url = url,
                method = method,
                thread = thread,
                logName = _loggerName,
                logLevel = logLev,
                message = message
            }
           ).Execute();

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _lvl;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        private string GetIpAddress()
        {
            var ip = _httpContext?.HttpContext?.Connection;
            return ip != null ? ip.RemoteIpAddress?.ToString() : "No Aviavable";

        }

        private string GetMethod()
        {
            var request = _httpContext?.HttpContext?.Request;
            return request != null ? request.Method : "No Aviavable"; ;
        }

        private string GetRequestUrl()
        {
            var request = _httpContext?.HttpContext?.Request;
            return request != null ? $"{request.Host.Value}/{request.Path}" : "No Aviavable"; ;
        }


        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}