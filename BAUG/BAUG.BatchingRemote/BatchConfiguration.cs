﻿#region

using System;
using System.Configuration;

#endregion

namespace BAUG.BatchingRemote
{
    /// <summary>
    ///     The configuration for TopNWords.
    /// </summary>
    public class BatchConfiguration
    {
        /// <summary>
        ///     The Batch service URL.
        /// </summary>
        public string BatchServiceUrl { get; private set; }

        /// <summary>
        ///     The Batch account name to run the sample against.
        /// </summary>
        public string BatchAccountName { get; private set; }

        /// <summary>
        ///     The Batch account key.
        /// </summary>
        public string BatchAccountKey { get; private set; }

        /// <summary>
        ///     The number of tasks to create.
        /// </summary>
        public int NumberOfTasks { get; private set; }

        /// <summary>
        ///     The size of the VMs to use in the pool.
        /// </summary>
        public int PoolSize { get; private set; }

        /// <summary>
        ///     The number of top N words to calculate (5 would mean the top 5 words).
        /// </summary>
        public int NumberOfTopWords { get; private set; }

        /// <summary>
        ///     The name of the pool.
        /// </summary>
        public string PoolName { get; private set; }

        /// <summary>
        ///     If a pool should be created.
        /// </summary>
        public bool ShouldCreatePool { get; private set; }

        /// <summary>
        ///     The name of the work item.
        /// </summary>
        public string WorkItemName { get; private set; }

        /// <summary>
        ///     The name of the storage account to store the files required to run the tasks.
        /// </summary>
        public string StorageAccountName { get; private set; }

        /// <summary>
        ///     The key of the storage account to store the files required to run the tasks.
        /// </summary>
        public string StorageAccountKey { get; private set; }

        /// <summary>
        ///     The storage accounts blob endpoint.
        /// </summary>
        public string StorageAccountBlobEndpoint { get; private set; }

        /// <summary>
        ///     The file name containing the book to process.
        /// </summary>
        public string BookFileName { get; private set; }

        /// <summary>
        ///     If the work item should be deleted when the sample ends.
        /// </summary>
        public bool ShouldDeleteWorkItem { get; private set; }

        /// <summary>
        ///     If the container should be deleted when the sample ends.
        /// </summary>
        public bool ShouldDeleteContainer { get; private set; }

        /// <summary>
        ///     Loads the configuration from the App.Config file
        /// </summary>
        /// <returns></returns>
        public static BatchConfiguration LoadConfigurationFromAppConfig()
        {
            var configuration = new BatchConfiguration();

            configuration.BatchServiceUrl = ConfigurationManager.AppSettings["BatchServiceUrl"];
            configuration.BatchAccountName = ConfigurationManager.AppSettings["Account"];
            configuration.BatchAccountKey = ConfigurationManager.AppSettings["Key"];

            configuration.NumberOfTasks = Int32.Parse(ConfigurationManager.AppSettings["NumTasks"]);
            configuration.PoolSize = Int32.Parse(ConfigurationManager.AppSettings["PoolSize"]);
            configuration.NumberOfTopWords = Int32.Parse(ConfigurationManager.AppSettings["NumTopWords"]);

            configuration.PoolName = ConfigurationManager.AppSettings["PoolName"];
            configuration.ShouldCreatePool = string.IsNullOrEmpty(configuration.PoolName);

            if (configuration.ShouldCreatePool)
            {
                configuration.PoolName = "BAUG_Pool" + DateTime.Now.ToString("_yyMMdd_HHmmss_") +
                                         Guid.NewGuid().ToString("N");
            }

            configuration.WorkItemName = "BAUG_WorkItem" + DateTime.Now.ToString("_yyMMdd_HHmmss_") +
                                         Guid.NewGuid().ToString("N");

            configuration.StorageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
            configuration.StorageAccountKey = ConfigurationManager.AppSettings["StorageAccountKey"];
            configuration.StorageAccountBlobEndpoint = ConfigurationManager.AppSettings["StorageAccountBlobEndpoint"];
           

            configuration.ShouldDeleteWorkItem = bool.Parse(ConfigurationManager.AppSettings["DeleteWorkitem"]);
            configuration.ShouldDeleteContainer = bool.Parse(ConfigurationManager.AppSettings["DeleteContainer"]);


            return configuration;
        }
    }
}