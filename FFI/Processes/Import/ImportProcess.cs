using FFI.Base;
using FFI.Commands.BulkCopy;
using FFI.Commands.ReadFlatFile;
using FFI.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace FFI.Processes.Import
{
    public class ImportProcess : IFFIProcess<ImportParams>
    {
        public IFFILogger Logger { get; set; }

        private List<string> _filesToImport = new List<string>();

        public async Task Execute(ImportParams parameters)
        {
            if (!string.IsNullOrEmpty(parameters.ImportFilePath))
            {
                _filesToImport.Add(parameters.ImportFilePath);
            }
            else if (!string.IsNullOrEmpty(parameters.ImportFolderPath))
            {
                _filesToImport.AddRange(Directory.GetFiles(parameters.ImportFolderPath, parameters.ImportFilePattern, SearchOption.TopDirectoryOnly));
            }

            foreach (var file in _filesToImport)
            {
                var tablename = Path.GetFileNameWithoutExtension(file);
                Logger.Log("Importing into table '{0}' from '{1}'.", tablename, file);

                using (SqlConnection connection = new SqlConnection(parameters.DestinationConnectionString))
                {
                    connection.Open();
                    if (parameters.TruncateTable)
                    {
                        Logger.Log("Truncating table '{0}'.", tablename, file);
                        await connection.TruncateTableAsync(tablename);
                    }
                    DataTable dataTable = await connection.GetDataTableWithSchemaOnlyAsync(tablename);

                    try
                    {
                        await ImportFile(parameters, file, tablename, connection, dataTable);
                    }
                    catch (System.Exception e)
                    {
                        Logger.Log("Exception: {0}", e.Message);                            
                    }                                           

                    connection.Close();
                }
            }
        }

        private async Task ImportFile(ImportParams parameters, string file, string tablename, SqlConnection connection, DataTable dataTable)
        {
            using (var reader = new System.IO.StreamReader(file))
            {
                var readCmd = new ReadFlatFileCommand()
                {
                    BatchSize = parameters.BatchSize,
                    NewLineValue = parameters.NewLineValue,
                    LineTrimStart = parameters.LineTrimStart,
                    LineTrimEnd = parameters.LineTrimEnd,
                    Separator = parameters.Separator,
                    NullString = parameters.NullString,
                    FirstLine = parameters.FirstLine,
                    BooleanTrueValue = parameters.BooleanTrueValue,
                    BooleanFalseValue = parameters.BooleanFalseValue,

                    DataTable = dataTable,
                    Reader = reader,

                };
                readCmd.ReadBatchCompletedEvent += async (s, e) =>
                {
                    var bulkCopy = new BulkCopyCommand()
                    {
                        DestinationTableName = tablename,
                        DataTable = e.DataTable,
                        Connection = connection,
                    };

                    await bulkCopy.Execute();
                    Logger.Log("Imported {0} rows.", e.LineIndex);
                    e.DataTable.Rows.Clear();
                };
                await readCmd.Execute();
            }
        }
    }
}
