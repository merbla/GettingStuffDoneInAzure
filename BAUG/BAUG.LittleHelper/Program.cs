using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace BAUG.LittleHelper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cruncher = new RsvpCruncher();
            var eventResult = cruncher.Go();

            if (args.Any())
            {
                var file = args[0];
                var filePath = Path.Combine(Environment.CurrentDirectory, file);

                File.WriteAllText(file, eventResult.ToJson());

                if (args.Count() > 1)
                {
                    bool launch;
                    bool.TryParse(args[1], out launch);

                    if (launch)
                        Process.Start(filePath);
                }
            }
        }
    }
}