using System;
using Microsoft.AspNet.Http;

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
        Func<T, object> GetLambda<T>(string property);
        bool IsMobileBrowser(HttpRequest request);
    }
}