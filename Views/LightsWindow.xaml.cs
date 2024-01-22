using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartHome.Models;
using System.Net.Http.Json;
using System.Windows.Threading;

namespace SmartHome.Views
{
    /// <summary>
    /// Логика взаимодействия для LightsWindow.xaml
    /// </summary>
    public partial class LightsWindow :Window
    {
        class Item
        {
            public int Code { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        private readonly string baseUrl;

        public Light LightInfo { get; set; }

        public LightsWindow (string baseUrl)
        {
            this.baseUrl = baseUrl;
            InitializeComponent();
            statusComboBox.ItemsSource = new Item[]
            {
            new Item { Code = 0, Description = "Выключено" },
            new Item { Code = 1, Description = "Слабое освещение" },
            new Item { Code = 2, Description = "Среднее освещение" },
            new Item { Code = 3, Description = "Полное освещение" },
            };
            statusComboBox.DisplayMemberPath = "Description";
            statusComboBox.SelectedIndex = 0;

            colorComboBox.ItemsSource = new Item[]
            {
            new Item { Code = 0, Description = "Нормальный цвет" },
            new Item { Code = 1, Description = "Тёплый цвет" },
            new Item { Code = 2, Description = "Холодный цвет" },
            };
            colorComboBox.DisplayMemberPath = "Description";
            colorComboBox.SelectedIndex = 0;

            timer.Start();

            timer.Tick += (sender, e) =>
            {
                loadLightStatus(sender!, null!);
            };
        }

        private async void updateStatus (object sender, RoutedEventArgs e)
        {
            var item = (statusComboBox.SelectedItem as Item);

            if (item is null)
            {
                return;
            }

            int id = item.Code;

            using (HttpClient client = new())
            {
                var response = await client.GetAsync(baseUrl + $"Room/Light/PowerOn/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadLightStatus(sender!, null!);

                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadLightStatus(sender!, null!);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadLightStatus(sender!, null!);
                }
            }
        }

        private async void updateColor (object sender, RoutedEventArgs e)
        {
            var item = (colorComboBox.SelectedItem as Item);

            if (item is null)
            {
                return;
            }

            int id = item.Code;

            using (HttpClient client = new())
            {
                var response = await client.GetAsync(baseUrl + $"Room/Light/SetColor/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadLightStatus(sender!, null!);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadLightStatus(sender!, null!);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadLightStatus(sender!, null!);
                }
            }
        }

        private async void loadLightStatus (object sender, RoutedEventArgs e)
        {
            Light? light = await loadLightAsync();
            if (light is not null)
            {
                LightInfo = light;
                DataContext = null;
                DataContext = LightInfo;
            }
        }

        private async Task<Light?> loadLightAsync ()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<Light>(baseUrl + "Room/Light");
            }
        }

        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private void Exit (object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
