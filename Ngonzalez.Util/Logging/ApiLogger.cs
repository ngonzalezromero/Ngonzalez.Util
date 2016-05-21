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
        private const int Indentation = 2;
        private readonly LogLevel _minimumLevel;
        private readonly string _loggerName;
        private readonly string _urlHost;
        private readonly string _urlApi;
        private readonly IRestHelper _helper;
        private readonly string _apiKey;
        private readonly string _system;
        private Func<string, LogLevel, bool> _filter;

        public Func<string, LogLevel, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _filter = value;
            }
        }


        public ApiLogger(string loggerName, Func<string, LogLevel, bool> filter, IHttpContextAccessor httpHttpContext, IRestHelper helper, string urlHost, string urlApi, string apiKey, string system)
        {
            _loggerName = loggerName;
            _urlHost = urlHost;
            _urlApi = urlApi;
            Filter = filter ?? ((category, logLevel) => true);
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
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (exception != null)
            {
                message += Environment.NewLine + exception;
            }
            else
            {
                message = exception.Message + " " + exception.StackTrace;
            }

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

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
            return Filter(_loggerName, logLevel);
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