using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Column
    {
        public Guid Id { get; set; }

        [ForeignKey("Table")]
        public Guid TableId { get; set; }
        public Table Table { get; set; }

        public string Name { get; set; }
        public bool IsNumeric { get; set; }
    }
}
