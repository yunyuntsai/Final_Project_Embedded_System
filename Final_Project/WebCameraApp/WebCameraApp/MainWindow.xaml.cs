using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http.Headers;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using WebEye.Controls.Wpf;
using System.Net.Http;
using System.IO;
using System.Globalization;

namespace WebCameraApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
     

        public MainWindow()
        {
            InitializeComponent();

            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            comboBox.ItemsSource = webCameraControl.GetVideoCaptureDevices();

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedItem = comboBox.Items[0];
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            startButton.IsEnabled = e.AddedItems.Count > 0;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var cameraId = (WebCameraId)comboBox.SelectedItem;
            webCameraControl.StartCapture(cameraId);
            Console.WriteLine("Start Camera...");
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("Catch Train Picture");
            //for (int i = 0; i < 3; i++)
            //{
                string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
                //string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string path = @"C:\Users\v-altsai\Pictures\Train";
                string p = System.IO.Path.Combine(path, "detect" + time + ".jpg");
                webCameraControl.GetCurrentImage().Save(p);
                //webCameraControl.GetCurrentImage().Dispose();
           // }
            TrainPicture();
        }

        private void OnImageButtonClick(object sender, RoutedEventArgs e)
        {
            string deviceConnectionString = "HostName=IoThub0104.azure-devices.net;DeviceId=Monitor;SharedAccessKey=1b7aZL8Yva04kMyu9ATCN8MxrzUiDZR2D9BvgheRHA8=";
            var dialog = new SaveFileDialog { Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif" };

            Console.WriteLine("Catch Picture...");

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
            //string myPhotos = Environment.GetFolderPath(Envionment.SpecialFolder.MyPictures);
            string path = @"C:\Users\v-altsai\Pictures\Original";
            string p = System.IO.Path.Combine(path, "detect" + time + ".jpg");
            // if (dialog.ShowDialog() == true)
            // {

            webCameraControl.GetCurrentImage().Save(p);
               
                //SendMessageToCloud sc = new SendMessageToCloud();
                //sc.sendWindTurbineMessageToCloudAsync("16", "female", "happy", deviceConnectionString);
            AnalysisPicture(p);
           // }
        }
        
        public async void AnalysisPicture(string path)
        {
            Console.WriteLine("Analysis Picture..."+path);
            TestFace tf = new TestFace();
           
            Console.WriteLine("Call Face API");
            var s = await tf.Testpicture(path);
            //var s = await tf.MakeAnalysisRequest(path);

            Console.WriteLine(s);         
            People p = new People();
        
        }

        public async void TrainPicture()
        {

            TrainFace tr = new TrainFace();
            string PersonGroupID = "family";

            Console.WriteLine("Enter your name : ");
            string name = Console.ReadLine();

            string path = @"C:\Users\v-altsai\Pictures\Train";

            await tr.trainmodel(PersonGroupID, name, path);


        }


    }
}
