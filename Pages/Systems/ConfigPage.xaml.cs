using DeviceControlPanel.Classes;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DeviceControlPanel.Pages.Systems
{
    /// <summary>
    /// Interaction logic for ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : Page
    {
        public ConfigPage()
        {
            InitializeComponent();
            InitializeFormats();
            LoadNetworkInformation();
        }
        public List<string> DateFormats = new List<string>();
        public List<string> TimeFormats = new List<string>();
        public List<string> DurationFormats = new List<string>();
        public void InitializeFormats()
        {
            DateFormats.Add("MM/dd/yyyy");
            DateFormats.Add("MM-dd-yyyy");
            DateFormats.Add("MM.dd-yyyy");
            DateFormats.Add("dd/MM/yyyy");
            DateFormats.Add("dd-MM-yyyy");
            DateFormats.Add("dd.MM-yyyy");
            DateFormats.Add("yyyy/MM/dd");
            DateFormats.Add("yyyy-MM-dd");
            DateFormats.Add("yyyy.MM.dd");
            DateFormats.Add("yyyy/dd/MM");
            DateFormats.Add("yyyy-dd-MM");
            DateFormats.Add("yyyy.dd.MM");

            TimeFormats.Add("HH:mm");
            TimeFormats.Add("HH:mm:ss");
            TimeFormats.Add("hh:mm tt");

            DurationFormats.Add("mm");
            DurationFormats.Add("HH:mm");
            DurationFormats.Add("HH.mm");
            DurationFormats.Add("HH/mm");
            DurationFormats.Add("d HH:mm");
            DurationFormats.Add("d HH.mm");
            DurationFormats.Add("d HH/mm");


            cmbDateFormat.ItemsSource = DateFormats;
            cmbTimeFormat.ItemsSource = TimeFormats;
            cmbDurationFormat.ItemsSource = DurationFormats;
        }

        private DispatcherTimer timer = new DispatcherTimer();
        private void CheckConnection()
        {
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            StartBackgroundThread();
        }

        private NetworkClass network = new NetworkClass();
        private Dispatcher uiDispatcher = Dispatcher.CurrentDispatcher;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private void StartBackgroundThread()
        {
            Task.Run(() => BackgroundThreadMethod(cancellationTokenSource.Token));
        }

        // Background thread method
        private async void BackgroundThreadMethod(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Perform the networking pinger operation asynchronously
                    await Task.Run(async () =>
                    {
                        // Update UI controls if needed
                        await uiDispatcher.InvokeAsync(async () =>
                        {
                            bool isOnline = await network.PingHostAsync(cmbGateway.SelectedItem.ToString());
                            if (isOnline)
                            {
                                txtNetStatus.Text = "CONNECTED";
                                txtNetStatus.Foreground = Brushes.Green;
                            }
                            else
                            {
                                txtNetStatus.Text = "DISCONNECTED";
                                txtNetStatus.Foreground = Brushes.Red;
                            }
                        });
                    });

                    // Delay for 5 seconds before the next iteration
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, so exit the method gracefully
            }
        }

        // Stop the background thread
        private void StopBackgroundThread()
        {
            cancellationTokenSource.Cancel();
        }


        private async Task LoadNetworkInformation()
        {
            await Task.Run(() =>
            {

                txtNetStatus.Dispatcher.Invoke(async () =>
                {
                    cmbMAC.ItemsSource = await network.GetAllMACAddresses();
                    cmbMAC.SelectedIndex = 0;
                    cmbIPv4.ItemsSource = await network.GetAllIPv4Addresses();
                    cmbIPv4.SelectedIndex = 0;
                    cmbGateway.ItemsSource = await network.GetAllGateways();
                    cmbGateway.SelectedIndex = 0;
                    cmbDNS.ItemsSource = await network.GetAllDNS();
                    cmbDNS.SelectedIndex = 0;
                    CheckConnection();
                });
            });
        }
    }
}
