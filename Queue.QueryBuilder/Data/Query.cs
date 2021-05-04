using Queue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class Query
    {
        public Guid Id { get; set; }
        public AuditType AuditType { get; set; }
        public string CommandText { get; set; }
    }
}
