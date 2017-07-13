using Ngonzalez.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Flurl.Http;
using Ngonzalez.Util.Implementation;

namespace TestNgonzalez.Util
{

    public class Class1
    {
        private const string Password = "AUUKXLPQMyUUhALQGUKAmttCcRqIxCKj";
        private const string iv = "HH5UNPJAI668QM6S";

        private static FlurlClient fc = new FlurlClient();

        private static HttpClient m = new HttpClient();

        private RestHelper rest = new RestHelper(fc);

        [Fact]
        public void TestStringBuilderCache()
        {
            var str = "[hola]";

            var builder = StringBuilderCache.AcquireBuilder();
            builder.Append(str);
            builder.Replace("[", "").Replace("]", "");
            Assert.True("hola" == StringBuilderCache.GetStringAndReleaseBuilder(builder));
        }

        [Fact]
        public void shoulbeSameUrl()
        {

            var e = new ApiUtil();

            for (int i = 0; i < 200000; i++)
            {
                var url = $"*{DateTime.Now}*{e.GenerateUserKey()}*ngonzalezromero@gmail.com";

                var encript = $"{e.EncriptUrl(url, Password, iv)}";
                var decript = e.DecryptUrl(encript, Password, iv);
                Assert.True(encript != null);
                Assert.True(decript != null);
                Assert.False(encript.Contains("+"));
                Assert.False(encript.Contains("/"));
                Assert.False(encript.Contains("="));
                Assert.True(url == decript);
                Assert.True(url == decript);

                var urlSplit = url.Split(new char[] { '*' });
                var urlDescriptSplit = decript.Split(new char[] { '*' });
                Assert.True(urlSplit[1] == urlDescriptSplit[1]);
                Assert.True(urlSplit[3].Contains('@'));
            }
        }

        [Fact]
        public void ShouldPasswordNotSpecialCharacters()
        {
            var util = new ApiUtil();
            for (int i = 0; i < 1000; i++)
            {
                var password = util.GeneratePassword();
                Console.WriteLine(password);
                Assert.False(password.Contains("+"));
                Assert.False(password.Contains("/"));

            }
        }

        [Fact]
        public void TestSerializacion()
        {
            var poco = new Poco { Id = "1", Name = "Nicolas", Date = DateTime.Now };

            var one = CacheUtilExtensions.Serializer(poco);
            var two = CacheUtilExtensions.Deserializer<Poco>(one);
            Assert.True(poco.Id == two.Id);
            Assert.True(poco.Name == two.Name);
            Assert.True(poco.Date == two.Date);
        }

        [Fact]
        public async Task TestEmail()
        {
            var email = await new AsyncMail()
                 .SmtpUser("inforeader")
                 .SmtpHost("mail.amtc.cl")
                 .SmtpPort(587)
                 .SmtpPassword("info$$00")
                 .Addresses(new List<string>() { "ngonzalezromero@gmail.com" })
                 .Body("Test")
                 .Cc("Nicolas.Gonzalez@csiro.au")
                 .SenderAddress("inforeader@amtc.cl")
                 .SenderName("Test User")
                 .Subject("Testing")
                 .BuildMailAsync();

            Assert.True(email);
        }

        [Fact]
        public async Task TestWeather()
        {
            var param = new Dictionary<string, dynamic>
                    {
                        {"Apikey","2a09ef377ac9619827a05819dbcbb815"},
                        {"latitude", "-22.3208804,-68.9033803"},
                    };

            dynamic json = await rest.UrlHost("https://api.forecast.io")
                   .UrlApi($"forecast")
                   .HttpMethod(RestMethod.Get)
                   .RequestParameter(param)
                   .SetQueryParam("units", "ca")
                   .ExecuteAsync();

            Assert.True(json != null);
        }
        /*

        [Fact]
        public async Task TestingPostRestApi()
        {
            FlurlClient fc = new FlurlClient();
            var root = "http://localhost:51101";
            var api = "api/v1/system/testdbconnection";
            var list = new List<Task>();
            var param = new Dictionary<string, dynamic> { { "param", "param" } };

            for (int i = 0; i < 325; i++)
            {
                list.Add(rest.UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Get).RequestParameter(param).ExecuteAsync());

            }
            await Task.WhenAll(list.ToArray());

            int count = 0;
            foreach (Task<dynamic> item in list)
            {
                string p = item.Result;
                Console.WriteLine(p);
                Console.WriteLine(count++);
            }
        }
        */

        [Theory]
        [InlineData("1234")]

        [InlineData("1@1234.sh")]
        [InlineData("0451-55530000")]
        [InlineData("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq")]
        [InlineData("qwertyuiopasdfghjklzxcvbnm1234567890~！@#￥%……&*（）qwertyuiopasdfghjklzxcvbnm1234567890~！@#￥%……&*（）qwertyuiopasdfghjklzxcvbnm1234567890~！@#￥%……&*（）")]
        public void shoulbeSamePassword(string str)
        {

            var e = new ApiUtil();
            var encript = e.EncriptData(str, Password, iv);

            var decript = e.DecryptData(encript, Password, iv);
            Assert.True(encript != null);
            Assert.True(decript != null);
            Assert.True(str == decript);
        }

        [Theory]
        [InlineData("file<name .docx")]
        [InlineData("*@123\\|?4.sh")]
        [InlineData("g?gd dg.xlsx")]
        [InlineData("g?gd dg.xls<x")]
        [InlineData("g? gd *dg.xl?s<x")]


        public void shoulCleanFileName(string str)
        {
            var e = new ApiValidation();
            var decript = e.SanitizeFileName(str);
            Assert.True(decript != str);

        }

        [Theory]
        [InlineData("file<name .docx")]
        [InlineData("*@123\\|?4.jpg")]
        [InlineData("g?gd dg.xlsx")]
        [InlineData("g?gd dg.xls")]
        [InlineData("g? gd *dg.do<c")]


        public void shoulNotFailExtension(string str)
        {
            var e = new ApiValidation();
            var decript = e.SanitizeFileName(str);
            var r = e.CheckFileExtension(decript, new List<string>() { "docx", "jpg", "xlsx", "xls", "doc" });
            Assert.True(r);
        }


        [Theory]
        [InlineData("file<name .docx")]
        [InlineData("*@123\\|?4.jpg")]
        [InlineData("g?gd dg.xlsx")]
        [InlineData("g?gd dg.xls")]
        [InlineData("g? gd *dg.do<c")]


        public void shoulFailExtension(string str)
        {
            var e = new ApiValidation();
            var decript = e.SanitizeFileName(str);
            var r = e.CheckFileExtension(decript, new List<string>() { "png" });
            Assert.False(r);
        }




        [Fact]
        public void ShouldBeANumberApiKey()
        {
            var e = new ApiUtil();
            var list = new List<string>();

            for (int i = 0; i < 1000; i++)
            {
                list.Add(e.GenerateApiKey());
            }

            var distinctBytes = new HashSet<string>(list);
            bool allDifferent = distinctBytes.Count == list.Count;

            Assert.True(allDifferent);
        }

        [Fact]
        public void ShouldBeANorNullUserSecret()
        {
            var e = new ApiUtil();
            var list = new List<string>();

            for (int i = 0; i < 100000; i++)
            {
                list.Add(e.GenerateUserKey());
            }

            var check = list.All(x => !string.IsNullOrWhiteSpace(x));
            Assert.True(check);
        }

        [Fact]
        public void ShouldBeANorNullAnd15LengthPassword()
        {
            var e = new ApiUtil();
            var list = new List<string>();

            for (int i = 0; i < 100000; i++)
            {
                list.Add(e.GeneratePassword());
            }
            var check = list.All(x => !string.IsNullOrWhiteSpace(x));
            var checkLength = list.All(x => x.Length <= 15);
            Assert.True(check);
            Assert.True(checkLength);
        }

        [Fact]
        public void ShouldBeFailALAmbda()
        {
            var e = new ApiUtil();
            Assert.Throws<ArgumentException>(() => e.GetLambda<Poco>("Idu"));
        }

        [Fact]
        public void ShouldBeNotFailALAmbda()
        {
            var e = new ApiUtil();
            var lambda = e.GetLambda<Poco>("Date");
            Assert.True(true);
        }

        [Fact]
        public void ShouldCleanParameterNotNull()
        {
            var e = new ApiValidation();
            var val = e.CleanParameter<string>("nicolas");
            Assert.True(val != null);
        }

        [Fact]
        public void ShouldCleanParameterSameEquals()
        {
            var e = new ApiValidation();
            var val = e.CleanParameter<string>("nicolas");
            Assert.True(val != null);
        }


        [Fact]
        public void GetAllException()
        {
            var e = new ApiUtil();
            Exception ee = null;
            try
            {
                var fail = File.OpenRead("c:/");
            }
            catch (System.Exception ex)
            {
                ee = ex;
            }
            var val = e.GetExceptionDetails(ee);
            Assert.True(val != null);

        }

        [Fact]
        public void TestingPostRestApi2()
        {
            var root = "http://jsonplaceholder.typicode.com";
            var api = "posts";

            var e = rest.UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Post).RequestBody(new { title = "foo", body = "bar", userId = "1" }).ExecuteSafe();
            Assert.True(e.id == 101);
        }

        [Fact]
        public async Task TestingPostRestApiAsync()
        {
            var root = "http://jsonplaceholder.typicode.com";
            var api = "posts";
            dynamic e = await rest.UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Post).RequestBody(new { title = "foo", body = "bar", userId = "1" }).ExecuteAsync();

        }

    }


    internal class Poco
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

}
