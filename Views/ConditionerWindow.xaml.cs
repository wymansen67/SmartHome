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
using System.Windows.Threading;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace SmartHome.Views
{
    /// <summary>
    /// Логика взаимодействия для ConditionerWindow.xaml
    /// </summary>
    public partial class ConditionerWindow :Window
    {
        class Item
        {
            public int Code { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        private readonly string baseUrl;

        public Conditioner ConditionerInfo { get; set; }

        public ConditionerWindow (string baseUrl)
        {

            this.baseUrl = baseUrl;
            InitializeComponent();

            statusComboBox.ItemsSource = new Item[]
            {
            new Item { Code = 0, Description = "Выключен" },
            new Item { Code = 1, Description = "Включен" },
            };
            statusComboBox.DisplayMemberPath = "Description";
            statusComboBox.SelectedIndex = 0;

            timer.Start();

            timer.Tick += (sender, e) =>
            {
                loadConditionerStatus(sender!, null!);
            };
        }

        private async void updateTemperature (object sender, RoutedEventArgs e)
        {
            if (temperatureTextBox.Text != null)
            {
                int id = int.Parse(temperatureTextBox.Text);

                Conditioner? conditioner = await loadConitionerAsync();
                bool power = conditioner.On;

                if (!power)
                {
                    MessageBox.Show("Кондиционер выключен", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (15 > id && id > 35)
                {
                    MessageBox.Show("Допустимые температуры кондиционера от 15 до 35 градусов", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (HttpClient client = new())
                {
                    var response = await client.PostAsync(baseUrl + $"Room/Coditioner/SetTemperature/{id}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadConditionerStatus(sender!, null!);

                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadConditionerStatus(sender!, null!);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadConditionerStatus(sender!, null!);
                    }
                }
            }
            else
            {
                MessageBox.Show("Поле температуры не может быть пустым", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void updateStatus (object sender, RoutedEventArgs e)
        {
            var item = (statusComboBox.SelectedItem as Item);

            if (item is null)
            {
                return;
            }

            int id = item.Code;
            MessageBox.Show($"{id}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            using (HttpClient client = new())
            {
                var response = await client.GetAsync(baseUrl + $"Room/Conditioner/Power/{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadConditionerStatus(sender!, null!);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadConditionerStatus(sender!, null!);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadConditionerStatus(sender!, null!);
                }
            }
        }

        private async void loadConditionerStatus (object sender, RoutedEventArgs e)
        {
            Conditioner? conditioner = await loadConitionerAsync();
            if (conditioner is not null)
            {
                ConditionerInfo = conditioner;
                DataContext = null;
                DataContext = ConditionerInfo;
            }
        }

        private async Task<Conditioner?> loadConitionerAsync ()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<Conditioner>(baseUrl + "Room/Conditioner");
            }
        }

        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private void temperatureValidationTextBox (object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Exit (object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
