using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace NetworkPrg.LS3.Date140324_HW
{
    //Server Side
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

        ////private string connectedClient;
        ////public string ConnectedClient
        ////{
        ////    get { return connectedClient; }
        ////    set { connectedClient = value; OnPropertyChanged(); }
        ////}

        ////private BitmapImage forCheckDirectConvertImg;
        ////public BitmapImage ForCheckDirectConvertImg
        ////{
        ////    get { return forCheckDirectConvertImg; }
        ////    set { forCheckDirectConvertImg = value; OnPropertyChanged(); }
        ////}

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

        private ObservableCollection<MyClients> allClients;
        public ObservableCollection<MyClients> AllClients
        {
            get { return allClients; }
            set { allClients = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LocalIP = SearchLocalIPAddress();
            var ipAddress = IPAddress.Parse(LocalIP);
            var port = 27001;

            AllClients = new ObservableCollection<MyClients>();
            Task.Run(() =>
            {
                using (Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    EndPoint = new IPEndPoint(ipAddress, port);
                    Socket.Bind(EndPoint);
                    Socket.Listen(10);

                    while (true)
                    {
                        //MessageBox.Show("Server is ready for accepting client !");
                        var client = Socket.Accept();
                        var myClient = new MyClients
                        {
                            ConnectedClientPort = client.RemoteEndPoint.ToString()
                        };
                        this.Dispatcher.Invoke(() =>
                        {
                            AllClients.Add(myClient);
                        });
                        //MessageBox.Show($"Client -> {client.RemoteEndPoint}");

                        Task.Run(() =>
                        {
                            //var connClient = $"CLIENT : {client.RemoteEndPoint} connected";
                            //ConnectedClient = connClient;

                            var length = 0;
                            var bytes = new byte[150000];
                            do
                            {
                                length = client.Receive(bytes/*, bytes.Length, SocketFlags.None*/);

                                this.Dispatcher.Invoke(() =>
                                {
                                    ////Burda hec komputere save-etmedende birbasa alinan sekilin(image-in) byte-larini yigib image-e cevirib tezden
                                    ////hemin UI-elementi olan image-in Source-na bind olunmus BitmapImage-tipinde propertiye menimsedirem.
                                    ////Yene de daha deqiq bilmek istesem bu proyekti Client-hissesi ile birge run edib test etmek olar.
                                    //ForCheckDirectConvertImg = ConvertByteArrayToBitmapImage(bytes);

                                    myClient.ImageSended = ConvertByteArrayToBitmapImage(bytes);
                                    //c.ImageSended = new BitmapImage(new Uri(ScreenFileName));
                                });
                            } while (true);
                        });

                    }
                }
            });
        }

        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("______Error converting byte array to BitmapImage: " + ex.Message);
                return null;
            }
        }

    }

}
