using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Model
{
    public class QueueModel : EventArgs
    {
        public WorkerOptions WorkerOptions { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public Guid DbId { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }
    }

    public class InsertModel : QueueModel
    {
        public IEnumerable<KeyValueModel> Values { get; set; }
    }

    public class UpdateModel : QueueModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
        public IEnumerable<AuditValue> Values { get; set; }
    }

    public class SoftDeleteModel : QueueModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
        public KeyValueModel Value { get; set; }
    }

    public class HardDeleteModel : QueueModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
    }


    public class KeyValueModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class AuditValue
    {
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
