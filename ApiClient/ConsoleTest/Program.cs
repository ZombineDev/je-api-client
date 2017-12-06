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

            var jeLoginReq = new LoginRequest()
            {
                Email = "test-account@jarvis-edge.io",
                Password = "test"
            };

            var res2 = client.Login(jeLoginReq).Result;
            Console.WriteLine(res2.Result.Token);

            var guid = Guid.NewGuid().ToString();
            var newUser = new RegisterUserRequest()
            {
                Pseudo = $"user1_{guid}",
                FirstName = "first",
                LastName = "last",
                Email = $"test{guid}@mail.com",
                Password = "Sup3rSecurePasswd!"
            };

            var res3 = client.Register(newUser).Result;
            Console.WriteLine(res3.Result.Token);

            var res4 = client.RequestNewPassword(newUser.Email).Result;
            Console.WriteLine(string.Join(", ", res4.ErrorAsArray.Select(x => x.Message)));

            var res5 = client.GetUser(res2.Result.Id, res2.Result.Token).Result;
            Console.WriteLine(res5.Result.LastName);

            var res6 = client.ModifyUser(res2.Result.Id, res2.Result.Token, new ModifyUser()
            {
                Email = jeLoginReq.Email,
                Pseudo = res2.Result.Pseudo,
                City = $"Sofia_{guid}"
            }).Result;

            Console.WriteLine(res6.Result.City);

            // TODO: Probably we need to activate the account, before we can delete it,
            // otherwise we get "Your Token is not valid" error.
            var loginReq = new LoginRequest() { Email = newUser.Email, Password = newUser.Password };
            var testLogin = client.Login(loginReq).Result.Result;
            var res7 = client.DeleteUser(testLogin.Id, testLogin.Token).Result;
            //Console.WriteLine(res7.Result.Id);

            var res8 = client.SearchUsers(res2.Result.Token, "%").Result;
            foreach (var u in res8.Result)
                Console.WriteLine($"#{u.Id} {u.FirstName} {u.LastName} [{u.Pseudo}] " +
                    "FriendState: {u.FriendState} LinkState: {u.LinkState}");
        }
    }
}
