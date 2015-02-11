#region

using BAUG.LittleHelper;
using Serilog;

#endregion

namespace BAUG.BatchingRemote
{
    public class ThisMeetupTask
    {
        public static void TaskMain(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Information("Got to here!");

            var cruncher = new RsvpCruncher();

            var rsvps = cruncher.GetRsvps("qdxxblytdbpb");

            foreach (var result in rsvps.results)
            {
                Log.Information("Hello Mum {name} is in the cloud", result.member.name);
            }
        }
    }
}