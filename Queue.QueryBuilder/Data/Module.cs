using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Database> Databases { get; set; }
    }
}
