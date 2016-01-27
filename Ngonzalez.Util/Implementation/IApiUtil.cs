namespace Ngonzalez.Util
{
    public interface IApiUtil
    {
        string DecryptData(string cipherText, string seed);
        string  EncriptData(string plainText, string seed);
        string GenerateApiKey();
        string GeneratePassword();
        string GenerateUserKey();
    }
}