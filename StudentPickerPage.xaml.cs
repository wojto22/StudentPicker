using System.Collections.ObjectModel;

namespace StudentPicker;

public partial class StudentPickerPage : ContentPage
{
	public string Name { get; set; }
    public ObservableCollection<Student> Students = new ObservableCollection<Student>();

    public StudentPickerPage()
	{
		InitializeComponent();

        students.ItemsSource = Students;
        LoadStudentsFromFile();
    }

    private async void AddStudent(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nowy uczeń", "Podaj imię ucznia", "Dodaj", "Anuluj");
        if (string.IsNullOrWhiteSpace(name) || name == "Anuluj")
            return;

        Student student = new Student { Name = name };
        Students.Add(student);
        SaveStudentsToFile();
    }

    private void DeleteStudent(object sender, EventArgs e)
    {
        if (students.SelectedItem is Student selectedStudent)
        {
            Students.Remove(selectedStudent);
            SaveStudentsToFile();
        }
    }

    private async void EditStudent(object sender, EventArgs e)
    {
        if (students.SelectedItem is Student selectedStudent)
        {
            string newName = await DisplayPromptAsync("Edycja ucznia", "Nowe imię ucznia", initialValue: selectedStudent.Name, accept: "Edytuj", cancel: "Anuluj");
            if (!string.IsNullOrWhiteSpace(newName) && newName != "Anuluj")
            {
                selectedStudent.Name = newName;
                SaveStudentsToFile();
            }
        }
    }

    private void LoadStudentsFromFile()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                Students.Add(new Student { Name = line });
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStudentsFromFile();
    }

    private void SaveStudentsToFile()
    {
        string filePath = GetFilePath();
        File.WriteAllLines(filePath, Students.Select(e => e.Name));
    }

    private string GetFilePath()
    {
        return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Classes"), $"{Name}.txt");
    }

    private async void PickStudent(object sender, EventArgs e)
    {
        if (Students.Count <= 0)
            await DisplayAlert("Błąd w trakcie losowania uczniów", "Klasa nie posiada żadnych uczniów! Dodaj uczniów.", "OK");

        Random rand = new Random();

        int randomIndex = rand.Next(0, Students.Count);
        Student randomStudent = Students[randomIndex];

        await DisplayAlert("Wylosowano Ucznia", $"Wylosowany Uczeń: {randomStudent.Name}", "OK");
    }

    private async void LoadClass(object sender, EventArgs e)
    {
        FileResult file = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Wybierz plik .txt"
        });

        if (file == null || Path.GetExtension(file.FullPath) != ".txt")
        {
            await DisplayAlert("Błąd", "Wystąpił błąd w trakcie wybierania pliku", "OK");
            return;
        }

        string text = File.ReadAllText(file.FullPath);

        string[] lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        Students.Clear();

        foreach(string line in lines)
        {
            Students.Add(new Student() { Name = line });
        }

        await DisplayAlert("Sukces", "Pomyślnie wczytano listę uczniów", "OK");
    }
}