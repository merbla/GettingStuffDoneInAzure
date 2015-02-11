
using System.Configuration;
using BAUG.LittleHelper;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Serilog;

namespace BAUG.WebJobUsingABlob
{
   internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            AddMeetup();

            var host = new JobHost();
            host.RunAndBlock();
        }

        private static void AddMeetup()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);
        

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("meetupevents");
            queue.CreateIfNotExists();

            Log.Information("Adding meetup id {id} to queue", "qdxxblytdbpb");

            var meetupEvent = new Event {id = "qdxxblytdbpb"};

            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(meetupEvent)));

            Log.Information("Queue message added");
        }
    }
}