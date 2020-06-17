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
using Utility.Standard;
using WebApi_Client.Models;

namespace WebApi_Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Device Device = new Device();
        int count;
        public MainWindow()
        {
            InitializeComponent();
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

            txtShow.AppendText(
                $"New Instance:" +
                $"\tDevice.ID:{Device.ID}" +
                 $"\tDevice.Name:{Device.Name}" +$"\tDevice.Data:..."
                );
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
