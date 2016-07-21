using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Ngonzalez.Util
{
    public interface IApiUtil
    {
        string DecryptData(string cipherText, string seed, string iv);
        string EncriptData(string plainText, string seed, string iv);
        string GenerateApiKey();
        string GeneratePassword();
        string GenerateUserKey();
        string GetMonthName(int month);
        void ValidPaginationParameter<T>(int? pageIndex, int? pageSize, string column, bool? orderDescending);
        Expression<Func<T, object>> GetLambda<T>(string property);
        bool IsMobileBrowser(HttpRequest request);
        string GetExceptionDetails(Exception exception);
        string GetRemoteInfo(IHttpRequestFeature request,IHttpConnectionFeature connection);
    }
}