using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Task1.ViewModel;

namespace Task1
{
    public class OpenFile
    {
        public static async Task ReadCSVFile(string path)
        {
            Encoding win1251 = Encoding.GetEncoding(1251);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null };
            try
            {
                using var streamReader = new StreamReader(path, win1251);
                using var csv = new CsvReader(streamReader, config);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<PeopleClassMap>();
                var record = csv.GetRecords<People>().ToList();

                await csv.ReadAsync();

                SQLRepository<People> rp = new SQLRepository<People>(new PeopleContext());
                
                    foreach (var item in record)
                    {
                       await rp.AddAsync(item);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
            }
            MessageBox.Show("Data has been imported to Database!");
        }
    }
}
