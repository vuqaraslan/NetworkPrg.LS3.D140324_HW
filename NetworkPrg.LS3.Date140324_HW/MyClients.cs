using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NetworkPrg.LS3.Date140324_HW
{
    public class MyClients : INotifyPropertyChanged
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

        public string ConnectedClientPort { get; set; }
        //public BitmapImage ImageClientSended { get; set; }

        private BitmapImage imageSended;
        public BitmapImage ImageSended
        {
            get { return imageSended; }
            set { imageSended = value; OnPropertyChanged(); }
        }

    }
}
