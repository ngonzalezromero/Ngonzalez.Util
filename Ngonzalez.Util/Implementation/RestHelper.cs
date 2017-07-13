using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text;
using Flurl.Http;
using Flurl;

namespace Ngonzalez.Util
{
    public sealed class RestHelper : IRestHelper
    {
        private string _hostUrl;
        private object _json;
        private RestMethod _restMethod;
        private FlurlClient _httpClient;
        private Url url;
        private static JObject _jsobject = new JObject();

        public RestHelper(FlurlClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IRestHelper UrlHost(string hostUrl)
        {
            _hostUrl = hostUrl;
            return this;
        }

        public IRestHelper UrlApi(string apiUrl)
        {
            url = _hostUrl.AppendPathSegment(apiUrl);
            return this;
        }

        public IRestHelper HttpMethod(RestMethod method)
        {
            _restMethod = method;
            return this;
        }

        public IRestHelper RequestParameter(Dictionary<string, dynamic> parameter)
        {
            var builder = new StringBuilder(parameter.Count);
            foreach (var param in parameter)
            {
                builder.Append($"/{param.Value}");
            }

            url.AppendPathSegments(builder.ToString());
            return this;
        }

        public IRestHelper SetQueryParam(string key, dynamic data)
        {
            url.SetQueryParam(key, data);
            return this;
        }

        public IRestHelper RequestBody(dynamic json)
        {
            _json = json;
            return this;
        }


        public dynamic ExecuteSafe()
        {
            try
            {
                return ExecuteSync();
            }
            catch (Exception)
            {
                return _jsobject;
            }
        }

        public dynamic Execute()
        {
            return ExecuteSync();
        }

        public async Task<dynamic> ExecuteAsync()
        {
            if (_restMethod == RestMethod.Get)
            {
                return await url.WithClient(_httpClient).GetAsync().ReceiveJson<dynamic>().ConfigureAwait(false); ;
            }
            else
            {
                return await url.WithClient(_httpClient).PostJsonAsync(_json).ReceiveJson<dynamic>().ConfigureAwait(false); ;
            }
        }

        private dynamic ExecuteSync()
        {
            if (_restMethod == RestMethod.Get)
            {
                return url.WithClient(_httpClient).GetAsync().ReceiveJson<dynamic>().GetAwaiter().GetResult();
            }
            else
            {
                return url.WithClient(_httpClient).PostJsonAsync(_json).ReceiveJson<dynamic>().GetAwaiter().GetResult();
            }

        }
    }
}