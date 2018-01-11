using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCameraApp
{
    class SendMessageToCloud
    {
        private const string DEVICENAME = "Monitor";// It's hard-coded for this workshop
        private static DeviceClient _deviceClient;
        private static bool _isStopped = false;
        ArrayList EmotionArray = new ArrayList();
        public async void sendWindTurbineMessageToCloudAsync( string PersonName,string PersonAge, string PersonGender, string PersonEmotion, string DeviceConnectionString)
        {
            // Random rand = new Random();
            // string[] names = { "Amy", "Alice", "Cindy", "Eric", "Jackie", "Joanne", "Jonathan", "York", "None" };

            int i = 1;

            _deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);
            Console.WriteLine("Connect to iot hub");
            if (_isStopped == false)
            {
                //string tmp_name = names[rand.Next(names.Length)];
                int tmp_open = 1;
                if (String.Compare(PersonName, "None", true) == 0)
                {
                    tmp_open = 0;
                }


                //foreach (KeyValuePair<string, float> element in EmotionScoreList)
                //{
                    //Console.WriteLine("Key: " + element.Key + " Value: " + element.Value);
                    //EmotionArray.Add(element.Value);
                //}

                var telemetryDataPoint = new
                {
                    deviceId = DEVICENAME,
                    msgId =   i,
                    name = PersonName,
                    open = tmp_open,
                    age = PersonAge,
                    gender = PersonGender,
                    emotion = PersonEmotion,
                    time = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ") // ISO8601 format, https://zh.wikipedia.org/wiki/ISO_8601
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("Device", "Camera");
                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                i++;

            }
            else
            {
                Console.WriteLine("{0} > Turn Off", DateTime.Now);

            }

        }
    }
}
