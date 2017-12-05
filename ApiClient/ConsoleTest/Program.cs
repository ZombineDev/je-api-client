using Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ApiClient();
            var res = client.GetBasicAsync().Result;
            Console.WriteLine(res.Result.Name);

            var res2 = client.Login("test-account@jarvis-edge.io", "test").Result;
            Console.WriteLine(res2.Result.Token);

            var guid = Guid.NewGuid().ToString();
            var userName = $"user1_{guid}";
            var email = $"test{guid}@mail.com";

            var res3 = client.Register(userName, "first", "last", email, "Sup3rSecurePasswd!").Result;
            Console.WriteLine(res3.Result.Token);

            var res4 = client.RequestNewPassword(email).Result;
            Console.WriteLine(string.Join(", ", res4.Error.Select(x => x.Message)));
        }
    }
}
