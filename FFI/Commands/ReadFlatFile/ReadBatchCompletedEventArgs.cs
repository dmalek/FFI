using System.Data;

namespace FFI.Commands.ReadFlatFile
{
    public class ReadBatchCompletedEventArgs
    {
        public DataTable DataTable { get; set; }
        public int LineIndex { get; set; }
        public int RowsInBatch { get; set; }
        public bool HasMore { get; set; }
    }
}
