using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api
{
    public class Response<T>
    {
        public string Status { get; set; }
        public string Error { get; set; }
        public T Result { get; set; }
    }

    public class Basic
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public string Socket { get; set; }
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

        // POST https://api.jarvis-edge.io/users/login/
        public async Task<Response<LoginResult>> Login(string email, string password)
        {
            var args = new List<KeyValuePair<string, string>>();
            args.Add(new KeyValuePair<string, string>("email", email));
            args.Add(new KeyValuePair<string, string>("password", password));
            var response = await client.PostAsync("/users/login", new FormUrlEncodedContent(args));
            var result = await response.Content.ReadAsAsync<Response<LoginResult>>(mediaFormatters.Value);
            return result;
        }
    }
}