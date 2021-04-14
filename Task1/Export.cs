using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Task1
{
    public class Export
    {
        public static void ExportToExcel(string path)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection dbConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Population;Integrated Security=True;"))
                try
                {
                    dbConnection.Open();
                    DataContext db = new DataContext(dbConnection);

                    var query = db.GetTable<People>()
                                     .WhereIfRightIsNotDefault(p => p.Date == MainWindow.Instance.date_text.Text)
                                     .WhereIfRightIsNotDefault(p => p.FirstName == MainWindow.Instance.firstname_text.Text.ToUpper())
                                     .WhereIfRightIsNotDefault(p => p.LastName == MainWindow.Instance.lastname_text.Text.ToUpper())
                                     .WhereIfRightIsNotDefault(p => p.MiddleName == MainWindow.Instance.middlename_text.Text.ToUpper())
                                     .WhereIfRightIsNotDefault(p => p.City == MainWindow.Instance.city_text.Text.ToUpper())
                                     .WhereIfRightIsNotDefault(p => p.Country == MainWindow.Instance.country_text.Text.ToUpper()).ToList();

                    var excelApplication = new Microsoft.Office.Interop.Excel.Application();
                    var excelWorkBook = excelApplication.Application.Workbooks.Add(Type.Missing);

                    excelApplication.Cells[1, 1] = "Date";
                    excelApplication.Cells[1, 2] = "FirstName";
                    excelApplication.Cells[1, 3] = "LastName";
                    excelApplication.Cells[1, 4] = "MiddleName";
                    excelApplication.Cells[1, 5] = "City";
                    excelApplication.Cells[1, 6] = "Country";
                    int i = 2;
                    foreach (People q in query)
                    {
                        excelApplication.Cells[i, 1] = q.Date.ToString();
                        excelApplication.Cells[i, 2] = q.FirstName.ToString();
                        excelApplication.Cells[i, 3] = q.LastName.ToString();
                        excelApplication.Cells[i, 4] = q.MiddleName.ToString();
                        excelApplication.Cells[i, 5] = q.City.ToString();
                        excelApplication.Cells[i, 6] = q.Country.ToString();
                        i++;
                    }

                    excelApplication.ActiveWorkbook.SaveCopyAs(path);
                    excelApplication.ActiveWorkbook.Saved = true;

                    MessageBox.Show("Data has been exported to Excel!");

                    excelApplication.Quit();
                    dbConnection.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Exception " + ex);
                }
        }

        public static void ExportToXML(string path)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Population;Integrated Security=True;"))
                {
                    DataContext db = new DataContext(dbConnection);
                    dbConnection.Open();

                    var query = db.GetTable<People>()
                                   .WhereIfRightIsNotDefault(p => p.Date == MainWindow.Instance.date_text.Text)
                                   .WhereIfRightIsNotDefault(p => p.FirstName == MainWindow.Instance.firstname_text.Text.ToUpper())
                                   .WhereIfRightIsNotDefault(p => p.LastName == MainWindow.Instance.lastname_text.Text.ToUpper())
                                   .WhereIfRightIsNotDefault(p => p.MiddleName == MainWindow.Instance.middlename_text.Text.ToUpper())
                                   .WhereIfRightIsNotDefault(p => p.City == MainWindow.Instance.city_text.Text.ToUpper())
                                   .WhereIfRightIsNotDefault(p => p.Country == MainWindow.Instance.country_text.Text.ToUpper()).ToList();

                    XElement elements = new XElement("TestProgram",
                          query.Select(item => new XElement("Record",
                                      new XAttribute("Id", item.Id),
                                      new XElement("Date", item.Date),
                                      new XElement("FirstName", item.FirstName),
                                      new XElement("LastName", item.LastName),
                                      new XElement("MiddleName", item.MiddleName),
                                      new XElement("City", item.City),
                                      new XElement("Country", item.Country))));
                    elements.Save(path);
                    dbConnection.Close();
                }
                MessageBox.Show("Data has been exported to XML!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception " + ex);
            }
        }
    }
}
