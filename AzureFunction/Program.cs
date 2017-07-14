/**
Copyright (c) Microsoft Corporation
All rights reserved. 
MIT License
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechSample
{
    /*
     * This class demonstrates how to get a valid O-auth token.
     */
    public class Authentication
    {
        private string _subscriptionKey;
        private Timer _accessTokenRenewer;
        private string _token;
        public const string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        public const int TokenExpireDuration = 10;

        public Authentication(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
            _token = FetchToken(FetchTokenUri, subscriptionKey).Result;
            _accessTokenRenewer = new Timer(OnTokenExpiredCallback, this, TimeSpan.FromMinutes(TokenExpireDuration - 1), TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return _token;
        }

        private void RenewAccessToken()
        {
            _token = FetchToken(FetchTokenUri, _subscriptionKey).Result;
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed renewing access token. Details: ${ex.Message}");
            }
            finally
            {
                try
                {
                    _accessTokenRenewer.Change(TimeSpan.FromMinutes(TokenExpireDuration -1), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to reschedule the timer to renew access token. Details: {ex.Message}");
                }
            }
        }

        private static async Task<string> FetchToken(string fetchUri, string subscriptionKey)
        {
            using (var client = new HttpClient())
            {
                var uriBuilder = new UriBuilder(fetchUri);
                uriBuilder.Path += "/issueToken";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }

    /*
     * This sample program shows how to send an speech recognition request to the 
     * Microsoft Speech service.      
     */
    class Program
    {
        static void Main(string[] args)
        {
            // Note: Sign up at http://www.projectoxford.ai to get a subscription key.  Search for Speech APIs from Azure Marketplace.  
            // Use the subscription key as Client secret below.
            Authentication auth = new Authentication("41242023732243be8f896a2dcb9b5920");
            var newRequestUri = "https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=en-US&format=detailed&requestid=" + Guid.NewGuid();
            
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

            /*
             * Input your own audio file or use read from a microphone stream directly.
             */
            string audioFile = @"test.wav";
            string responseString;

            try
            {
                var token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token);
                Console.WriteLine("Request Uri: " + newRequestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)WebRequest.Create(newRequestUri);
                request.SendChunked = true;
                //request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                //request.ProtocolVersion = HttpVersion.Version11;
                //request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = "Bearer " + token;

                using (var fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
                {

                    /*
                     * Open a request stream and write 1024 byte chunks in the stream one at a time.
                     */
                    byte[] buffer = null;
                    int bytesRead = 0;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        /*
                         * Read 1024 raw bytes from the input audio file.
                         */
                        buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                        // Flush
                        requestStream.Flush();
                    }

                    /*
                     * Get the response from the service.
                     */
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);

                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }

                        Console.WriteLine(responseString);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}