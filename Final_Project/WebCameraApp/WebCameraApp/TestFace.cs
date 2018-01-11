using Microsoft.Azure.Devices.Client;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Face;

namespace WebCameraApp
{


    class TestFace
    {
        
        public static readonly string deviceConnectionString = "HostName=IoThub0104.azure-devices.net;DeviceId=Monitor;SharedAccessKey=1b7aZL8Yva04kMyu9ATCN8MxrzUiDZR2D9BvgheRHA8=";
        const string uriBase = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect";
        const string subscriptionKey = "a21ef68c0ee045019bdd4ae85adf8ff6";
        string personGroupId = "family";
        /// <summary>
        /// This is testting Face module
        /// </summary>
        /// 

        private readonly IFaceServiceClient faceServiceClient =
                  new FaceServiceClient("a21ef68c0ee045019bdd4ae85adf8ff6", "https://eastasia.api.cognitive.microsoft.com/face/v1.0");


        public async Task<string> MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceRectangle=false&returnFaceAttributes=age,gender,smile,glasses,emotion";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;
            string contentString;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);
                
                // Get the JSON response.
                contentString = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(contentString);

                //FaceJsonString fc =  JsonConvert.DeserializeObject<JObject>(contentString);
                //var item = fc.faceResult;
                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");

                //Console.WriteLine(o.);
                foreach (var pair in o)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
            }
            return JsonPrettyPrint(contentString);
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
        public async Task<String> Testpicture(String testImageFile)
        {

            try
            {
                People people = new People();
                List<string> rslist = new List<string>();
                string[] HeadRandom;
                StringBuilder Mount_path = new StringBuilder();           
                //FaceServiceClient fc = new FaceServiceClient(ApiKey);

                    using (Stream s = File.OpenRead(testImageFile))
                    {


                        var requiredFaceAttributes = new FaceAttributeType[]
                         {
                            FaceAttributeType.Age,
                            FaceAttributeType.Gender,
                            FaceAttributeType.Smile,
                            FaceAttributeType.FacialHair,
                            FaceAttributeType.HeadPose,
                            FaceAttributeType.Glasses,
                            FaceAttributeType.Emotion
                         };

                        //var faces = faceServiceClient.DetectAsync(s).Result;
                        //Console.WriteLine(faces);
                        var faces = await faceServiceClient.DetectAsync(s, returnFaceLandmarks: true, returnFaceAttributes: requiredFaceAttributes);
                        var faceIds = faces.Select(face => face.FaceId).ToArray();
                        var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);   
                        try
                        {
                            int isM = 0, isF = 0;
                            // string age = "";
                            string sex = "";
                            int age;
                            String age_s = "";
                            String emr = "";
                            String Top_Emotion = "";
                            Dictionary<string, float> Emotion = new Dictionary<string, float>();
                            foreach (var face in faces)
                            {
                                var faceRect = face.FaceRectangle;
                                var attributes = face.FaceAttributes;
                                float Happiness = attributes.Emotion.Happiness;
                                float Anger = attributes.Emotion.Anger;
                                float Neutral = attributes.Emotion.Neutral;
                                float Contempt = attributes.Emotion.Contempt;
                                float Disgust = attributes.Emotion.Disgust;
                                float Fear = attributes.Emotion.Fear;
                                float Sadness = attributes.Emotion.Sadness;
                                float Surprise = attributes.Emotion.Surprise;
                                String[] Emotion_string = { "Anger", "Happiness", "Neutral", "Contempt", "Disgust", "Fear", "Sadness", "Surprise" };
                                float[] Emotion_array = { Anger, Happiness, Neutral, Contempt, Disgust, Fear, Sadness, Surprise };

                                // g.DrawEllipse(new Pen(Brushes.Blue, 5), new System.Drawing.Rectangle(faceRect.Left-90, faceRect.Top-90,
                                //   faceRect.Width+150, faceRect.Height+150));
                                /* g.DrawRectangle(
                                 new Pen(Brushes.Red, 3),
                                 new System.Drawing.Rectangle(faceRect.Left, faceRect.Top,
                                     faceRect.Width, faceRect.Height));*/
                                //g.DrawString(new Font(attributes.Gender.ToString(),));
                                for (int i = 0; i < Emotion_string.Length; i++)
                                {
                                    Emotion.Add(Emotion_string[i], Emotion_array[i]);
                                }

                                if (attributes.Gender.StartsWith("male"))
                                    isM += 1;
                                else
                                    isF += 1;


                                age = Convert.ToInt32(attributes.Age);
                                age_s = age.ToString();
                                sex = attributes.Gender.ToString();

                                Top_Emotion = GetEmotion(attributes.Emotion);
                                Console.WriteLine("Emotion: " + Top_Emotion);
                                Console.WriteLine("Age: " + age_s);
                                Console.WriteLine("Female: " + isF);
                                Console.WriteLine("Male: " + isM);
                                //String name = "";
                                //people.Name = name;

                            }
                        String name = "";
                        foreach (var identifyResult in results)
                        {
                            // Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                            if (identifyResult.Candidates.Length == 0)
                            {
                                Console.WriteLine("No one identified");
                                name = "None";
                            }
                            else if (identifyResult.Candidates.Length != 0)
                            {
                                var candidateId = identifyResult.Candidates[0].PersonId;
                                var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                                Console.WriteLine("Identified as {0}", person.Name);
                                name = person.Name;

                            }
                        }
                        people.Name = name;
                        people.Age = age_s;
                        people.Gender = sex;
                        people.Emotion = Top_Emotion;
                        people.Emotionlistscore = Emotion;

                        SendMessageToCloud sc = new SendMessageToCloud();
                        sc.sendWindTurbineMessageToCloudAsync(people.Name, people.Age, people.Gender, people.Emotion, deviceConnectionString);

                            s.Close();
                            Emotion.Clear();
                            return "OK";
                        }
                        catch (FaceAPIException fs)
                        {
                            Console.WriteLine(fs.ToString());
                            return null;
                        }
                    
                }
               
            }
            catch (Exception e)
            {
                String msg = "Oops! Something went wrong. Try again later";
                if (e is ClientException && (e as ClientException).Error.Message.ToLowerInvariant().Contains("access denied"))
                {
                    msg += " (access denied - hint: check your APIKEY ).";
                    Console.Write(msg);
                }
                Console.Write(e.ToString());
                return null;

            }
        }
       private string GetEmotion(Microsoft.ProjectOxford.Common.Contract.EmotionScores emotion)
        {
            string emotionType = string.Empty;
            double emotionValue = 0.0;
            if (emotion.Anger > emotionValue)
            {
                emotionValue = emotion.Anger;
                emotionType = "Anger";
            }
            if (emotion.Contempt > emotionValue)
            {
                emotionValue = emotion.Contempt;
                emotionType = "Contempt";
            }
            if (emotion.Disgust > emotionValue)
            {
                emotionValue = emotion.Disgust;
                emotionType = "Disgust";
            }
            if (emotion.Fear > emotionValue)
            {
                emotionValue = emotion.Fear;
                emotionType = "Fear";
            }
            if (emotion.Happiness > emotionValue)
            {
                emotionValue = emotion.Happiness;
                emotionType = "Happiness";
            }
            if (emotion.Neutral > emotionValue)
            {
                emotionValue = emotion.Neutral;
                emotionType = "Neutral";
            }
            if (emotion.Sadness > emotionValue)
            {
                emotionValue = emotion.Sadness;
                emotionType = "Sadness";
            }
            if (emotion.Surprise > emotionValue)
            {
                emotionValue = emotion.Surprise;
                emotionType = "Surprise";
            }
            return emotionType;
        }
     }
}
