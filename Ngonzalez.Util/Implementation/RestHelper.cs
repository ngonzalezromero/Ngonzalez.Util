using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Ngonzalez.Util
{
    public sealed class RestHelper : IRestHelper
    {
        private string _apiUrl;
        private string _hostUrl;
        private dynamic _json;
        private RestMethod _restMethod;
        private HttpClient _httpClient;
        private Dictionary<string, string> _parameter;


        public IRestHelper UrlHost(string hostUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _hostUrl = hostUrl;
            return this;
        }

        public IRestHelper UrlApi(string apiUrl)
        {
            _apiUrl = apiUrl;
            return this;
        }

        public IRestHelper HttpMethod(RestMethod method)
        {
            _restMethod = method;
            return this;
        }

        public IRestHelper RequestParameter(Dictionary<string, string> parameter)
        {
            _parameter = parameter;
            return this;
        }

        public IRestHelper RequestBody(dynamic json)
        {
            _json = json;
            return this;
        }

        private dynamic DeserializeToJson(string content)
        {
            return JsonConvert.DeserializeObject<dynamic>(content);
        }

        private StringContent ConvertObjectToJson()
        {
            var t = new StringContent(JsonConvert.SerializeObject(_json), Encoding.UTF8, "application/json");
            return t;
        }

        private string AddParameters()
        {
            var builder = new StringBuilder(_parameter.Count);
            foreach (var param in _parameter)
            {
                builder.Append($"/{param.Value}");
            }
            return builder.ToString();
        }

        private string CreateUrl()
        {
            return $"{_hostUrl}/{_apiUrl}";
        }

        public dynamic ExecuteSafe()
        {
            try
            {
                return ExecuteSync();
            }
            catch (Exception)
            {
                return new JObject();
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

                var response = await _httpClient.GetAsync(new Uri($"{CreateUrl()}{AddParameters()}"));
                return DeserializeToJson(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var e = $"{_hostUrl}/{_apiUrl}/";

                var response = await _httpClient.PostAsync(new Uri($"{CreateUrl()}"), ConvertObjectToJson());
                return DeserializeToJson(await response.Content.ReadAsStringAsync());
            }
        }

        private dynamic ExecuteSync()
        {
            if (_restMethod == RestMethod.Get)
            {
                var t = $"{_hostUrl}/{_apiUrl}{AddParameters()}";
                var response = _httpClient.GetAsync(new Uri($"{CreateUrl()}{AddParameters()}")).Result;
                return DeserializeToJson(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                var response = _httpClient.PostAsync(new Uri($"{CreateUrl()}"), ConvertObjectToJson()).Result;
                return DeserializeToJson(response.Content.ReadAsStringAsync().Result);
            }

        }

    }
}