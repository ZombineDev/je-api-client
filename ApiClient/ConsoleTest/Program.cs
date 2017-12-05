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
        }
    }
}