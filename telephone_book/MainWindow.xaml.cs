using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;


namespace telephone_book
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Contact> contacts;
        private ICollectionView contactsView;
        public MainWindow()
        {
            InitializeComponent();

            contacts = new ObservableCollection<Contact>();
            contactsView = CollectionViewSource.GetDefaultView(contacts);
            dgContacts.ItemsSource = contactsView;
            LoadContactsFromFile(); 
        }

        private void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e) // мнтод на проверку, если это не буквы, то добавляем символ в текстбокс
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text) // метод на проверку, является ли символ числом
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string phoneNumber = txtPhoneNumber.Text;

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) &&
                !string.IsNullOrEmpty(phoneNumber))
            {
                contacts.Add(new Contact { FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber });

                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtPhoneNumber.Text = "";
                SaveContactsToFile();
            }
        }

        private void SaveContactsToFile() // сохранение контактов
        {
            string filePath = "contacts.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Contact contact in contacts)
                {
                    writer.WriteLine($"{contact.FirstName},{contact.LastName},{contact.PhoneNumber}");
                }
            }
        }

        private void LoadContactsFromFile() // загрузка контактов
        {
            string filePath = "contacts.txt";

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] contactData = line.Split(',');
                        if (contactData.Length == 3)
                        {
                            contacts.Add(new Contact { FirstName = contactData[0], LastName = contactData[1], PhoneNumber = contactData[2] });
                        }
                    }
                }
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e) // сброс
        {
            txtSearch.Text = "";
            contactsView.Filter = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e) // удаление
        {
            if (dgContacts.SelectedItem != null)
            {
                contacts.Remove((Contact)dgContacts.SelectedItem);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e) // поиск
        {
            string searchQuery = txtSearch.Text;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                contactsView.Filter = (obj) =>
                {
                    Contact contact = obj as Contact;
                    return contact.FirstName.ToLower().Contains(searchQuery.ToLower()) ||
                           contact.LastName.ToLower().Contains(searchQuery.ToLower()) ||
                           contact.PhoneNumber.ToLower().Contains(searchQuery.ToLower());
                };
            }
            else
            {
                contactsView.Filter = null;
            }
        }

        

    }

    public class Contact : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private string phoneNumber;

        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
