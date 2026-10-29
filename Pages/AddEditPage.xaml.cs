using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using Task = TRPO_Prakt7.Models.Task;

namespace TRPO_Prakt7.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage :Page
    {
        private bool isEdit = false;
        public Task Task { get; set; }
        private HttpClient client;

        public AddEditPage(Task? selectedTask, HttpClient _client)
        {
            Task = new Task();
            if (selectedTask != null)
            {
                Task.Priority = selectedTask.Priority;
                Task.IsCompleted = selectedTask.IsCompleted;
                Task.Title = selectedTask.Title;
                Task.Description = selectedTask.Description;
                Task.Id = selectedTask.Id;
            }
            isEdit = selectedTask != null;
            
            client = _client;
            InitializeComponent();

            PatchButton.IsEnabled = isEdit;
            PutButton.IsEnabled = isEdit;
        }

        private async void PutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid(Task))
            {
                MessageBox.Show("Замена с помощью Put невозможна так как объект не валиден");
                return;
            }

            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/v1/Task/{Task.Id}", Task);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Книга успешно добавлена");
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.PatchAsJsonAsync($"api/v1/Task/{Task.Id}", Task);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Частичное изменение применено");
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsValid(Task task)
        {
            return !(task == null ||
                String.IsNullOrWhiteSpace(task.Title) ||
                String.IsNullOrWhiteSpace(task.Description) ||
                task.Deadline == null ||
                String.IsNullOrWhiteSpace(task.Priority) ||
                task.IsCompleted == null);
        }

        private async void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid(Task))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Task", Task);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Задача успешно добавлена");
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {response.StatusCode} \n" +
                        $"{Task.Title} {Task.Deadline} {Task.Priority} {Task.Deadline} {Task.IsCompleted}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
