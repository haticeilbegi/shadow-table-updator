using Queue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Model
{
    public class RabbitMQDataModel : EventArgs
    {
        public string HashedValue { get; set; }
        public Guid MessageId { get; set; }
        public WorkerOptions WorkerOptions { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public AuditType AuditType { get; set; }
        public Guid DbId { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }
    }

    public class RabbitMQKeyValueModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RabbitMQInsertModel : RabbitMQDataModel
    {
        public IEnumerable<RabbitMQKeyValueModel> Values { get; set; }
    }

    public class RabbitMQUpdateModel : RabbitMQDataModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
        public IEnumerable<AuditValue> Values { get; set; }

        public class AuditValue
        {
            public string ColumnName { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }
    }

    public class RabbitMQSoftDeleteModel : RabbitMQDataModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
        public RabbitMQKeyValueModel Value { get; set; }
    }

    public class RabbitMQHardDeleteModel : RabbitMQDataModel
    {
        public string PrimaryKeyColumnName { get; set; }
        public string EntityId { get; set; }
    }
}
