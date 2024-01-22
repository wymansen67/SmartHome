using SmartHome.Models;
using SmartHome.Views;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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

namespace SmartHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :Window
    {
        private readonly string baseUrl;
        public Room RoomInfo { get; set; }

        public MainWindow ()
        {
            InitializeComponent();
            baseUrl = ConfigurationManager.AppSettings["baseUrl"] ?? throw new ApplicationException("Invalid configuration: baseUrl is not specified");

            timer.Start();

            timer.Tick += (sender, e) =>
            {
                loadRoomStatus(sender!, null!);
            };
        }

        private async void loadRoomStatus (object sender, RoutedEventArgs e)
        {
            Room? room = await loadRoomAsync();
            if (room is not null)
            {
                RoomInfo = room;
                // обновим контекст привязки окна
                DataContext = null;
                DataContext = RoomInfo;
            }
        }

        private async Task<Room?> loadRoomAsync ()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<Room>(baseUrl + "Room/Status");
            }
        }

        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private void LightChange (object sender, RoutedEventArgs e)
        {
            var lw = new LightsWindow(baseUrl);
            lw.ShowDialog();
        }

        private void ConditionerChange (object sender, RoutedEventArgs e)
        {
            var cw = new ConditionerWindow(baseUrl);
            cw.ShowDialog();
        }

        private void TvChange (object sender, RoutedEventArgs e)
        {
            var tw = new TvWindow(baseUrl);
            tw.ShowDialog();
        }
    }
}