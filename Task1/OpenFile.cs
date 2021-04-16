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
                csv.Configuration.RegisterClassMap<PeopleClassMap>();
                var record = csv.GetRecords<People>();

                await csv.ReadAsync();

                SQLRepository<People> rp = new SQLRepository<People>(new PeopleContext());
                while (await csv.ReadAsync())
                {
                    foreach (var item in record)
                    {
                       await rp.AddAsync(item);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
            }
            MessageBox.Show("Data has been imported to Database!");
        }

        public static DataTable ReadCSV(string filePath)
        {
            var dataTable = new DataTable();
            using (TextFieldParser csvReader = new TextFieldParser(filePath))
            {
                csvReader.SetDelimiters(new string[] { ";" });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();

                DataColumn datecolumnDate = new DataColumn("Date");
                datecolumnDate.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnDate);
                DataColumn datecolumnName = new DataColumn("FirstName");
                datecolumnName.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnName);
                DataColumn datecolumnSName = new DataColumn("LastName");
                datecolumnSName.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnSName);
                DataColumn datecolumnMName = new DataColumn("MiddleName");
                datecolumnMName.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnMName);
                DataColumn datecolumnCity = new DataColumn("City");
                datecolumnCity.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnCity);
                DataColumn datecolumnCountry = new DataColumn("Country");
                datecolumnCountry.AllowDBNull = true;
                dataTable.Columns.Add(datecolumnCountry);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding win1251 = Encoding.GetEncoding(1251);

            File.ReadLines(filePath, win1251).Skip(1)
                .Select(x => x.Split(';'))
                .ToList()
                .ForEach(line => dataTable.Rows.Add(line));
            return dataTable;
        }

        public static bool ReadCSVandSave2DB(string filePath)
        {
            DataTable csv = new DataTable();

            try
            {
                csv = ReadCSV(filePath);
                if (!ReadCSVToDatabase(csv)) return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
                return false;
            } 
            return true;
        }

        static public bool ReadCSVToDatabase(DataTable csvData)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Population;Integrated Security=True;"))
                {
                    dbConnection.Open();
                    using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                    {
                        s.DestinationTableName = "dbo.People";
                        foreach (var column in csvData.Columns)
                        {
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        }
                        s.WriteToServer(csvData);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
                return false;
            }
            MessageBox.Show("Data has been imported to Database!");
            return true;
        }

        //public static int readCSVandSave2DataBase(string path)
        //{
        //    int csvBufferSize = 9000;
        //    DataTable csvData = new DataTable();
        //    try
        //    {
        //        using (TextFieldParser csvReader = new TextFieldParser(path))
        //        {
        //            csvReader.SetDelimiters(new string[] { ";" });
        //            csvReader.HasFieldsEnclosedInQuotes = true;
        //            string[] colFields = csvReader.ReadFields();

        //            DataColumn datecolumnDate = new DataColumn("Date");
        //            datecolumnDate.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnDate);
        //            DataColumn datecolumnName = new DataColumn("FirstName");
        //            datecolumnName.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnName);
        //            DataColumn datecolumnSName = new DataColumn("LastName");
        //            datecolumnSName.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnSName);
        //            DataColumn datecolumnMName = new DataColumn("MiddleName");
        //            datecolumnMName.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnMName);
        //            DataColumn datecolumnCity = new DataColumn("City");
        //            datecolumnCity.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnCity);
        //            DataColumn datecolumnCountry = new DataColumn("Country");
        //            datecolumnCountry.AllowDBNull = true;
        //            csvData.Columns.Add(datecolumnCountry);

        //            int buffer = 0;
        //            while (!csvReader.EndOfData)
        //            {
        //                string[] fieldData = csvReader.ReadFields();
        //                csvData.Rows.Add(fieldData);

        //                buffer++;
        //                if (buffer == csvBufferSize)
        //                {
        //                    buffer = 0;

        //                    if (!ReadCSVToDatabase(csvData)) return 1;

        //                    csvData.Rows.Clear();
        //                }
        //            }

        //            if (buffer != 0)
        //            {
        //                if (!ReadCSVToDatabase(csvData)) return 1;
        //                csvData.Rows.Clear();
        //            }
        //            MessageBox.Show("Data has been imported to Database!");
        //        }
        //    }
        //    catch { return 2; }
        //    return 0;
        //}
    }

        public class PeopleClassMap : ClassMap<People>
        {
             public PeopleClassMap()
             {
                 Map(item => item.Date).Index(0);
                 Map(item => item.FirstName).Index(1);
                 Map(item => item.LastName).Index(2);
                 Map(item => item.MiddleName).Index(3);
                 Map(item => item.City).Index(4);
                 Map(item => item.Country).Index(5);
              }
        }
}
