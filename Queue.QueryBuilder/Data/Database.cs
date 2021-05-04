using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Database
    {
        public Guid Id { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }
        public Module Module { get; set; }

        public string Name { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public ICollection<Schema> Schemas { get; set; }
    }
}
