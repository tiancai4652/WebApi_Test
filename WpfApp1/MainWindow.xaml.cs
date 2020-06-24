using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

      
        async void Init()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://192.168.0.115:7788")
            };

            HubConnection hubConnection = new HubConnectionBuilder()
              .WithUrl("http://192.168.0.115:7788/myhub")
              .Build();



            await hubConnection.StartAsync();

            hubConnection.SendAsync("Register", "客户端").Wait();
        }
    }
}
