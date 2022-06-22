using System;

namespace FFI.Processes.Import
{
    public class ImportParams
    {
        public string DestinationConnectionString { get; set; }
        public string ImportFilePath { get; set; }
        public string ImportFolderPath { get; set; }
        public string ImportFilePattern { get; set; } = "*";
        public bool TruncateTable { get; set; } = false;
        public int BatchSize { get; set; } = 50000;
        public string NullString { get; set; } = @"\N";
        public string Separator { get; set; } = "\t";
        public string LineTrimStart { get; set; } = "";
        public string LineTrimEnd { get; set; } = "";
        public int FirstLine { get; set; } = 1;
        public bool TrimValues { get; set; } = false;
        public string NewLineValue { get; set; } = @"\r\n";
        public string BooleanTrueValue { get; set; } = "t";
        public string BooleanFalseValue { get; set; } = "f";
        public string LogFilePath { get; set; }
    }
}
