#region

using System;
using System.Linq;
using Microsoft.Azure.Batch.Apps.Cloud;
using Serilog;

#endregion

namespace BAUG.BatchingApp
{
    /// <summary>
    ///     Processes a task.
    /// </summary>
    public class BatchingAppTaskProcessor : ParallelTaskProcessor
    {
        const string RsvpJsonFileName = "rsvp.json";

        protected override TaskProcessResult RunExternalTaskProcess(ITask task, TaskExecutionSettings settings)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
               .WriteTo.ColoredConsole()
               .CreateLogger();

            Log.Info("Starting");

            var process = new ExternalProcess
            {
                CommandPath = ExecutablePath("BAUG.LittleHelper.exe"),
                Arguments = string.Format(RsvpJsonFileName),
                WorkingDirectory = LocalStoragePath
            };

            var processOutput = process.Run();

            Log.Info("Done");

            return new TaskProcessResult
            {
                Success = TaskProcessSuccess.Succeeded,
                ProcessorOutput = processOutput.StandardOutput
            };
        } 

        protected override JobResult RunExternalMergeProcess(ITask mergeTask, TaskExecutionSettings settings)
        {
            var mergeTaskId = mergeTask.TaskId;
           
            var process = new ExternalProcess
            {
                CommandPath = ExecutablePath("BAUG.LittleHelper.exe"),
                Arguments = string.Format(RsvpJsonFileName),
                WorkingDirectory = LocalStoragePath
            };

            var processOutput = process.Run();


          
            return new JobResult
            { 
                OutputFile = RsvpJsonFileName,
                PreviewFile = RsvpJsonFileName
            };
        }
    }
}