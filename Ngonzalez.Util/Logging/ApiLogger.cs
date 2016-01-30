using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.AspNet.Http;
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

        public ApiLogger(string loggerName, LogLevel minimumLevel, IHttpContextAccessor httpHttpContext, IRestHelper helper, string urlHost, string urlApi, string apiKey, string system)
        {
            _loggerName = loggerName;
            _urlHost = urlHost;
            _urlApi = urlApi;
            _minimumLevel = minimumLevel;
            _httpContext = httpHttpContext;
            _helper = helper;
            _apiKey = apiKey;
            _system = system;

        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            string message;
            var values = state as ILogValues;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else if (values != null)
            {
                var builder = new StringBuilder();
                FormatLogValues(builder, values, level: 1, bullet: false);
                message = builder.ToString();
                if (exception != null)
                {
                    message += Environment.NewLine + exception;
                }
            }
            else
            {
                message = LogFormatter.Formatter(state, exception);
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
            return (logLevel >= _minimumLevel);
        }

        public IDisposable BeginScopeImpl(object state)
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

        private static void FormatLogValues(StringBuilder builder, ILogValues logValues, int level, bool bullet)
        {
            var values = logValues.GetValues();
            if (values == null)
            {
                return;
            }
            var isFirst = true;
            foreach (var kvp in values)
            {
                builder.AppendLine();
                if (bullet && isFirst)
                {
                    builder.Append(' ', level * Indentation - 1).Append('-');
                }
                else
                {
                    builder.Append(' ', level * Indentation);
                }
                builder.Append(kvp.Key).Append(": ");

                if (kvp.Value is IEnumerable && !(kvp.Value is string))
                {
                    foreach (var value in (IEnumerable)kvp.Value)
                    {
                        if (value is ILogValues)
                        {
                            FormatLogValues(builder, (ILogValues)value, level + 1, bullet: true);
                        }
                        else
                        {
                            builder.AppendLine()
                                   .Append(' ', (level + 1) * Indentation)
                                   .Append(value);
                        }
                    }
                }
                else if (kvp.Value is ILogValues)
                {
                    FormatLogValues(builder, (ILogValues)kvp.Value, level + 1, bullet: false);
                }
                else
                {
                    builder.Append(kvp.Value);
                }
                isFirst = false;
            }
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }


}
