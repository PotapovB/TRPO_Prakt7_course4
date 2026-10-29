using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Reflection.Metadata.BlobBuilder;
using Task = TRPO_Prakt7.Models.Task;

namespace TRPO_Prakt7.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage :Page
    {
        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
        public int Id { get; set; }

        private HttpClient client = new();

        public MainPage()
        {
            client.BaseAddress = new Uri("https://localhost:7060/");
            InitializeComponent();
        }

        private async void GetAllButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateList();

        }

        private async void UpdateList()
        {
            var builder = new UriBuilder(client.BaseAddress);
            builder.Path = "/api/v1/Task";

            var query = HttpUtility.ParseQueryString(builder.Query);
            if (!String.IsNullOrWhiteSpace(Priority))
                query["priority"] = Priority;
            if ((bool)CompletedFilterCheckbox.IsChecked)
                query["completed"] = IsCompleted.ToString();
            if (query.Count > 0)
                builder.Query = query.ToString();

            try
            {
                var response = await client.GetFromJsonAsync<List<Task>>(builder.ToString());
                if (response != null)
                {
                    Tasks.Clear();
                    foreach (Task book in response)
                    {
                        Tasks.Add(book);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GetByIdButton_Click(object sender, RoutedEventArgs e)
        {
            string path = $"/api/v1/Task/{Id}";
            try
            {
                var response = await client.GetFromJsonAsync<Task>(path);
                if (response != null)
                {
                    Tasks.Clear();
                    Tasks.Add(response);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Task? selectedTask = null;
            if (TaskListView.SelectedItem != null)
            {
                selectedTask = TaskListView.SelectedItem as Task;
            }
            NavigationService.Navigate(new Pages.AddEditPage(selectedTask, client));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem == null)
            {
                MessageBox.Show("Для удаления выберите запись");
                return;
            }
            Task task = TaskListView.SelectedItem as Task;

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"/api/v1/Task/{task.Id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Задача успешно удалена");
                    UpdateList();
                }
                else
                {
                    MessageBox.Show($"Ошибка: {response.StatusCode} \n" +
                        $"{task.Title} {task.Deadline} {task.Priority} {task.Deadline} {task.IsCompleted}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
