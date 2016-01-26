using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using R256 = Rijndael256.Rijndael;
using R256Key = Rijndael256.KeySize;

namespace Ngonzalez.Util
{
    public sealed class ApiUtil :IApiUtil
    {

        private const string AllowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
        private const byte MaxLength = 15;
        

        public string EncriptData(string plainText, string seed)
        {
            return R256.Encrypt(plainText, seed, R256Key.Aes256);
        }

        public string DecryptData(string cipherText, string seed)
        {
            try
            {
                return R256.Decrypt(cipherText, seed, R256Key.Aes256);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GenerateApiKey()
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                var data = new byte[10];
                crypto.GetNonZeroBytes(data);
                var response = new StringBuilder();
                foreach (byte b in data)
                {
                    response.Append(b);
                }
                return response.ToString();
            }
        }

        public string GenerateUserKey()
        {
            var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * ((int)b + 1));
            return $"{i - DateTime.Now.Ticks:x}";
        }

        public string GenerateHmacsha256(string message, string secret)
        {
            var encoding = new ASCIIEncoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage).Replace("+", "");
            }
        }

        public string GeneratePassword()
        {
            var charGroups = new[]
            {
                CreateRandomChar (MaxLength),
                CreateRandomChar (MaxLength)
            };

            var charsLeftInGroup = new int[charGroups.Length];
            for (var j = 0; j < charsLeftInGroup.Length; j++)
            {
                charsLeftInGroup[j] = charGroups[j].Length;
            }

            var leftGroupsOrder = new int[charGroups.Length];

            for (var j = 0; j < leftGroupsOrder.Length; j++)
            {
                leftGroupsOrder[j] = j;
            }

            var randomBytes = new byte[4];

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            var seed = (randomBytes[0] & 0x7f) << 24 |
                       randomBytes[1] << 16 |
                       randomBytes[2] << 8 |
                       randomBytes[3];

            var random = new Random(seed);

            var password = new char[random.Next(MaxLength, MaxLength)];
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            for (int j = 0; j < password.Length; j++)
            {
                int nextLeftGroupsOrderIdx = lastLeftGroupsOrderIdx == 0 ? 0 : random.Next(0, lastLeftGroupsOrderIdx);
                int nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];
                int lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;
                int nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);
                password[j] = charGroups[nextGroupIdx][nextCharIdx];

                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                else
                {

                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    charsLeftInGroup[nextGroupIdx]--;
                }

                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else
                {

                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                            leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    lastLeftGroupsOrderIdx--;
                }
            }
            return new string(password);
        }

        private static char[] CreateRandomChar(int stringLength)
        {
            var chars = new char[stringLength];
            var rd = new Random();

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = AllowedChars[rd.Next(0, AllowedChars.Length)];
            }
            return chars;
        }

        public Func<T, object> GetLambda<T>(string property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property);

            if (!parent.Type.IsValueType)
            {
                return Expression.Lambda<Func<T, object>>(parent, param).Compile();
            }
            var convert = Expression.Convert(parent, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param).Compile();
        }

        public string GetMonthName(int month)
        {
            var temp = string.Empty;
            switch (month)
            {
                case 1:
                    temp = "January";
                    break;
                case 2:
                    temp = "February ";
                    break;
                case 3:
                    temp = "March";
                    break;
                case 4:
                    temp = "April";
                    break;
                case 5:
                    temp = "May";
                    break;
                case 6:
                    temp = "June";
                    break;
                case 7:
                    temp = "July";
                    break;
                case 8:
                    temp = "August";
                    break;
                case 9:
                    temp = "September";
                    break;
                case 10:
                    temp = "October";
                    break;
                case 11:
                    temp = "November";
                    break;
                case 12:
                    temp = "December";
                    break;
            }
            return temp;
        }
    }
}
