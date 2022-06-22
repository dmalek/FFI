using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FFI.Extensions
{
    public static class SqlConnectionExtensions
    {
        public static async Task<DataTable> GetDataTableWithSchemaOnlyAsync(this SqlConnection connection, string tableName, SqlTransaction transaction = null)
        {
            DataTable dtResult = new DataTable();

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = String.Format("SELECT TOP 1 * FROM {0}", tableName);
                command.CommandType = CommandType.Text;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly);

                dtResult.Load(reader);

            }

            return dtResult;
        }

        public static async Task TruncateTableAsync(this SqlConnection connection, string tableName, SqlTransaction transaction = null)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = String.Format("TRUNCATE TABLE {0}", tableName);
                command.CommandType = CommandType.Text;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
