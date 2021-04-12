using System;
using System.Windows;
using CsvHelper.Configuration.Attributes;

namespace Task1
{
    public partial class People
    {
        private DateTime date;

        [Name("id")]
        public int Id { get; set; }
        [Name("date_time")]
        public string Date
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
        [Name("first_name")]
        public string FirstName { get; set; }
        [Name("last_name")]
        public string LastName { get; set; }
        [Name("middle_name")]
        public string MiddleName { get; set; }
        [Name("city")]
        public string City { get; set; }
        [Name("country")]
        public string Country { get; set; }
    }
}
