#region

using System.IO;
using System.Text;
using System.Threading.Tasks;
using BAUG.LittleHelper;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Serilog;

#endregion

namespace BAUG.WebJobUsingABlob
{
    public class Functions
    {

        public static async Task ProcessMeetupMessage(
            [QueueTrigger("meetupevents")] Event meetupEvent,
            string id,
            [Blob("meetupevents/{id}", FileAccess.Write)] Stream output)

        {
            Log.Information("Processing Meetup Event Id {id}", id);

            var cruncher = new RsvpCruncher();
            var rsvps = cruncher.GetRsvps(id);

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rsvps));

            await output.WriteAsync(messageBytes, 0, messageBytes.Length);

            Log.Information("Completed processing Meetup Event Id {id}", id);
        }
    }
}