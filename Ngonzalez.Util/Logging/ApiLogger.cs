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
            try
            {

                var apilog = new ApiLog();
                apilog.ApiKey = _apiKey;
                apilog.System = _system;
                apilog.LogType = new SystemLog()
                {
                    IpAddress = ipAddress,
                    Culture = culture,
                    Url = url,
                    Method = method,
                    Thread = thread,
                    LogName = _loggerName,
                    LogLevel = logLev,
                    Message = message
                };
                _helper.UrlHost(_urlHost).UrlApi(_urlApi).HttpMethod(RestMethod.Post).RequestBody(apilog).Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending log. ex {ex}");
            }
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

        internal class ApiLog
        {
            public string ApiKey { get; set; }
            public string System { get; set; }
            public SystemLog LogType { get; set; }
        }

        internal class SystemLog
        {
            public string IpAddress { get; set; }
            public string Culture { get; set; }
            public string Url { get; set; }
            public string Method { get; set; }
            public string Thread { get; set; }
            public string LogName { get; set; }
            public string LogLevel { get; set; }
            public DateTime Date { get; set; }
            public string Message { get; set; }
        }
    }
}