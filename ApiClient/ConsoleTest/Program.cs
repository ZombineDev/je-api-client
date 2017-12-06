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

            var jeTestEmail = "test-account@jarvis-edge.io";
            var jeTestPass = "test";

            var res2 = client.Login(jeTestEmail, jeTestPass).Result;
            Console.WriteLine(res2.Result.Token);

            var guid = Guid.NewGuid().ToString();
            var pseudo = $"user1_{guid}";
            var email = $"test{guid}@mail.com";
            var pass = "Sup3rSecurePasswd!";

            var res3 = client.Register(pseudo, "first", "last", email, pass).Result;
            Console.WriteLine(res3.Result.Token);

            var res4 = client.RequestNewPassword(email).Result;
            Console.WriteLine(string.Join(", ", res4.ErrorAsArray.Select(x => x.Message)));

            var res5 = client.GetUser(res2.Result.Id, res2.Result.Token).Result;
            Console.WriteLine(res5.Result.LastName);

            var res6 = client.ModifyUser(res2.Result.Id, res2.Result.Token, new ModifyUser()
            {
                Email = jeTestEmail,
                Pseudo = jeTestPass,
                City = $"Sofia_{guid}"
            }).Result;

            Console.WriteLine(res6.Result.City);

            // TODO: Probably we need to activate the account, before we can delete it,
            // otherwise we get "Your Token is not valid" error.
            var testLogin = client.Login(email, pass).Result.Result;
            var res7 = client.DeleteUser(testLogin.Id, testLogin.Token).Result;
            //Console.WriteLine(res7.Result.Id);
        }
    }
}
