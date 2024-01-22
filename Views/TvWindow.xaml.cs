using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SmartHome.Views
{
    /// <summary>
    /// Логика взаимодействия для TvWindow.xaml
    /// </summary>
    public partial class TvWindow :Window
    {
        class Item
        {
            public bool Code { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        private readonly string baseUrl;

        public Tv TvInfo { get; set; }

        public TvWindow (string baseUrl)
        {
            this.baseUrl = baseUrl;
            InitializeComponent();

            statusComboBox.ItemsSource = new Item[]
            {
            new Item { Code = false, Description = "Выключен" },
            new Item { Code = true, Description = "Включен" },
            };
            statusComboBox.DisplayMemberPath = "Description";
            statusComboBox.SelectedIndex = 0;

            timer.Start();

            timer.Tick += (sender, e) =>
            {
                loadTvStatus(sender!, null!);
            };
        }

        private async void updateChannel (object sender, RoutedEventArgs e)
        {
            if (channelTextBox.Text != null)
            {
                int id = int.Parse(channelTextBox.Text);

                Tv? tv = await loadTvAsync();
                bool power = tv.On;

                if (!power)
                {
                    MessageBox.Show("Телевизор выключен", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (0 > id && id > 250)
                {
                    MessageBox.Show("Допустимый диапозон каналов от 0 до 250", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (HttpClient client = new())
                {
                    var response = await client.GetAsync(baseUrl + $"Room/TV/Channel/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadTvStatus(sender!, null!);

                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadTvStatus(sender!, null!);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadTvStatus(sender!, null!);
                    }
                }
            }
            else
            {
                MessageBox.Show("Поле канала не может быть пустым", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void updateVolume (object sender, RoutedEventArgs e)
        {
            if (volumeTextBox.Text != null)
            {
                int id = int.Parse(volumeTextBox.Text);

                Tv? tv = await loadTvAsync();
                bool power = tv.On;

                if (!power)
                {
                    MessageBox.Show("Телевизор выключен", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (0 > id && id > 100)
                {
                    MessageBox.Show("Допустимые значения громкости от 0 до 100", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (HttpClient client = new())
                {
                    var response = await client.GetAsync(baseUrl + $"Room/TV/Volume/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadTvStatus(sender!, null!);

                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadTvStatus(sender!, null!);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        loadTvStatus(sender!, null!);
                    }
                }
            }
            else
            {
                MessageBox.Show("Поле громкост не может быть пустым", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            string status;
            if (item.Code == false)
            {
                status = "Off";
            }
            else
            {
                status = "On";
            }

            using (HttpClient client = new())
            {
                var response = await client.PostAsync(baseUrl + $"Room/TV/{status}", null);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Обновлено.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadTvStatus(sender!, null!);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Не удалось обновить. Передано некорректное значение", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadTvStatus(sender!, null!);
                }
                else
                {
                    MessageBox.Show("Не удалось обновить", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    loadTvStatus(sender!, null!);
                }
            }
        }

        private async void loadTvStatus (object sender, RoutedEventArgs e)
        {
            Tv? tv = await loadTvAsync();
            if (tv is not null)
            {
                TvInfo = tv;
                DataContext = null;
                DataContext = TvInfo;
            }
        }

        private async Task<Tv?> loadTvAsync ()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<Tv>(baseUrl + "Room/TV");
            }
        }

        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        private void numericValidationTextBox (object sender, TextCompositionEventArgs e)
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
