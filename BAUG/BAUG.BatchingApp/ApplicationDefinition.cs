#region

using Microsoft.Azure.Batch.Apps.Cloud;

#endregion

namespace BAUG.BatchingApp
{
    public class ApplicationDefinition
    {
        public static readonly CloudApplication Application = new ParallelCloudApplication
        {
            ApplicationName = "baug-rsvps-batching",
            JobType = "baug-rsvps-crunch",
            JobSplitterType = typeof (BatchingAppJobSplitter),
            TaskProcessorType = typeof (BatchingAppTaskProcessor)
        };
    }
}