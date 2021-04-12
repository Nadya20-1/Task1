using ClosedXML.Excel;
using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;

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
                    SqlCommand command = new SqlCommand("SELECT * FROM People", dbConnection);

                    string query = "SELECT * FROM People WHERE Date = '" + MainWindow.Instance.data_text.Text + "' OR FirstName = N'" + MainWindow.Instance.firstname_text.Text + "' OR LastName = N'" + MainWindow.Instance.lastname_text.Text + "' OR MiddleName = N'" + MainWindow.Instance.middlename_text.Text + "' OR City = N'" + MainWindow.Instance.city_text.Text + "' OR Country = N'" + MainWindow.Instance.country_text.Text + "' ";

                    SqlCommand data_sampling = new SqlCommand(query, dbConnection);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(data_sampling);

                    dataAdapter.Fill(dataTable);

                    var excelApplication = new Microsoft.Office.Interop.Excel.Application();
                    var excelWorkBook = excelApplication.Application.Workbooks.Add(Type.Missing);

                    DataColumnCollection dataColumnCollection = dataTable.Columns;

                    for (int i = 1; i <= dataTable.Rows.Count + 1; i++)
                    {
                        for (int j = 1; j <= dataTable.Columns.Count; j++)
                        {
                            if (i == 1)
                                excelApplication.Cells[i, j] = dataColumnCollection[j - 1].ToString();
                            else
                                excelApplication.Cells[i, j] = dataTable.Rows[i - 2][j - 1].ToString();
                        }
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
                    string CmdString = "SELECT * FROM People";

                    string query = "SELECT * FROM People WHERE Date = '" + MainWindow.Instance.data_text.Text + "' OR FirstName = N'" + MainWindow.Instance.firstname_text.Text + "' OR LastName = N'" + MainWindow.Instance.lastname_text.Text + "' OR MiddleName = N'" + MainWindow.Instance.middlename_text.Text + "' OR City = N'" + MainWindow.Instance.city_text.Text + "' OR Country = N'" + MainWindow.Instance.country_text.Text + "' ";

                    SqlCommand data_sampling = new SqlCommand(query, dbConnection);

                    SqlCommand command = new SqlCommand(CmdString, dbConnection);
                    dbConnection.Open();
                    DataTable dataTable = new DataTable("People");
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(data_sampling);
                    dataAdapter.Fill(dataTable);
                    dataTable.WriteXml(path);
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
