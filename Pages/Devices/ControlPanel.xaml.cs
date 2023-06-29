using DeviceControlPanel.Classes;
using DeviceControlPanel.Helpers;
using DeviceControlPanel.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceControlPanel.Pages.Devices
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Page
    {
        public ControlPanel()
        {
            InitializeComponent();
            InitializeDeviceDatabase();
            GetDevicesLists();
        }
        private CRUDDevice crudDevice { get; set; }
        private void InitializeDeviceDatabase()
        {
            crudDevice = new CRUDDevice("data/devices.db");
        }

        private Device device = new Device();
        private SDKHelper sdk = new SDKHelper();
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            device.DeviceName = txbName.Text;
            device.Ip = txbIP.Text;
            device.Port = txbPort.Text.ToInt();
            device.Model = txbModel.Text;
            device.Serial = txbSerialNumber.Text;
            device.Version = txbVersion.Text;
            device.Processor = txbProcessor.Text;
            device.Alias = txbBrand.Text;

            crudDevice.Insert(device);
            IsInfoLoaded = false;
            btnSave.IsEnabled = false;
            btnGetDeviceInfo.IsEnabled = true;
            GetDevicesLists();
        }
        public List<Device> deviceList { get; set; }
        private void GetDevicesLists()
        {

            deviceList = crudDevice.GetAll(); // Get the device list from wherever you are fetching it
            if (lbDevices.Items.Count > 0)
            {
                lbDevices.Items.Clear();
            }

            foreach (Device device in deviceList)
            {
                DeviceListBoxItem item = new DeviceListBoxItem();
                item.txtName.Text = device.DeviceName;
                item.txtIP.Text = device.Ip;
                lbDevices.Items.Add(item);
            }

        }

        private bool IsInfoLoaded = false;
        private string sFirmver;
        private string sMacl;
        private string sPlatform;
        private string sSN;
        private string sProductTime;
        private string sDeviceName;
        private int iFPAlg;
        private int iFaceAlg;
        private string sProducter;

        private void btnGetDeviceInfo_Click(object sender, RoutedEventArgs e)
        {
            sdk.sta_DisConnect();
            int isConnected = sdk.sta_ConnectTCP(txbIP.Text, txbPort.Text, "1");
            if (isConnected == 1)
            {
                sdk.sta_GetDeviceInfo(out sFirmver, out sMacl, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);
                txbSerialNumber.Text = sSN.ToUpper();
                txbProcessor.Text = sPlatform;
                txbVersion.Text = sFirmver;
                txbModel.Text = sDeviceName;

                IsInfoLoaded = true;
                btnGetDeviceInfo.IsEnabled = false;
                btnSave.IsEnabled = true;
                sdk.sta_DisConnect();
            }
            else
            {
                System.Windows.MessageBox.Show("Please check connection details.");
            }
        }
        NetworkClass network = new NetworkClass();
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int port = 4370;              // Specify the port to check

            // Scan for connected devices
            List<string> connectedDevices = await network.ScanConnectedDevicesAsync();

            // Check if the port is open for the connected devices
            List<string> devicesWithOpenPort = await network.CheckPortOpenAsync(connectedDevices, port);

            foreach (string device in devicesWithOpenPort)
            {
                Console.WriteLine($"Device found: {device}");
            }
        }

        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            foreach (Device device in deviceList)
            {
                await Task.Run(() =>
                {
                    DateTimePicker dt = new DateTimePicker();
                    Console.WriteLine($"Synchronizing time: {device.Ip}");
                    SDKHelper sdkTime = new SDKHelper();
                    sdkTime.sta_DisConnect();
                    sdkTime.sta_ConnectTCP(device.Ip.Trim(), device.Port.ToString().Trim(), "1");
                    dt.Value = DateTime.Now;
                    sdkTime.sta_SetDeviceTime(dt);
                    sdkTime.sta_DisConnect();
                });
            }

        }

        private async void btnDownloadTransaction_Click(object sender, RoutedEventArgs e)
        {
            foreach (Device device in deviceList)
            {
                Console.WriteLine($"Downloading data at: {device.Ip}");
                await Task.Run(() =>
                {
                    Console.WriteLine($"Data: {device.Ip}");
                    SDKHelper sdkTime = new SDKHelper();
                    sdkTime.sta_DisConnect();
                    sdkTime.sta_ConnectTCP(device.Ip.Trim(), device.Port.ToString().Trim(), "1");
                    sdkTime.sta_readAttLog();
                    sdkTime.sta_DisConnect();
                });
            }
        }
    }
}
