using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FFI.Base;

namespace FFI.Commands.BulkCopy
{
    public class BulkCopyCommand : IFFICommand
    {
        public string DestinationTableName { get; set; }
        public DataTable DataTable { get; set; }
        public SqlConnection Connection { get; set; }
        public async Task Execute()
        {
            using (var bulk = new SqlBulkCopy(Connection))
            {
                bulk.DestinationTableName = DestinationTableName;
                await bulk.WriteToServerAsync(DataTable);
            }
        }
    }
}
