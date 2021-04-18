using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Task1
{
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
