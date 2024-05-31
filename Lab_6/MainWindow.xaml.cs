using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MunicipalManagement
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Project> Projects;

        public MainWindow()
        {
            Projects = new ObservableCollection<Project>();
            InitializeComponent();
            lBox.DataContext = Projects;
        }

        async Task FillDataAsync()
        {
            var projects = await Project.GetAllProjectsAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Projects.Clear();
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }
            });
        }

        async Task PopulateInitialDataIfEmptyAsync()
        {
            var projects = await Project.GetAllProjectsAsync();
            if (projects.Any())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("База данных уже содержит данные. Операция заполнения дефолтными значениями пропущена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return;
            }

            var defaultProjects = new[]
            {
                new Project
                {
                    ProjectName = "Road Construction",
                    Budget = 1500000,
                    StartDate = new DateTime(2023, 6, 1),
                    EndDate = new DateTime(2024, 6, 1)
                },
                new Project
                {
                    ProjectName = "Park Renovation",
                    Budget = 500000,
                    StartDate = new DateTime(2023, 8, 15),
                    EndDate = new DateTime(2024, 4, 30)
                },
                new Project
                {
                    ProjectName = "New Library",
                    Budget = 750000,
                    StartDate = new DateTime(2023, 9, 1),
                    EndDate = new DateTime(2025, 12, 31)
                },
                new Project
                {
                    ProjectName = "Public Transport Upgrade",
                    Budget = 1200000,
                    StartDate = new DateTime(2023, 10, 1),
                    EndDate = null // Проект без указанной даты окончания
                }
            };

            foreach (var project in defaultProjects)
            {
                await project.InsertAsync();
            }

            await FillDataAsync(); // Обновляем список проектов после добавления данных
        }

        private async void btnFill_Click(object sender, RoutedEventArgs e)
        {
            await PopulateInitialDataIfEmptyAsync();
            await FillDataAsync();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var project = new Project()
            {
                ProjectName = "Sample Project",
                Budget = 1000000,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };
            await project.InsertAsync();
            await FillDataAsync();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var project = (Project)lBox.SelectedItem;

            if (project == null)
            {
                MessageBox.Show("Пожалуйста, выберите проект для изменения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            project.ProjectName = "Updated Project";
            await project.UpdateAsync();
            await FillDataAsync();
        }

        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var project = (Project)lBox.SelectedItem;

            if (project == null)
            {
                MessageBox.Show("Пожалуйста, выберите проект для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await Project.DeleteAsync(project.ProjectId);
            await FillDataAsync();
        }
    }
}
