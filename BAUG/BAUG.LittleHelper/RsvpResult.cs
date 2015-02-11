using System.Collections.Generic;

namespace BAUG.LittleHelper
{
    public class RsvpResult
    {
        public List<Rsvp> results { get; set; }
        public MetaData meta { get; set; }
    }

    public class Rsvp
    {
        public Member member { get; set; }
    }
}