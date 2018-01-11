
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;


namespace ServicebusApp
{
    class Program
    {
        /* Service Bus */
        private const string QueueName = "cloud2device";// It's hard-coded for this workshop

        /* IoT Hub */
        private static ServiceClient _serviceClient;
        private const string DEVICEID = "Monitor";

        static void Main(string[] args)
        {
            string serviceBusConnectionString = "Endpoint=sb://iotservicebus0104.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=emSxp91VqdESeqAU/+c6yIGaV6qbGVuUyK1DRoRzs2c=";
            string iotHubConnectionString = "HostName=IoThub0104.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=gxP0FYr44G0roJomrqbZNKTS2Q388WHoOa2LgNZZ4Vk=";

            Console.WriteLine("Console App for Alarm Service Bus...");
            Console.WriteLine("serviceBusConnectionString={0}\n", serviceBusConnectionString);
            Console.WriteLine("iotHubConnectionString={0}\n", iotHubConnectionString);

            // Retrieve a Queue Client
            QueueClient queueClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, QueueName);

            // Retrieve a Service Client of IoT Hub
            _serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);

            queueClient.OnMessage(message =>
            {
                Console.WriteLine("\n*******************************************************");
                //string msg = message.GetBody<String>();
                try
                {
                    //AlarmMessage alarmMessage = JsonConvert.DeserializeObject<AlarmMessage>(msg);
                    //ProcessAlarmMessage(alarmMessage);
                    //Console.WriteLine(msg);
                    Stream stream = message.GetBody<Stream>();
                    StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                    string s = reader.ReadToEnd();
                    var start = s.IndexOf("{");
                    var end = s.LastIndexOf("}") + 1;
                    var length = end - start;
                    // Get actual message
                    string cleandJsonString = s.Substring(start, length);

                    Console.WriteLine(String.Format("Message body: {0}", cleandJsonString));
                    ServiceBusApp.AlarmMessage alarmMessage = JsonConvert.DeserializeObject<ServiceBusApp.AlarmMessage>(cleandJsonString);
                    ProcessAlarmMessage(alarmMessage);
                    message.Complete();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("****  Exception=" + ex.Message);
                }


            });
            Console.ReadLine();
        }
        private static void ProcessAlarmMessage(ServiceBusApp.AlarmMessage alarmMessage)
        {

            switch (alarmMessage.open)
            {
                case "1":
                    ActionOpenDoor(alarmMessage);
                    break;
                case "0":
                    ActionRepair(alarmMessage);
                    break;
                default:
                    Console.WriteLine("AlarmType is Not accepted!");
                    break;
            }
        }

        private static void ActionRepair(ServiceBusApp.AlarmMessage alarmMessage)
        {
            //if (alarmMessage.ioTHubDeviceID.Equals(DEVICEID))
            if (alarmMessage.deviceId.Equals(DEVICEID))
                ActionRepairWindows(alarmMessage);
        }

        private static void ActionRepairWindows(ServiceBusApp.AlarmMessage alarmMessage)
        {
            //DateTime date1 = Convert.ToDateTime(alarmMessage.createdAt);
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            //DateTime date1 = DateTime.Parse(alarmMessage.createdAt, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime date1 = DateTime.Parse(alarmMessage.time, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime date2 = DateTime.UtcNow;
            TimeSpan diff = date2.Subtract(date1.AddHours(-16));
            /* if (diff.TotalSeconds > 20)
             {
                 //WriteHighlightedMessage("Message Expired(Open door), " + "Diff=" + diff.TotalSeconds.ToString(), ConsoleColor.Blue);
                 ignore_messages.Add(GetDeviceIdHint(alarmMessage.deviceId) +
                     " OpenDoor! Certificate=" + alarmMessage.name +
                     ", MessageID=" + alarmMessage.msgId +
                     ", Diff=" + diff.TotalSeconds.ToString());
                 open_ignore_count += 1;
                 return;
             }*/
            WriteHighlightedMessage(
                    GetDeviceIdHint(alarmMessage.deviceId) +
                    " CloseDoor! Certificate=" + alarmMessage.name +
                    ", MessageID=" + alarmMessage.msgId +
                    ", Diff=" + diff.TotalSeconds.ToString(),
                    ConsoleColor.Blue);

            /*C2DCommand c2dCommand = new C2DCommand();
            c2dCommand.command = C2DCommand.COMMAND_OPEN_DOOR_WARNING;
            c2dCommand.value = alarmMessage.msgId;
            c2dCommand.time = alarmMessage.time;
            SendCloudToDeviceCommand(_serviceClient, alarmMessage.deviceId, c2dCommand).Wait();*/

            C2DCommand2 c2dCommand = new C2DCommand2();
            c2dCommand.name = alarmMessage.name;
            c2dCommand.Lock = false;
            color c = new color(alarmMessage);
            c2dCommand.R = c.R;
            c2dCommand.G = c.G;
            c2dCommand.B = c.B;
            SendCloudToDeviceCommand(_serviceClient, "Devices", c2dCommand).Wait();


            // using (var db = new HumanContext())
            //{
            // Create and save a new Blog 

            /*var entity = new Dbmsg
            {
               // name = alarmMessage.name,
                deviceId = alarmMessage.deviceId,
                msgId = alarmMessage.msgId,
                open = alarmMessage.open,
                age = alarmMessage.age,
                gender = alarmMessage.gender,
                emotion = alarmMessage.emotion,
                angerScore = alarmMessage.angerScore,
                happyScore = alarmMessage.happyScore,
                neutralScore = alarmMessage.neutralScore,
                contemptScore = alarmMessage.contemptScore,
                disgustScore = alarmMessage.disgustScore,
                fearScore = alarmMessage.fearScore,
                sadScore = alarmMessage.sadScore,
                surpriseScore = alarmMessage.surpriseScore,
                CreatedAt = DateTime.UtcNow.AddHours(8)
                //time = alarmMessage.time
            };
            db.Data.Add(entity);
            db.SaveChanges();*/

            // }
        }
        private static void ActionOpenDoor(ServiceBusApp.AlarmMessage alarmMessage)
        {
            //if (alarmMessage.ioTHubDeviceID.Equals(DEVICEID))
            if (alarmMessage.deviceId.Equals(DEVICEID))
                ActionOpenDoorWindows(alarmMessage);
        }
        private static void ActionOpenDoorWindows(ServiceBusApp.AlarmMessage alarmMessage)
        {
            //DateTime date1 = Convert.ToDateTime(alarmMessage.createdAt);
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            //DateTime date1 = DateTime.Parse(alarmMessage.createdAt, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime date1 = DateTime.Parse(alarmMessage.time, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            DateTime date2 = DateTime.UtcNow;
            TimeSpan diff = date2.Subtract(date1.AddHours(-16));
            /* if (diff.TotalSeconds > 20)
             {
                 //WriteHighlightedMessage("Message Expired(Open door), " + "Diff=" + diff.TotalSeconds.ToString(), ConsoleColor.Blue);
                 ignore_messages.Add(GetDeviceIdHint(alarmMessage.deviceId) +
                     " OpenDoor! Certificate=" + alarmMessage.name +
                     ", MessageID=" + alarmMessage.msgId +
                     ", Diff=" + diff.TotalSeconds.ToString());
                 open_ignore_count += 1;
                 return;
             }*/
            WriteHighlightedMessage(
                    GetDeviceIdHint(alarmMessage.deviceId) +
                    " OpenDoor! Certificate=" + alarmMessage.name +
                    ", MessageID=" + alarmMessage.msgId +
                    ", Diff=" + diff.TotalSeconds.ToString(),
                    ConsoleColor.Yellow);

            /*C2DCommand c2dCommand = new C2DCommand();
            c2dCommand.command = C2DCommand.COMMAND_OPEN_DOOR_WARNING;
            c2dCommand.value = alarmMessage.msgId;
            c2dCommand.time = alarmMessage.time;
            SendCloudToDeviceCommand(_serviceClient, alarmMessage.deviceId, c2dCommand).Wait();*/

            C2DCommand2 c2dCommand = new C2DCommand2();
            c2dCommand.name = alarmMessage.name;
            c2dCommand.Lock = true;
            color c = new color(alarmMessage);
            c2dCommand.R = c.R;
            c2dCommand.G = c.G;
            c2dCommand.B = c.B;
            SendCloudToDeviceCommand(_serviceClient, "Devices", c2dCommand).Wait();
            
            Console.WriteLine("Name: "+c2dCommand.name+" Lock: "+c2dCommand.Lock+" R: "+c2dCommand.R+" G: "+c2dCommand.G+" B: "+c2dCommand.B);

            // using (var db = new HumanContext())
            //{
            // Create and save a new Blog 

            /*var entity = new Dbmsg
            {
               // name = alarmMessage.name,
                deviceId = alarmMessage.deviceId,
                msgId = alarmMessage.msgId,
                open = alarmMessage.open,
                age = alarmMessage.age,
                gender = alarmMessage.gender,
                emotion = alarmMessage.emotion,
                angerScore = alarmMessage.angerScore,
                happyScore = alarmMessage.happyScore,
                neutralScore = alarmMessage.neutralScore,
                contemptScore = alarmMessage.contemptScore,
                disgustScore = alarmMessage.disgustScore,
                fearScore = alarmMessage.fearScore,
                sadScore = alarmMessage.sadScore,
                surpriseScore = alarmMessage.surpriseScore,
                CreatedAt = DateTime.UtcNow.AddHours(8)
                //time = alarmMessage.time
            };
            db.Data.Add(entity);
            db.SaveChanges();*/

            // }
        }

        private async static Task SendCloudToDeviceCommand(ServiceClient serviceClient, String deviceId, C2DCommand2 command)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(command)));
            await serviceClient.SendAsync(deviceId, commandMessage);
        }

        private static void WriteHighlightedMessage(string message, System.ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static string GetDeviceIdHint(string ioTHubDeviceID)
        {
            return "[" + ioTHubDeviceID + " (" + DateTime.UtcNow.ToString("MM-ddTHH:mm:ss") + ")" + "]";
        }




    }
    class color
    {
        public color(ServiceBusApp.AlarmMessage alarmMessage)
        {
            string emotion = alarmMessage.emotion;
            switch (emotion)
            {
                case "Anger"://紅色
                    R = 255;
                    G = 0;
                    B = 0;
                    break;
                case "Contempt":
                    R = 0;
                    G = 255;
                    B = 255;
                    break;
                    R = 64;
                    G = 64;
                    B = 64;
                    break;
                case "Fear":
                    R = 224;
                    G = 0;
                    B = 112;
                    break;
                case "Happiness"://紫色
                    R = 225;
                    G = 0;
                    B = 255;
                    break;
                case "Neutral"://綠色
                    R = 0;
                    G = 255;
                    B = 0;
                    break;
                case "Sadness"://dark blue
                    R = 0;
                    G = 120;
                    B = 255;
                    break;
                case "Surprise"://咖啡色
                    R = 255;
                    G = 255;
                    B = 0;
                    break;
                default:
                    Console.WriteLine("Emotion Type is Not accpeted!");
                    break;
            }

        }
        public int R;
        public int G;
        public int B;

    }
}
