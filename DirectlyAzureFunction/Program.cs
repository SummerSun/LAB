namespace DirectlyAzureFunction
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Common.Extensions;
    using TransportType = Microsoft.Azure.Devices.Client.TransportType;

    static class Program
    {
        static void Main(string[] args)
        {
            /*
            var header = Encoding.ASCII.GetBytes("zh-CN,en\r\n");
            var fileContent = File.ReadAllBytes("test.wav");
            byte[] whole = new byte[header.Length + fileContent.Length];
            Buffer.BlockCopy(header, 0, whole, 0, header.Length);
            Buffer.BlockCopy(fileContent, 0, whole, header.Length, fileContent.Length);
            var ms = new MemoryStream(whole);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("source", "zh-CN");
            client.DefaultRequestHeaders.TryAddWithoutValidation("target", "en");
            var url =
                "https://qisunfun.azurewebsites.net/api/HttpTriggerDevKitTranslator?code=1utPtB3uOaGSK9cad/VbAToUcmryaEhKc8EZcecSZmfFt7vR4SOiww==";
            var re = client.PostAsync(url, new StreamContent(ms)).Result;

            Console.ReadLine();
            var c = new HttpRequestMessage();
            c.Headers.GetValues("source").FirstOrDefault();
            source.FirstOrDefault();
            */


            ///*
           var deviceClient = DeviceClient.CreateFromConnectionString("HostName=qisun-iothub.azure-devices.net;DeviceId=AZ3166;SharedAccessKey=9sWysLOdM21UrwgsZ9MZv7RRftbxzqEofo12Zz2Rjuk=", TransportType.Mqtt);
           Message receivedMessage = deviceClient.ReceiveAsync().Result;
           //deviceClient
           if (receivedMessage == null) return;
    
           Console.ForegroundColor = ConsoleColor.Yellow;
           Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
           Console.ResetColor();
    
           deviceClient.CompleteAsync(receivedMessage).Wait();
           Console.ReadLine();
           //*/
            // Run().Wait();
        }

        static async Task Run()
        {
            HttpClient client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var url =
                "http://qisunfun.azurewebsites.net/api/HttpTriggerDevKitTranslator?code=1utPtB3uOaGSK9cad/VbAToUcmryaEhKc8EZcecSZmfFt7vR4SOiww==";
            var content = Encoding.ASCII.GetBytes("zh-CN,en\r\n").Concat(File.ReadAllBytes("test.wav"));
            
            //await client.PostAsync(url, new StreamContent(new MemoryStream(content)));
            Console.WriteLine(DateTime.Now);

        }
    }
}
