using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
namespace Ngonzalez.Util
{
    public sealed class ApiUtil: IApiUtil
    {

        private const string AllowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
        private const byte MaxLength = 15;

        public string EncriptData(string plainText, string seed, string iv)
        {
            return new Aes(seed,iv).Encrypt(plainText);
        }

        public string DecryptData(string cipherText, string seed,string iv)
        {
            try
            {
                return new Aes(seed,iv).Decrypt(cipherText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GenerateApiKey()
        {
            using (var crypto = RandomNumberGenerator.Create())
            {
                var data = new byte[10];
                crypto.GetBytes(data);
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

        public string GeneratePassword()
        {
            return Truncate(new Aes().Encrypt(DateTime.Now.Ticks.ToString()), MaxLength);
        }

        private string Truncate( string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public Func<T, object> GetLambda<T>(string property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property);


#if DNX451
            if (!parent.Type.IsValueType)
#endif

#if DNXCORE50
                if (!parent.GetType().GetTypeInfo().IsValueType)
#endif



            {
                return Expression.Lambda<Func<T, object>>(parent, param).Compile();
            }
            var convert = Expression.Convert(parent, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, param).Compile();
        }
        
        
        public void ValidPaginationParameter<T>(int? pageIndex, int? pageSize, string column, bool? orderDescending)
        {
            if (pageIndex == null)
            {
                ExceptionLogAndThrow<AppException>("Invalid pageIndex");
            }

            if (pageSize == null)
            {
                ExceptionLogAndThrow<AppException>("Invalid pageSize");
            }

            if (string.IsNullOrWhiteSpace(column) || column.ToLower() == "null")
            {
                ExceptionLogAndThrow<AppException>("Invalid column");
            }

            if (orderDescending == null)
            {
                ExceptionLogAndThrow<AppException>("Invalid order");
            }

            if (typeof(T).GetProperties().All(x => x.Name != column))
            {
                ExceptionLogAndThrow<AppException>($"Column {column} no exists");
            }

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
