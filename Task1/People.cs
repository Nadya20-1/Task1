using System;
using System.Windows;
using CsvHelper.Configuration.Attributes;
using System.Data.Linq.Mapping;

namespace Task1
{
    [Table(Name = "People")]
    public partial class People
    {
        private DateTime date;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(Name = "Date")]

        [Name("Date")]
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

        [Column(Name = "FirstName")]
        [Name("FirstName")]
        public string FirstName { get; set; }

        [Column(Name = "LastName")]
        [Name("LastName")]
        public string LastName { get; set; }
      
        [Column(Name = "MiddleName")]
        [Name("MiddleName")]
        public string MiddleName { get; set; }
    
        [Column(Name = "City")]
        [Name("City")]
        public string City { get; set; }
  
        [Column(Name = "Country")]
        [Name("Country")]
        public string Country { get; set; }
    }
}
