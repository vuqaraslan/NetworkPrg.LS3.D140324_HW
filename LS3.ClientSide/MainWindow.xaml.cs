using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace LS3.ClientSide
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public string LocalIP { get; set; }
        public Socket Socket { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public string SearchLocalIPAddress()
        {
            var hostName = Dns.GetHostName();
            var data = Dns.GetHostByName(hostName);
            var data2 = data.AddressList;
            var localIP = data2.FirstOrDefault().ToString();
            return localIP;
        }
        
        private byte[] dataImage;
        public byte[] DataImage
        {
            get { return dataImage; }
            set { dataImage = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LocalIP = SearchLocalIPAddress();
            var ipAddress = IPAddress.Parse(LocalIP);
            var port = 27001;
            EndPoint = new IPEndPoint(ipAddress, port);
        }

        async Task<Image> TakeScreenshootAsync()
        {
            var width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            var height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }
            return bitmap;
        }
        
        async Task<byte[]> ConvertImageToByte(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Socket.Connect(EndPoint);
                if (Socket.Connected)
                {
                    MessageBox.Show("Connected to the Server !");
                    Task.Run(async () =>
                    {
                        //while (true)
                        //{
                            var screenImage = await TakeScreenshootAsync();
                            var imageBytes = await ConvertImageToByte(screenImage);
                            Socket.Send(imageBytes);
                        //}
                    });
                }
                else
                {
                    MessageBox.Show("Is not connected to the Server !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
