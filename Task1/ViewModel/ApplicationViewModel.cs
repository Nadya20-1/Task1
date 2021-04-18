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

        public static void XML()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Xml file (*.xml)|*.xml";

            if (dialog.ShowDialog() == true)
            {
                Export.ExportToXML(dialog.FileName);
            }
        }

        public static void Excel()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm";

            if (dialog.ShowDialog() == true)
            {
                Export.ExportToExcel(dialog.FileName);
            }
        }

        public static async Task OpenCSVAsync(string path)
        {
            await OpenFile.ReadCSVFile(path);
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
        }

        public RelayCommand SelectFile => new RelayCommand(obj => ReadCSVandSendDataToSQL());

        public RelayCommand ExportToExcel => new RelayCommand(obj => Excel());

        public RelayCommand ExportToXML => new RelayCommand(obj => XML());

        public static async void AsyncReadCSVandSave2DB(string path)
        {
            await Task.Run(() => OpenCSVAsync(path));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
