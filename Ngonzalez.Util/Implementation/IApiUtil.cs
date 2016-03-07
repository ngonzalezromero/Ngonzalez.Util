using System;

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
        Func<T, object> GetLambda<T>(string property); 
    }
}