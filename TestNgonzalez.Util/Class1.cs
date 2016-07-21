﻿using Ngonzalez.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using Ngonzalez.Util.Logging;
using Microsoft.Extensions.Logging;

namespace TestNgonzalez.Util
{

    public class Class1
    {
        private const string Password = "AUUKXLPQMyUUhALQGUKAmttCcRqIxCKj";
        private const string iv = "HH5UNPJAI668QM6S";

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
            int zero = 0;
            try
            {
                var fail = File.OpenRead("c:/");
            }
            catch (System.Exception ex)
            {
                ee = ex;
            }
            var val = e.GetExceptionDetails(ee);
            Console.WriteLine(val);
            Assert.True(val != null);

        }



        [Fact]
        public void ShouldInsertLogger()
        {
            var root = "http://localhost:32934";
            var api = "Logger/Insert";

            Func<Poco, Exception, string> logFilter = (Poco p, Exception logLevel) =>
            {
               
                return "";
            };

            Console.WriteLine("Test Logger");
            var log = new ApiLogger(LogLevel.Error, "loggerName", null, new RestHelper(), root, api, "22410613140124249126217019", "test");
            log.Log(LogLevel.Error, 1, new Poco(), new Exception("Fake EXception"), logFilter);
            Assert.True(true);
        }
        /*



          [Fact]
          public void TestingGetRestApi()
          {
              var root = "http://localhost:32934";
              var api = "Logger/GenerateApiKey";

              var param = new Dictionary<string, string>
              {
                  {"system", "dnxtest"}

              };
              var e = new RestHelper().UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Get).RequestParameter(param).ExecuteSafe();

              Assert.True(e.message == "New ApiKey" || e.message == "Renew ApiKey");
          }

          [Fact]
          public async Task TestingGetRestApiAsync()
          {
              var root = "http://localhost:32934";
              var api = "Logger/GenerateApiKey";

              var param = new Dictionary<string, string>
              {
                  {"system", "dnxtest"}

              };
              dynamic e = await new RestHelper().UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Get).RequestParameter(param).ExecuteAsync();
              Console.WriteLine(e);
              Assert.True(e.message == "New ApiKey" || e.message == "Renew ApiKey");
          }
  

        [Fact]
        public void TestingPostRestApi()
        {
            var root = "http://jsonplaceholder.typicode.com";
            var api = "posts";

            var e = new RestHelper().UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Post).RequestBody(new { title = "foo", body = "bar", userId = "1" }).ExecuteSafe();
            Assert.True(e.id == 101);
        }



        [Fact]
        public async Task TestingPostRestApiAsync()
        {
            var root = "http://jsonplaceholder.typicode.com";
            var api = "posts";

            dynamic e = await new RestHelper().UrlHost(root).UrlApi(api).HttpMethod(RestMethod.Post).RequestBody(new { title = "foo", body = "bar", userId = "1" }).ExecuteAsync();
            Assert.True(e.id == 101);
        }
        */

    }


    internal class Poco
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

}
