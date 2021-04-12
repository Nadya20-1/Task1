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

namespace Task1.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
    
        private static PeopleContext DataBase;

        public ApplicationViewModel()
        {
          
             
        }

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

        public static void Text()
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ShowDialog();
                dialog.Filter = "Csv file (*.csv)|*.csv";
                MainWindow.Instance.file_text.Text = dialog.FileName;

                OpenFile.readCSVandSave2DB(dialog.FileName);
               
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

        public RelayCommand SelectFile => new RelayCommand(obj => Text());

        public RelayCommand LoadToDatabase => new RelayCommand(obj => SendDataToSQL());

        public RelayCommand ExportToExcel => new RelayCommand(obj => Excel());

        public RelayCommand ExportToXML => new RelayCommand(obj => XML());

 
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
