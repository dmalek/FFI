using FFI.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFI.Commands.ReadFlatFile
{
    public class ReadFlatFileCommand: IFFICommand
    {
        public int BatchSize { get; set; } = 50000;
        public string NullString { get; set; }
        public string Separator { get; set; }
        public string LineTrimStart { get; set; }
        public string LineTrimEnd { get; set; }
        public int FirstLine { get; set; } = 1;
        public bool TrimValues { get; set; } = false;
        public string NewLineValue { get; set; }
        public string BooleanTrueValue { get; set; }
        public string BooleanFalseValue { get; set; }
        public StreamReader Reader { get; set; }
        public DataTable DataTable { get; set; }

        public delegate Task ReadBatchCompletedEventHandler(object sender, ReadBatchCompletedEventArgs e);
        public event ReadBatchCompletedEventHandler ReadBatchCompletedEvent;

        public async Task Execute()
        {
            int recordsCount = 0;
            int lineIndex = 0;

            var rowsInBatch = 0;
            while (Reader.Peek() >= 0)
            {
                lineIndex++;
                string lineTxt;
                lineTxt = await Reader.ReadLineAsync();              

                // Trim line
                if (!string.IsNullOrEmpty(LineTrimStart))
                {
                    lineTxt = lineTxt.TrimStart(LineTrimStart.ToCharArray());
                }
                if (!string.IsNullOrEmpty(LineTrimEnd))
                {
                    lineTxt = lineTxt.TrimEnd(LineTrimEnd.ToCharArray());
                }                
                
                string[] data = lineTxt.Split(Separator.ToCharArray());

                try
                {
                    if (lineIndex >= FirstLine)
                    {
                        ValidateData(data);

                        DataTable.Rows.Add(data);
                        rowsInBatch++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex.Message}");
                    Console.WriteLine($"Data {lineTxt}");
                    Console.WriteLine(data.ToString());
                    return;
                }

                recordsCount++;
                if (rowsInBatch >= BatchSize || Reader.Peek() <= 0)
                {
                    // call event
                    await ReadBatchCompletedEvent?.Invoke(this, new ReadBatchCompletedEventArgs()
                    {
                        LineIndex = lineIndex,
                        DataTable = this.DataTable,
                        RowsInBatch = rowsInBatch,
                        HasMore = Reader.Peek() >= 0,
                    });

                    rowsInBatch = 0;
                }
            }
        }

        private void ValidateData(string[] data)
        {
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                if (TrimValues)
                {
                    data[i] = data[i].Trim();
                }

                if (NullString != string.Empty)
                {
                    if (DataTable.Columns[i].AllowDBNull && data[i] == NullString)
                    {
                        data[i] = null;
                    }
                }

                if (data[i] != null && DataTable.Columns[i].DataType == typeof(String) && NewLineValue != string.Empty)
                {
                    data[i] = data[i].Replace(NewLineValue, Environment.NewLine);
                }

                if (DataTable.Columns[i].DataType == typeof(DateTime))
                {
                    if (!string.IsNullOrWhiteSpace(data[i]))
                    {
                        if (DateTime.TryParse(data[i], out var dateTimeRez))
                        {
                            if (dateTimeRez < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            {
                                data[i] = System.Data.SqlTypes.SqlDateTime.MinValue.Value.ToString();
                            }                            
                        }                       
                    }
                }

                if ((DataTable.Columns[i].DataType == typeof(bool))
                    || (DataTable.Columns[i].DataType == typeof(Boolean))
                    )
                {
                    if (BooleanTrueValue != string.Empty && data[i] == BooleanTrueValue)
                    {
                        data[i] = true.ToString();
                    }

                    if (BooleanFalseValue != string.Empty && data[i] == BooleanFalseValue)
                    {
                        data[i] = false.ToString();
                    }
                }
            }
        }
    }
}
