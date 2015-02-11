#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.Azure.Batch.FileStaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Serilog;
using Constants = Microsoft.Azure.Batch.Constants;

#endregion

namespace BAUG.BatchingRemote
{
    public class Job
    {
        private const string LittleHelperExeName = "BAUG.BatchingRemote.exe";
        private const string StorageClientDllName = "Microsoft.WindowsAzure.Storage.dll";

        public static void JobMain(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
              .WriteTo.ColoredConsole()
              .CreateLogger();

            //Load the configuration
            var configuration = BatchConfiguration.LoadConfigurationFromAppConfig();

            var stagingStorageAccount = new StagingStorageAccount(
                configuration.StorageAccountName,
                configuration.StorageAccountKey,
                configuration.StorageAccountBlobEndpoint);

            var client = BatchClient.Connect(configuration.BatchServiceUrl,
                new BatchCredentials(configuration.BatchAccountName, configuration.BatchAccountKey));
            string stagingContainer = null;

            //Create a pool (if user hasn't provided one)
            if (configuration.ShouldCreatePool)
            {
                using (var pm = client.OpenPoolManager())
                {
                      var pool = pm.CreatePool(configuration.PoolName, targetDedicated: configuration.PoolSize,
                        osFamily: "4", vmSize: "small");
                   
                    Log.Information("Adding pool {0}", configuration.PoolName);

                    pool.Commit();
                }
            }

            try
            {
                using (var wm = client.OpenWorkItemManager())
                {
                    var toolbox = client.OpenToolbox();

                    var taskSubmissionHelper = toolbox.CreateTaskSubmissionHelper(wm, configuration.PoolName);
                    taskSubmissionHelper.WorkItemName = configuration.WorkItemName;

                    var topNWordExe = new FileToStage(LittleHelperExeName, stagingStorageAccount);
                    var storageDll = new FileToStage(StorageClientDllName, stagingStorageAccount);
                    var serilog = new FileToStage("Serilog.dll", stagingStorageAccount);
                    var serilogFull = new FileToStage("Serilog.FullNetFx.dll", stagingStorageAccount);
                    var serviceStack = new FileToStage("ServiceStack.Text.dll", stagingStorageAccount);
                    var litteHelper = new FileToStage("BAUG.LittleHelper.exe", stagingStorageAccount);
                    var config = new FileToStage("BAUG.BatchingRemote.exe.config", stagingStorageAccount);
                     
                    foreach (var numberOfTask in Enumerable.Range(1,configuration.NumberOfTasks))
                    {
                        var task = new CloudTask("task_no_" + numberOfTask, String.Format("{0} --Task", LittleHelperExeName));

                        task.FilesToStage = new List<IFileStagingProvider>
                        {
                            topNWordExe,
                            storageDll,
                            serilog,
                            serilogFull,
                            serviceStack,
                            litteHelper,config
                        };

                        taskSubmissionHelper.AddTask(task);
                    }
                     
                    var artifacts = taskSubmissionHelper.Commit() as IJobCommitUnboundArtifacts;

                    foreach (var fileStagingArtifact in artifacts.FileStagingArtifacts)
                    {
                        var stagingArtifact = fileStagingArtifact.Value as SequentialFileStagingArtifact;
                        if (stagingArtifact != null)
                        {
                            stagingContainer = stagingArtifact.BlobContainerCreated;
                            Log.Information("Uploaded files to container: {0}",stagingArtifact.BlobContainerCreated);
                        }
                    }

                    //Get the job to monitor status.
                    var job = wm.GetJob(artifacts.WorkItemName, artifacts.JobName);

                    Console.Write("Waiting for tasks to complete ...");
                    // Wait 1 minute for all tasks to reach the completed state
                    client.OpenToolbox()
                        .CreateTaskStateMonitor()
                        .WaitAll(job.ListTasks(), TaskState.Completed, TimeSpan.FromMinutes(20));
                    Log.Information("Done.");

                    foreach (var task in job.ListTasks())
                    {
                        Log.Information("Task " + task.Name + " says:\n" + task.GetTaskFile(Constants.StandardOutFileName).ReadAsString());
                        Log.Information(task.GetTaskFile(Constants.StandardErrorFileName).ReadAsString());
                    }
                }
            }
            finally
            {
                //Delete the pool that we created
                if (configuration.ShouldCreatePool)
                {
                    using (var pm = client.OpenPoolManager())
                    {
                        Log.Information("Deleting pool: {0}", configuration.PoolName);
                        pm.DeletePool(configuration.PoolName);
                    }
                }

                //Delete the workitem that we created
                if (configuration.ShouldDeleteWorkItem)
                {
                    using (var wm = client.OpenWorkItemManager())
                    {
                        Log.Information("Deleting work item: {0}", configuration.WorkItemName);
                        wm.DeleteWorkItem(configuration.WorkItemName);
                    }
                }

                //Delete the containers we created
                if (configuration.ShouldDeleteContainer)
                {
                    DeleteContainers(configuration, stagingContainer);
                }
            }

            Console.ReadLine();
        }

        /// <summary>
        ///     Delete the containers in Azure Storage which are created by this sample.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileStagingContainer"></param>
        private static void DeleteContainers(BatchConfiguration configuration, string fileStagingContainer)
        {
            var cred = new StorageCredentials(configuration.StorageAccountName, configuration.StorageAccountKey);
            var storageAccount = new CloudStorageAccount(cred, true);
            var client = storageAccount.CreateCloudBlobClient();

            //Delete the books container
            var container = client.GetContainerReference("books");
            Log.Information("Deleting container: {0}", "books");
            container.DeleteIfExists();

            //Delete the file staging container
            if (!string.IsNullOrEmpty(fileStagingContainer))
            {
                container = client.GetContainerReference(fileStagingContainer);
                Log.Information("Deleting container: {0}", fileStagingContainer);
                container.DeleteIfExists();
            }
        }

        /// <summary>
        ///     Upload a text file to a cloud blob.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="fileName">The name of the file to upload</param>
        /// <returns>The URI of the blob.</returns>
        private static string UploadBookFileToCloudBlob(BatchConfiguration configuration, string fileName)
        {
            var cred = new StorageCredentials(configuration.StorageAccountName, configuration.StorageAccountKey);
            var storageAccount = new CloudStorageAccount(cred, true);
            var client = storageAccount.CreateCloudBlobClient();

            //Create the "books" container if it doesn't exist.
            var container = client.GetContainerReference("books");
            container.CreateIfNotExists();

            //Upload the blob.
            var blob = container.GetBlockBlobReference(fileName);
            blob.UploadFromFile(fileName, FileMode.Open);
            return blob.Uri.ToString();
        }
    }
}