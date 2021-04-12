using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
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
        public static List<People> ReadCSVFile(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", BadDataFound = null };
            try
            {
                using var streamReader = new StreamReader(path);
                using var csv = new CsvReader(streamReader, config);
                csv.Configuration.RegisterClassMap<PeopleClassMap>();
                return csv.GetRecords<People>().ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
                return null;
            }
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
            return true;
        }

        public static int readCSVandSave2DB(string path)
        {
            int csvBufferSize = 9000;
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(path))
                {
                    csvReader.SetDelimiters(new string[] { ";" });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();

                    DataColumn datecolumnDate = new DataColumn("Date");
                    datecolumnDate.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnDate);
                    DataColumn datecolumnName = new DataColumn("FirstName");
                    datecolumnName.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnName);
                    DataColumn datecolumnSName = new DataColumn("LastName");
                    datecolumnSName.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnSName);
                    DataColumn datecolumnMName = new DataColumn("MiddleName");
                    datecolumnMName.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnMName);
                    DataColumn datecolumnCity = new DataColumn("City");
                    datecolumnCity.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnCity);
                    DataColumn datecolumnCountry = new DataColumn("Country");
                    datecolumnCountry.AllowDBNull = true;
                    csvData.Columns.Add(datecolumnCountry);

                    int buffer = 0;
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        csvData.Rows.Add(fieldData);

                        buffer++;
                        if (buffer == csvBufferSize)
                        {
                            buffer = 0;

                            if (!ReadCSVToDatabase(csvData)) return 1;

                            csvData.Rows.Clear();
                        }
                    }

                    if (buffer != 0)
                    {
                        if (!ReadCSVToDatabase(csvData)) return 1;
                        csvData.Rows.Clear();
                    }
                    MessageBox.Show("Data has been imported to Database!");
                }
            }
            catch { return 2; }
            return 0;
        }
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
