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

        public string ErrorMsg
        {
            get
            {
                if (ErrorAsArray != null && ErrorAsArray.Length > 0)
                {
                    var errors = ErrorAsArray.Length == 1 ? "error" : "errors";
                    var errorMsgs = String.Join(",\n", ErrorAsArray.Select((x, i) => $"#{i + 1} \"{x.Message}\""));
                    return $"{ErrorAsArray.Length} {errors} detected:\n{errorMsgs}.";
                }
                else
                    return ErrorAsString;
            }
        }

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

    public class RegisterUserRequest
    {
        public string Pseudo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterResult
    {
        public string Token { get; set; }
    }

    public class RequestNewPasswordResult
    {
        public string Email { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult : ModifyUser
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Token { get; set; }
    }

    public class User : ModifyUser
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public int LinkState { get; set; }
        public int FriendState { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

    public class ModifyUser
    {
        public string Pseudo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string SteemUserName { get; set; }
        public string SteemPostingKey { get; set; }
    }

    public class ModifyUserResult
    {
        public string Token { get; set; }
    }

    public class DeleteUserResult
    {
        public int Id { get; set; }
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

        public static string BuildQuery(params (string, string)[] args)
        {
            string res = null;
            using (var tmp = new FormUrlEncodedContent(args.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2))))
                res = tmp.ReadAsStringAsync().Result;
            return res;
        }

        public static FormUrlEncodedContent ToFormUrlEncodedContent<T>(T obj)
        {
            string ToCamelCase(string inPascalCase)
            {
                var sb = new System.Text.StringBuilder(inPascalCase);
                sb[0] = Char.ToLower(sb[0]);
                return sb.ToString();
            }

            var pairs = obj.GetType().GetProperties()
                .Select(p => new KeyValuePair<string, string>(ToCamelCase(p.Name), p.GetValue(obj)?.ToString()))
                .Where(kv => kv.Value != null);

            return new FormUrlEncodedContent(pairs);
        }

        private readonly HttpClient client;

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
        public async Task<Response<RegisterResult>> Register(RegisterUserRequest user)
        {
            var request = ToFormUrlEncodedContent(user);
            var response = await client.PostAsync("/users/register", request);
            var result = await response.Content.ReadAsAsync<Response<RegisterResult>>(mediaFormatters.Value);
            return result;
        }

        // GET https://www.jarvis-edge.io/activation/{token}/
        public async Task<bool> ActivateAccount(string token)
        {
            token = System.Net.WebUtility.UrlEncode(token);

            bool ok = false;

            using (var tmpClient = new HttpClient())
            {
                tmpClient.BaseAddress = new Uri("https://www.jarvis-edge.io");
                var response = await client.GetAsync($"/activation/{token}");
                ok = response.IsSuccessStatusCode;
            }

            return ok;
        }

        // POST https://api.jarvis-edge.io/users/login/
        public async Task<Response<LoginResult>> Login(LoginRequest login)
        {
            var request = ToFormUrlEncodedContent(login);
            var response = await client.PostAsync("/users/login", request);
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

        // POST: https://api.jarvis-edge.io/users/{userId}/
        public async Task<Response<User>> ModifyUser(int userId, string token, ModifyUser user)
        {
            var query = BuildQuery(("token", token));
            var content = ToFormUrlEncodedContent(user);
            var response = await client.PostAsync($"/users/{userId}?{query}", content);
            var result = await response.Content.ReadAsAsync<Response<User>>(mediaFormatters.Value);
            return result;
        }

        // GET: https://api.jarvis-edge.io/users/{userId}/delete/
        public async Task<Response<DeleteUserResult>> DeleteUser(int userId, string token)
        {
            var query = BuildQuery(("token", token));
            var response = await client.GetAsync($"/users/{userId}/delete?{query}");
            var result = await response.Content.ReadAsAsync<Response<DeleteUserResult>>(mediaFormatters.Value);
            return result;
        }

        // GET: https://api.jarvis-edge.io/users/search
        public async Task<Response<User[]>> SearchUsers(string token, string searchQuery)
        {
            var query = BuildQuery(("token", token), ("search", searchQuery));
            var response = await client.GetAsync($"users/search?{query}");
            var result = await response.Content.ReadAsAsync<Response<User[]>>(mediaFormatters.Value);
            return result;
        }
    }
}
