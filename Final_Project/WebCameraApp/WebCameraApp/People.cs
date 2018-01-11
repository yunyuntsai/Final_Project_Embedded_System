using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCameraApp
{
    class People
    {
        public String Name { get; set; }
        public String Age { get; set; }
        public String Gender { get; set; }
        public String Emotion { get; set; }
        public Dictionary<string, float> Emotionlistscore { get; set; }
    }
}
