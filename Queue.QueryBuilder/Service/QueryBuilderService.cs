using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Queue.Infrastructure;
using Queue.Model;
using Queue.QueryBuilder.Data;
using Queue.QueryBuilder.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Queue.QueryBuilder.Service
{
    public interface IQueryBuilderService
    {
        List<SqlCommand> CreateInsertQuery(RabbitMQInsertModel model);
        List<SqlCommand> CreateUpdateQuery(RabbitMQUpdateModel model);
        List<SqlCommand> CreateSoftDeleteQuery(RabbitMQSoftDeleteModel model);
        List<SqlCommand> CreateHardDeleteQuery(RabbitMQHardDeleteModel model);
    }

    public class QueryBuilderService : IQueryBuilderService
    {
        private readonly QueryBuilderDbContext _context;
        private readonly ISqlConnectionHelper _sqlConnectionHelper;
        public QueryBuilderService(ISqlConnectionHelper sqlConnectionHelper,
            QueryBuilderDbContext context)
        {
            _sqlConnectionHelper = sqlConnectionHelper;
            _context = context;
        }

        public List<SqlCommand> CreateInsertQuery(RabbitMQInsertModel model)
        {
            List<SqlCommand> result = new List<SqlCommand>();

            var table = _context.Tables
                .Include(x => x.Schema).ThenInclude(x => x.Database)
                .Include(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Schema).ThenInclude(x => x.Database)
                .FirstOrDefault(x =>
                    x.Schema.DatabaseId == model.DbId &&
                    x.Schema.Name == model.Schema &&
                    x.Name == model.Table);

            foreach (var item in table.Shadows)
            {
                var columns = item.Columns.Join(model.Values, c => c.Name, a => a.Key, (ci, a) => a);

                var command = _sqlConnectionHelper.CreateCommand(item.Schema.Database);

                // INSERT INTO [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] ( {#COLUMNS#} ) VALUES ( {#VALUES#} )
                var cmdText = _context.Queries.FirstOrDefault(x => x.AuditType == AuditType.Insert).CommandText;

                cmdText = cmdText.Replace("{#DATABASE#}", item.Schema.Database.Name);
                cmdText = cmdText.Replace("{#SCHEMA#}", item.Schema.Name);
                cmdText = cmdText.Replace("{#TABLE#}", item.Name);

                List<string> values = new List<string>();
                foreach (var c in columns)
                {
                    var column = item.Columns.FirstOrDefault(x => x.Name == c.Key);

                    if (!column.IsNumeric)
                        values.Add($"\'{c.Value}\'");
                    else
                        values.Add(c.Value);
                }

                cmdText = cmdText.Replace("{#COLUMNS#}", string.Join(", ", columns.Select(x => $"[{x.Key}]")));
                cmdText = cmdText.Replace("{#VALUES#}", string.Join(", ", values));

                if (!string.IsNullOrEmpty(cmdText))
                {
                    command.CommandText = cmdText;
                    //$"SET IDENTITY_INSERT [{item.Schema.Database.Name}].[{item.Schema.Name}].[{item.Name}] ON" +
                    //$"\n{cmdText}\n" +
                    //$"SET IDENTITY_INSERT [{item.Schema.Database.Name}].[{item.Schema.Name}].[{item.Name}] OFF";

                    result.Add(command);
                }
            }

            return result;
        }

        public List<SqlCommand> CreateUpdateQuery(RabbitMQUpdateModel model)
        {
            List<SqlCommand> result = new List<SqlCommand>();

            var table = _context.Tables
                .Include(x => x.Schema).ThenInclude(x => x.Database)
                .Include(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Schema).ThenInclude(x => x.Database)
                .FirstOrDefault(x => x.Schema.DatabaseId == model.DbId && x.Schema.Name == model.Schema && x.Name == model.Table);

            foreach (var item in table.Shadows)
            {
                var columns = item.Columns.Join(model.Values, c => c.Name, a => a.ColumnName, (ci, a) => a);

                var command = _sqlConnectionHelper.CreateCommand(item.Schema.Database);

                // UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMNS#} WHERE {#CONDITION#}
                var cmdText = _context.Queries.FirstOrDefault(x => x.AuditType == AuditType.Update).CommandText;
                cmdText = cmdText.Replace("{#DATABASE#}", item.Schema.Database.Name);
                cmdText = cmdText.Replace("{#SCHEMA#}", item.Schema.Name);
                cmdText = cmdText.Replace("{#TABLE#}", item.Name);

                string str = "";
                int count = 0;
                foreach (var c in columns)
                {
                    if (c.OldValue != c.NewValue)
                    {
                        var column = item.Columns.FirstOrDefault(x => x.Name == c.ColumnName);

                        if (!column.IsNumeric)
                            str += $"[{c.ColumnName}] = \'{c.NewValue}\', ";
                        else
                            str += $"[{c.ColumnName}] = {c.NewValue}, ";

                        count += 1;
                    }
                }

                if (count > 0)
                {
                    str = str.Substring(0, str.Length - 2);
                    cmdText = cmdText.Replace("{#COLUMNS#}", str);

                    var pkColumn = item.Columns.FirstOrDefault(x => x.Name == model.PrimaryKeyColumnName);
                    if (!pkColumn.IsNumeric)
                        cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = \'{model.EntityId}\'");
                    else
                        cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = {model.EntityId}");
                }
                else
                    cmdText = string.Empty;

                if (!string.IsNullOrEmpty(cmdText))
                    command.CommandText = cmdText;

                result.Add(command);
            }

            return result;
        }

        public List<SqlCommand> CreateSoftDeleteQuery(RabbitMQSoftDeleteModel model)
        {
            List<SqlCommand> result = new List<SqlCommand>();

            var table = _context.Tables
                .Include(x => x.Schema).ThenInclude(x => x.Database)
                .Include(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Schema).ThenInclude(x => x.Database)
                .FirstOrDefault(x => x.Schema.DatabaseId == model.DbId && x.Schema.Name == model.Schema && x.Name == model.Table);

            foreach (var item in table.Shadows)
            {
                var command = _sqlConnectionHelper.CreateCommand(item.Schema.Database);

                // UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMN#} = {#VALUE#} WHERE {#CONDITION#}
                var cmdText = _context.Queries.FirstOrDefault(x => x.AuditType == AuditType.SoftDelete).CommandText;
                cmdText = cmdText.Replace("{#DATABASE#}", item.Schema.Database.Name);
                cmdText = cmdText.Replace("{#SCHEMA#}", item.Schema.Name);
                cmdText = cmdText.Replace("{#TABLE#}", item.Name);

                var column = item.Columns.FirstOrDefault(x => x.Name == model.Value.Key);
                cmdText = cmdText.Replace("{#COLUMN#}", $"[{model.Value.Key}]");

                if (column.IsNumeric)
                    cmdText = cmdText.Replace("{#VALUE#}", $"{model.Value.Value}");
                else
                    cmdText = cmdText.Replace("{#VALUE#}", $"\'{model.Value.Value}\'");

                var pkColumn = item.Columns.FirstOrDefault(x => x.Name == model.PrimaryKeyColumnName);
                if (!pkColumn.IsNumeric)
                    cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = \'{model.EntityId}\'");
                else
                    cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = {model.EntityId}");

                if (!string.IsNullOrEmpty(cmdText))
                    command.CommandText = cmdText;

                result.Add(command);
            }

            return result;
        }

        public List<SqlCommand> CreateHardDeleteQuery(RabbitMQHardDeleteModel model)
        {
            List<SqlCommand> result = new List<SqlCommand>();

            var table = _context.Tables
                .Include(x => x.Schema).ThenInclude(x => x.Database)
                .Include(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Columns)
                .Include(x => x.Shadows).ThenInclude(x => x.Schema).ThenInclude(x => x.Database)
                .FirstOrDefault(x => x.Schema.DatabaseId == model.DbId && x.Schema.Name == model.Schema && x.Name == model.Table);

            foreach (var item in table.Shadows)
            {
                var command = _sqlConnectionHelper.CreateCommand(item.Schema.Database);

                // DELETE FROM [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] WHERE {#CONDITION#}
                var cmdText = _context.Queries.FirstOrDefault(x => x.AuditType == AuditType.HardDelete).CommandText;
                cmdText = cmdText.Replace("{#DATABASE#}", item.Schema.Database.Name);
                cmdText = cmdText.Replace("{#SCHEMA#}", item.Schema.Name);
                cmdText = cmdText.Replace("{#TABLE#}", item.Name);

                var pkColumn = item.Columns.FirstOrDefault(x => x.Name == model.PrimaryKeyColumnName);
                if (!pkColumn.IsNumeric)
                    cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = \'{model.EntityId}\'");
                else
                    cmdText = cmdText.Replace("{#CONDITION#}", $"[{model.PrimaryKeyColumnName}] = {model.EntityId}");

                if (!string.IsNullOrEmpty(cmdText))
                    command.CommandText = cmdText;

                result.Add(command);
            }

            return result;
        }
    }
}
