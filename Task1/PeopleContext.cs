using System;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Task1
{
    public partial class PeopleContext : System.Data.Entity.DbContext
    {
        public PeopleContext()  : base("Population")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PeopleContext>());
        }
        public virtual System.Data.Entity.DbSet<People> DbPeople { get; set; }

    }
}
