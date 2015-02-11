using System;
using System.Collections.Generic;

namespace BAUG.LittleHelper
{

    public class Event
    { 
        public string id { get; set; }
        public DateTime utc_offset { get; set; }
        public string name { get; set; }
        public RsvpResult Rsvps{ get; set; }

    }
}