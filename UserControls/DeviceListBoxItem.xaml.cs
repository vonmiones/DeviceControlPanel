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
using System.Windows.Threading;
using DeviceControlPanel.Classes;
using System.Threading;

namespace DeviceControlPanel.UserControls
{
    /// <summary>
    /// Interaction logic for DeviceListBoxItem.xaml
    /// </summary>
    public partial class DeviceListBoxItem : UserControl
    {
        public DeviceListBoxItem()
        {
            InitializeComponent();
            CheckConnection();
        }
        private DispatcherTimer timer = new DispatcherTimer();
        private void CheckConnection()
        {
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;
            
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
            while (!cancellationToken.IsCancellationRequested)
            {
                // Perform the networking pinger operation asynchronously
                await Task.Run(() =>
                {
                    // Networking pinger code
                    // ...

                    // Update UI controls if needed
                    uiDispatcher.Invoke(() =>
                    {
                        // Update UI controls based on the pinger result
                        // ...
                    });
                });

                // Delay for 5 seconds before the next iteration
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        // Stop the background thread
        private void StopBackgroundThread()
        {
            cancellationTokenSource.Cancel();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {

            await Task.Run(() =>
            {

                txtIP.Dispatcher.Invoke(async () =>
                {
                    bool status = await network.PingHostAsync(txtIP.Text);
                    SolidColorBrush brush;
                    if (status)
                    {
                        Color red = Color.FromArgb(255, 0, 255, 0);
                        brush = new SolidColorBrush(red);
                    }
                    else
                    {
                        Color red = Color.FromArgb(255, 255, 0, 0);
                        brush = new SolidColorBrush(red);
                    }
                    txtIP.Dispatcher.Invoke(() => {
                        statusIndicator.Fill = brush;
                    });
                });
            });
           
            
        }
    }
}
