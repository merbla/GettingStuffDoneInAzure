using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Batch.Apps.Cloud;

namespace BAUG.BatchingApp
{
    /// <summary>
    /// Splits a job into tasks.
    /// </summary>
    public class BatchingAppJobSplitter : JobSplitter
    {
        /// <summary>
        /// Splits a job into more granular tasks to be processed in parallel.
        /// </summary>
        /// <param name="job">The job to be split.</param>
        /// <param name="settings">Contains information and services about the split request.</param>
        /// <returns>A sequence of tasks to be run on compute nodes.</returns>
        protected override IEnumerable<TaskSpecifier> Split(IJob job, JobSplitSettings settings)
        {
            return new List<TaskSpecifier>
                {
                    new TaskSpecifier
                        {
                            RequiredFiles = job.Files,
                            TaskId = 1,
                            TaskIndex = 1,
                            Parameters = job.Parameters,
                        },
                    new TaskSpecifier
                        {
                            RequiredFiles = job.Files,
                            TaskId = 2,
                            TaskIndex = 2,
                            Parameters = job.Parameters,
                        },
                };
        }
    }
}
