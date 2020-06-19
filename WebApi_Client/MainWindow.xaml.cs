using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Utility.Standard;
using My.Share.Models;

namespace WebApi_Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static HttpClient client = new HttpClient();
        Device Device = new Device();
        int count;
        public MainWindow()
        {
            InitializeComponent();
            txtURI.Text = "https://localhost:5001/";
            txtRequest.Text = "api/Devices";
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Device.ID = Guid.NewGuid().ToString("N");
            Device.Name = $"Instance{count}";
            Device.Spectrum = new List<Spectrum>();

            Spectrum spectrum = new Spectrum();
            spectrum.ID = Guid.NewGuid().ToString("N");
            spectrum.Name = $"spectrum{count}";
            spectrum.Data = new List<SpectrumData>();

            RSpcFile rSpcFile = new RSpcFile();
            rSpcFile.Open("0.spc");
            float[] xList;
            float[] yList;
            rSpcFile.Read(out xList, out yList);
            rSpcFile.Close();
            var length = xList.Length > yList.Length ? yList.Length : xList.Length;
            for (uint uIndex = 0; uIndex < length; uIndex++)
            {
                spectrum.Data.Add(
                    new SpectrumData()
                    {
                        ID = $"{spectrum.ID}{uIndex}",
                        X = xList[uIndex],
                        Y = yList[uIndex]
                    });
            }

            Device.Spectrum.Add(spectrum);

            AppendText(
                $"New Instance:" +
                $"{Environment.NewLine}Device.ID:{Device.ID}" +
                 $"{Environment.NewLine}Device.Name:{Device.Name}" +$"{Environment.NewLine}Device.Data:..."
                );
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            AppendText("Insert");
            var url = await CreateProductAsync(Device);
            AppendText($"Response Location:{url}");
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            client.BaseAddress = new Uri(txtURI.Text);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            AppendText("Set Base Api");
        }





        async Task<Uri> CreateProductAsync(Device product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"{txtRequest.Text}", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        async Task<Device> GetProductAsync(string path)
        {
            Device product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Device>();
            }
            return product;
        }

         async Task<Device> UpdateProductAsync(Device product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"{txtRequest.Text}/{product.ID}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Device>();
            return product;
        }

         async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"{txtRequest.Text}/{id}");
            return response.StatusCode;
        }

        private void btnFindAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnFindById_Click(object sender, RoutedEventArgs e)
        {
            AppendText($"Find id:{txtID.Text}");
            string path = $"{txtRequest.Text}/{txtID.Text}";
            var device = await GetProductAsync(path);
            Show(device);
        }

        void Show(Device device)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(device);
            AppendText(json);
        }

        void AppendText(string msg)
        {
            txtShow.AppendText($"{Environment.NewLine}{msg}");
        }

        private async void btnRemoveById_Click(object sender, RoutedEventArgs e)
        {
            AppendText($"Remove By Id:{txtID.Text}");
            var result = await DeleteProductAsync(txtID.Text);
            AppendText($"result:{result.ToString()}");
        }
    }
}
