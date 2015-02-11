using System;
using System.Configuration;
using Serilog;
using ServiceStack;

namespace BAUG.LittleHelper
{
    public class RsvpCruncher
    {
        private const string EventsUri = "https://api.meetup.com/2/events?group_id=3523812&status=upcoming&order=time&limited_events=False&desc=false&offset=0&photo-host=public&format=json&page=20&fields=&sig_id=12748095&sig=5e815345666721275b191d1cdb13e11d41b49a3f";
        private const string RSVPsUri = "https://api.meetup.com/2/rsvps?&sign=true&rsvp=yes&page=1000";

        private  string ApiKey
        {
            get { return ConfigurationManager.AppSettings["MeetupKey"]; }
        }

        public RsvpCruncher()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();
        }

        public EventResult Go()
        {
            Log.Information("Lets Go!");

            var eventsUri = new Uri(String.Format("{0}&key={1}", EventsUri, ApiKey));

            var events = eventsUri.ToString().GetJsonFromUrl().FromJson<EventResult>();

            foreach (var evt in events.results)
            {
                var rsvpUri = new Uri(String.Format("{0}&key={1}&event_id={2}", RSVPsUri, ApiKey, evt.id));
                
                var rsvps = rsvpUri.ToString().GetJsonFromUrl().FromJson<RsvpResult>();

                Log.Information("Event - {@evt} ", evt);
                Log.Information("RSVPs - {count}", rsvps.meta.total_count);

                evt.Rsvps = rsvps;
            }

            return events;
        }

        public void GetRsvps()
        {
            var eventsUri = new Uri(String.Format("{0}&key={1}", EventsUri, ApiKey));

            var events = eventsUri.ToString().GetJsonFromUrl().FromJson<EventResult>();

            foreach (var evt in events.results)
            {
                var rsvpUri = new Uri(String.Format("{0}&key={1}&event_id={2}", RSVPsUri, ApiKey, evt.id));

                var rsvps = rsvpUri.ToString().GetJsonFromUrl().FromJson<RsvpResult>();

                Log.Information("Event - {@evt} ", evt);

                Log.Information("RSVPs - {count}", rsvps.meta.total_count);

            }
        }


        public RsvpResult GetRsvps(string eventId)
        {
            var rsvpUri = new Uri(String.Format("{0}&key={1}&event_id={2}", RSVPsUri, ApiKey, eventId));

            var rsvps = rsvpUri.ToString().GetJsonFromUrl().FromJson<RsvpResult>();

            return rsvps;
        }
    }
}