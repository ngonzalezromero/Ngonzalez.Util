namespace Ngonzalez.Util
{
    public interface IApiUtil
    {
        string DecryptData(string cipherText, string seed);
        string  EncriptData(string plainText, string seed);
        string GenerateApiKey();
        string GenerateHmacsha256(string message, string secret);
        string GeneratePassword();
        string GenerateUserKey();
    }
}