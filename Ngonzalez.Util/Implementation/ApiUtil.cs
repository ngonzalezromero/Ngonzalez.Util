using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using Ngonzalez.Util.CustomException;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Ngonzalez.Util
{
    public sealed class ApiUtil : IApiUtil
    {

        private const string AllowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
        private const byte MaxLength = 15;

        private static readonly Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|iPad", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);



        public string EncriptData(string plainText, string seed, string iv)
        {
            return new Aes(seed, iv).Encrypt(plainText);
        }

        public string DecryptData(string cipherText, string seed, string iv)
        {
            try
            {
                return new Aes(seed, iv).Decrypt(cipherText);
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

        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public Func<T, object> GetLambda<T>(string property)
        {
            var param = Expression.Parameter(typeof(T), "p");

            Expression parent = Expression.Property(param, property);


#if NET451
            if (!parent.Type.IsValueType)
#endif

#if NETCOREAPP1_0
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
                throw new AppException("Invalid pageIndex");
            }

            if (pageSize == null)
            {
                throw new AppException("Invalid pageSize");
            }

            if (string.IsNullOrWhiteSpace(column) || column.ToLower() == "null")
            {
                throw new AppException("Invalid column");
            }

            if (orderDescending == null)
            {
                throw new AppException("Invalid order");
            }

            if (typeof(T).GetProperties().All(x => x.Name != column))
            {
                throw new AppException($"Column {column} no exists");
            }

        }

        public bool IsMobileBrowser(HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].ToString();
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return true;
            }

            return false;
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

        public string GetExceptionDetails(Exception exception)
        {
            return $"Exception: {exception.GetType()}\r\nInnerException: {exception.InnerException}\r\nMessage: {exception.Message}\r\nStackTrace: {exception.StackTrace}\r\n Full Trace: {exception.ToString()}";
        }



    }

}
