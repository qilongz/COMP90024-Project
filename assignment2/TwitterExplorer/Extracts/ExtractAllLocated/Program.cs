using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtractAllLocated
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(
                    $"Usage: {AppDomain.CurrentDomain.FriendlyName} tgtDir srcDir1 [srcDir2 ....] ");
                return;
            }

            Console.WriteLine($"Start {DateTime.Now}");

            var tgtLoc = args[0];
            var srcLocs = new List<string>(args.Skip(1));

            var eng = new BatchEngine(6, srcLocs, tgtLoc);

            eng.Process();


            Console.WriteLine($"Done {DateTime.Now}");
        }
    }
}