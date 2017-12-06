using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api
{
    public class Response<T>
    {
        public string Status { get; set; }
        public Object Error { get; set; }
        public T Result { get; set; }

        public string ErrorAsString => Error as string;
        public Error[] ErrorAsArray => Error as Error[];

        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (Error is Newtonsoft.Json.Linq.JArray json)
                Error = json.ToObject<Error[]>();
        }
    }

    public class Error
    {
        public string Message { get; set; }
    }

    public class Basic
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public string Socket { get; set; }
    }

    public class RegisterResult
    {
        public string Token { get; set; }
    }

    public class RequestNewPasswordResult
    {
        public string Email { get; set; }
    }

    public class LoginResult
    {
        public int Id { get; set; }
        public string Pseudo { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SteemUserName { get; set; }
        public string SteemPostingKey { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Token { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Pseudo { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SteemUserName { get; set; }
        public string SteemPostingKey { get; set; }
        public int LinkState { get; set; }
        public int FriendState { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

    // Docs: https://api.docs.jarvis-edge.io/
    public class ApiClient
    {
        private readonly static Lazy<IEnumerable<MediaTypeFormatter>> mediaFormatters =
            new Lazy<IEnumerable<MediaTypeFormatter>>(() =>
            {
                var r = new JsonMediaTypeFormatter();
                r.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                return new[] { r };
            }, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private readonly HttpClient client;

        public static string BuildQuery(params (string, string)[] args)
        {
            string res = null;
            using (var tmp = new FormUrlEncodedContent(args.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2))))
                res = tmp.ReadAsStringAsync().Result;
            return res;
        }

        public ApiClient(string baseAddress = "https://api.jarvis-edge.io")
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
        }

        // GET https://api.jarvis-edge.io/basic/
        public async Task<Response<Basic>> GetBasicAsync()
        {
            var response = await client.GetAsync("/basic");
            var result = await response.Content.ReadAsAsync<Response<Basic>>(mediaFormatters.Value);
            return result;
        }

        // POST https://api.jarvis-edge.io/users/register/
        public async Task<Response<RegisterResult>> Register(
            string pseudo, string firstName, string lastName, string email, string password)
        {
            var args = new[]
            {
                new KeyValuePair<string, string>("pseudo", pseudo),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            };
            var response = await client.PostAsync("/users/register", new FormUrlEncodedContent(args));
            var result = await response.Content.ReadAsAsync<Response<RegisterResult>>(mediaFormatters.Value);
            return result;
        }

        // POST https://api.jarvis-edge.io/users/login/
        public async Task<Response<LoginResult>> Login(string email, string password)
        {
            var args = new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password)
            };
            var response = await client.PostAsync("/users/login", new FormUrlEncodedContent(args));
            var result = await response.Content.ReadAsAsync<Response<LoginResult>>(mediaFormatters.Value);
            return result;
        }

        // POST https://api.jarvis-edge.io/users/requestPassword/
        public async Task<Response<RequestNewPasswordResult>> RequestNewPassword(string email)
        {
            var args = new[]
            {
                new KeyValuePair<string, string>("email", email),
            };
            var response = await client.PostAsync("/users/requestPassword", new FormUrlEncodedContent(args));
            var result = await response.Content.ReadAsAsync<Response<RequestNewPasswordResult>>(mediaFormatters.Value);
            return result;
        }

        // GET: https://api.jarvis-edge.io/users/{userId}/
        public async Task<Response<User>> GetUser(int userId, string token)
        {
            var query = BuildQuery(("token", token));
            var response = await client.GetAsync($"users/{userId}?{query}");
            var result = await response.Content.ReadAsAsync<Response<User>>(mediaFormatters.Value);
            return result;
        }
    }
}
