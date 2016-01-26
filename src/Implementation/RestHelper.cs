using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Ngonzalez.Util
{
    public sealed class RestHelper : IRestHelper
    {
        private string _apiUrl;
        private dynamic _json;
        private RestMethod _restMethod;
        private RestClient _restClient;
        private Dictionary<string, string> _parameter;


        public IRestHelper UrlHost(string hostUrl)
        {
            _restClient = new RestClient(hostUrl);
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


        public dynamic ExecuteSafe()
        {
            try
            {
                var request = new RestRequest { Resource = _apiUrl, Method = ParseMethod(_restMethod) };

                if (request.Method == Method.GET)
                {
                    if (_parameter != null)
                    {
                        foreach (var param in _parameter)
                        {
                            request.AddParameter(param.Key, param.Value, ParameterType.UrlSegment);
                        }
                    }
                }
                else
                {
                    request.AddParameter("text/json", JsonConvert.SerializeObject(_json), ParameterType.RequestBody);
                }
                request.RequestFormat = DataFormat.Json;
                var response = _restClient.Execute(request);
                return !string.IsNullOrWhiteSpace(response.Content) ? JObject.Parse(response.Content) : new JObject();
            }
            catch (Exception)
            {
                return new JObject();
            }
        }

        public dynamic Execute()
        {
            var request = new RestRequest { Resource = _apiUrl, Method = ParseMethod(_restMethod) };
            if (request.Method == Method.GET)
            {
                if (_parameter != null)
                {
                    foreach (var param in _parameter)
                    {
                        request.AddParameter(param.Key, param.Value, ParameterType.UrlSegment);
                    }
                }
            }
            else
            {
                request.AddParameter("text/json", JsonConvert.SerializeObject(_json), ParameterType.RequestBody);
            }

            request.RequestFormat = DataFormat.Json;
            var response = _restClient.Execute(request);

            if (response.ErrorException != null)
            {
                throw new ServerErrorException("Conection Server Error,Please contact the server administrator.", response.ErrorException);
            }

            if (response.StatusCode == (HttpStatusCode)403)
            {
                throw new ValidationApiException("ApiKey Invalid", response.ErrorException);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizedException("No access", response.ErrorException);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServerErrorException("Server Error", response.ErrorException);
            }
            return JObject.Parse(response.Content);
        }

        public async Task<dynamic> ExecuteAsync()
        {
            var request = new RestRequest { Resource = _apiUrl, Method = ParseMethod(_restMethod) };

            if (request.Method == Method.GET)
            {
                if (_parameter != null)
                {
                    foreach (var param in _parameter)
                    {
                        request.AddParameter(param.Key, param.Value, ParameterType.UrlSegment);
                    }
                }
            }
            else
            {
                request.AddParameter("text/json", JsonConvert.SerializeObject(_json), ParameterType.RequestBody);
            }

            request.RequestFormat = DataFormat.Json;
            var response = await _restClient.ExecuteTaskAsync(request);

            if (response.ErrorException != null)
            {
                throw new ServerErrorException("Conection Server Error,Please contact the server administrator.", response.ErrorException);
            }

            if (response.StatusCode == (HttpStatusCode)403)
            {
                throw new ValidationApiException("ApiKey Invalid", response.ErrorException);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizedException("No access", response.ErrorException);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServerErrorException("Server Error", response.ErrorException);
            }
            return JObject.Parse(response.Content);
        }

        private static Method ParseMethod(RestMethod method)
        {
            switch (method)
            {
                case RestMethod.Post:
                    return Method.POST;
                case RestMethod.Put:
                    return Method.PUT;
                case RestMethod.Delete:
                    return Method.DELETE;
                case RestMethod.Get:
                    return Method.GET;
                default:
                    return Method.POST;
            }
        }
    }
}