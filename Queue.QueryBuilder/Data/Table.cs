using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Table
    {
        public Guid Id { get; set; }

        [ForeignKey("Schema")]
        public Guid SchemaId { get; set; }
        public Schema Schema { get; set; }

        public string Name { get; set; }

        [ForeignKey("OriginalTable")]
        public Guid? OriginalTableId { get; set; }
        public Table OriginalTable { get; set; }

        public virtual ICollection<Column> Columns { get; set; }
        public virtual ICollection<Table> Shadows { get; set; }
    }
}
