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
    }
}
