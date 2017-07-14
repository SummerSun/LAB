namespace HttpSample
{
    using System;
    using System.Net.Http;

    public class HttpSample
    {
        private static readonly Lazy<HttpSample> Lazy = new Lazy<HttpSample>(()=>new HttpSample());
        public HttpSample Instance => Lazy.Value;
        private static readonly HttpClient Client = new HttpClient();
        private const string Host = "https://www.google.com";
        static void Main(string[] args)
        {
           Client.BaseAddress = new Uri(Host);
            var response = Client.GetAsync("/gmail").Result;
            Console.WriteLine(response.Content);
            Console.ReadLine();

        }
    }
}
