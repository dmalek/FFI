using FFI.Base;
using FFI.Helpers;
using FFI.Processes.Import;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FFI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            ImportParams importParams = new ImportParams();

            var argValues = ArgsParser.Parse(args);

            if (argValues.Count == 0)
            {
                return;
            }
            
            if (argValues.TryGetValue("--new", out var newProjectFileName))
            {
                // Create empty project
                await JsonFile.SaveAsync(new ImportParams(), newProjectFileName);
                return;
            }
            else if (argValues.TryGetValue("--run", out var projectFileName))
            { 
                // Load project
                importParams = await JsonFile.LoadAsync<ImportParams>(projectFileName); 
            }

            if (argValues.TryGetValue("--import", out var importFile))
            {
                importParams.ImportFilePath = importFile;
            }
            if (argValues.TryGetValue("--importdir", out var importPath))
            {
                importParams.ImportFolderPath = importPath;
            }
            if (argValues.TryGetValue("--pattern", out var importPattern))
            {
                importParams.ImportFilePattern = importPattern;
            }
            if (argValues.TryGetValue("--log", out var logFilePath))
            {
                importParams.LogFilePath = logFilePath;
            }

            // Prepare
            var logger = new FFILogger();
            logger.LogFilePath = importParams.LogFilePath;
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // Execute project
            var importProcess = new ImportProcess();
            importProcess.Logger = logger;
            await importProcess.Execute(importParams);


            // Finish
            TimeSpan ts = stopWatch.Elapsed;
            logger.Log("Total import time " + ts.ToString());
        }
    }
}
