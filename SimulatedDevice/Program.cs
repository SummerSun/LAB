namespace SimulatedDevice
{
    using System;
    using System.IO;
    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Client;
    using TransportType = Microsoft.Azure.Devices.Client.TransportType;

    public class Program
    {
        private const string IotHubUri = "qisun-iothub.azure-devices.net";
        private const string DeviceKey = "9sWysLOdM21UrwgsZ9MZv7RRftbxzqEofo12Zz2Rjuk=";
        private const string DeviceId = "AZ3166";
        static ServiceClient serviceClient;
        private static DeviceClient _deviceClient;

        private static async void SendToBlobAsync()
        {
            string fileName = "test.wav";
            Console.WriteLine("Uploading file: {0}", fileName);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            using (var sourceData = new FileStream(fileName, FileMode.Open))
            {
                await _deviceClient.UploadToBlobAsync(fileName, sourceData);
            }

            watch.Stop();
            Console.WriteLine("Time to upload file: {0}ms\n", watch.ElapsedMilliseconds);
        }
        

        private static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            _deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey), TransportType.Mqtt);

            SendToBlobAsync();
            
            Console.ReadLine();
        }
        
    }
}

