using System;
using System.Collections.Generic;
using System.Text;

namespace Task1
{
    public interface IRepository<People> where People : class
    {
        void Create(People item);
        People FindById(int id);
        IEnumerable<People> Get();
        IEnumerable<People> Get(Func<People, bool> predicate);
        void Remove(People item);
        void Update(People item);
    }
}
