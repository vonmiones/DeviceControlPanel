using DeviceControlPanel.Classes;
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

namespace DeviceControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string folderPath = "assets/backgrounds";
            _backgroundSwitcher = new BackgroundSwitcher(folderPath);
            _backgroundSwitcher.SwitchBackground(gridBackground);

        }

        private void gridHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            if (e.ClickCount == 2)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }
        private BackgroundSwitcher _backgroundSwitcher;
        private void btnBackgroundSwitcher_Click(object sender, RoutedEventArgs e)
        {
            _backgroundSwitcher.SwitchBackground(gridBackground);
        }

        Pages.Systems.ConfigPage configPage = new Pages.Systems.ConfigPage();
        private void btnSystems_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(configPage);
        }
        Pages.Devices.ControlPanel deviceControlPanel = new Pages.Devices.ControlPanel();
        private void btnDevices_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(deviceControlPanel);
        }
    }
}
