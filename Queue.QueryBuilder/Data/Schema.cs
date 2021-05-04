using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Schema
    {
        public Guid Id { get; set; }

        [ForeignKey("Database")]
        public Guid DatabaseId { get; set; }
        public Database Database { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Table> Tables { get; set; }
    }
}
