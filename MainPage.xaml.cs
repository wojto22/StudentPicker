using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;

namespace StudentPicker
{
    public partial class MainPage : ContentPage
    {
        private string classesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Classes");
        public ObservableCollection<StudentPickerPage> Classes = new ObservableCollection<StudentPickerPage>();
        public MainPage()
        {
            InitializeComponent();
            classes.ItemsSource = Classes;

            if (!Directory.Exists(classesFolderPath))
                Directory.CreateDirectory(classesFolderPath);

            Debug.WriteLine(classesFolderPath);

            LoadClasses();
        }

        private async void AddClass(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Tworzenie", "Podaj nazwę nowej klasy", "Stwórz", "Anuluj");
            if (string.IsNullOrWhiteSpace(name) || name == "Anuluj")
                return;

            StudentPickerPage newClass = new StudentPickerPage()
            {
                Name = name
            };

            SaveCollectionFile(newClass);
            Classes.Add(newClass);
            await Navigation.PushAsync(newClass);
        }
        private void DeleteClass(object sender, EventArgs e)
        {
            StudentPickerPage selectedClass = classes.SelectedItem as StudentPickerPage;

            if (selectedClass == null)
                return;

            DeleteClassFile(selectedClass);
            Classes.Remove(selectedClass);
        }

        private void SaveCollectionFile(StudentPickerPage Class)
        {
            string filePath = Path.Combine(classesFolderPath, $"{Class.Name}.txt");
            try
            {
                File.Create(filePath).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w trackie zapisywania pliku: {ex.Message}");
            }
        }

        private void DeleteClassFile(StudentPickerPage Class)
        {
            string filePath = Path.Combine(classesFolderPath, $"{Class.Name}.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd w trakcie usuwania pliku: {ex.Message}");
                }
            }
        }

        private async void GoToClass(object sender, EventArgs e)
        {
            if (classes.SelectedItem == null)
                return;
            await Navigation.PushAsync(new StudentPickerPage() { Name = (classes.SelectedItem as StudentPickerPage).Name });
        }

        private void LoadClasses()
        {
            var files = Directory.GetFiles(classesFolderPath);

            foreach (string file in files)
            {
                var fileName = Path.GetFileName(file).Split(".")[0];

                StudentPickerPage Class = new StudentPickerPage() { Name = fileName };

                Classes.Add(Class);
            };
        }
    }

}
