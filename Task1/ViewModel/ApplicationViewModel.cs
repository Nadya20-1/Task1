using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using System.Linq;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using ClosedXML.Excel;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Task1.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
    
        private static PeopleContext database;
       
        public ApplicationViewModel()
        {
            database = new PeopleContext();
            Population = database.DbPeople.Local;
        }

        public static ObservableCollection<People> Population { get; set; }

        private static DateTime date;
        public static string DateText
        {
            get
            {
                if (date != DateTime.MinValue)
                    return date.ToShortDateString();
                return string.Empty;
            }
            set
            {
                date = DateTime.Parse(value);
            }
        }
        public static string FirstNameText { get; set; }
        public static string LastNameText { get; set; }
        public static string MiddleNameText { get; set; }
        public static string CityText { get; set; }
        public static string CountryText { get; set; }

        private static List<People> Parameters()
        {
            List<People> people = Population.ToList();
            if (date != DateTime.MinValue)
            {
                people = people.Where(p => p.Date == date.ToShortDateString()).Select(p => p).ToList();
            }
            if (FirstNameText != string.Empty && FirstNameText != null)
            {
                people = people.Where(p => p.FirstName == FirstNameText).Select(p => p).ToList();
            }
            if (LastNameText != string.Empty && LastNameText != null)
            {
                people = people.Where(p => p.LastName == LastNameText).Select(p => p).ToList();
            }
            if (MiddleNameText != string.Empty && MiddleNameText != null)
            {
                people = people.Where(p => p.MiddleName == MiddleNameText).Select(p => p).ToList();
            }
            if (CityText != string.Empty && CityText != null)
            {
                people = people.Where(p => p.City == CityText).Select(p => p).ToList();
            }
            if (CountryText != string.Empty && CountryText != null)
            {
                people = people.Where(p => p.Country == CountryText).Select(p => p).ToList();
            }
            return people;
        }


        public async static void XML()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Xml file (*.xml)|*.xml";

            if (dialog.ShowDialog() == true)
            {
                await Task.Delay(1000);
                Export.ExportToXML(dialog.FileName);
            }
        }

        public async static void Excel()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm";

            if (dialog.ShowDialog() == true)
            {
                await Task.Delay(1000);
                Export.ExportToExcel(dialog.FileName);
            }
        }

        public static void OpenCSV(string path)
        {
            List<People> a = OpenFile.ReadCSVFile(path)?.ToList();
            if (a != null)
                foreach (var item in a)
                {
                    SQLRepository<People> rp = new SQLRepository<People>(new PeopleContext());
                    rp.Create(item);
                }
            MessageBox.Show("Data has been imported to Database!");
        }

        public static void ReadCSVandSendDataToSQL()
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();
                dialog.Filter = "Csv file (*.csv)|*.csv";

                MainWindow.Instance.file_text.Text = dialog.FileName;

                if (dialog.FileName == "")
                {
                    return;
                }

                 AsyncReadCSVandSave2DB(dialog.FileName);
                //SendDataToSQL();
               
                //MainWindow.Instance.data_view.ItemsSource = OpenFile.ReadCSVFile(dialog.FileName);
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }


        public static void SendDataToSQL()
        {
            try
            {
                DataTable dataTable = ToDataTable(OpenFile.ReadCSVFile(MainWindow.Instance.file_text.Text));

                OpenFile.ReadCSVToDatabase(dataTable);
                MessageBox.Show("Data has been imported to Database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
            }
        }

        public RelayCommand SelectFile => new RelayCommand(obj => ReadCSVandSendDataToSQL());

        public RelayCommand LoadToDatabase => new RelayCommand(obj => SendDataToSQL());

        public RelayCommand ExportToExcel => new RelayCommand(obj => Excel());

        public RelayCommand ExportToXML => new RelayCommand(obj => XML());

        public static async void AsyncReadCSVandSave2DB(string path)
        {
            await Task.Run(() => OpenCSV(path));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
