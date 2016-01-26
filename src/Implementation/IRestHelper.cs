using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ngonzalez.Util
{
    public enum RestMethod
    {
        Post,
        Put,
        Get,
        Delete
    }

    public interface IRestHelper
    {
        IRestHelper UrlHost(string hostUrl);
        IRestHelper UrlApi(string apiUrl);
        IRestHelper HttpMethod(RestMethod method);
        IRestHelper RequestParameter(Dictionary<string, string> parameter);
        IRestHelper RequestBody(dynamic json);
        dynamic ExecuteSafe();
        dynamic Execute();
        Task<object> ExecuteAsync();
    }
}