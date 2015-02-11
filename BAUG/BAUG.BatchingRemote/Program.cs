namespace BAUG.BatchingRemote
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            if (args != null && args.Length > 0 && args[0] == "--Task")
            {
                ThisMeetupTask.TaskMain(args);
            }
            else
            {
                Job.JobMain(args);
            }
        }
    }
}
