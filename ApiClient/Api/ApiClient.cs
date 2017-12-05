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

        // The method wraps  https://api.jarvis-edge.io/basic/
        public async Task<Response<Basic>> GetBasicAsync()
        {
            var response = await client.GetAsync("/basic");
            var content = await response.Content.ReadAsAsync<Response<Basic>>(mediaFormatters.Value);
            return content;
        }
    }
}
