using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyOpt;

/**
 * Use case of EasyOpt: selected GNU time command line arguments.
 */
namespace UseCaseTime
{
    class Program
    {
        static void Main(string[] args)
        {
            
            CommandLine parser = new CommandLine(args);

            var outputParam = new StringParameter(true, "FILE");
            var output = OptionFactory.Create(
                false,
                "Do not send the results to stderr, but overwrite the specified file.",
                outputParam
            );
            parser.AddOption(output, 'o', "output");

            var append = OptionFactory.Create(false, "(Used together with -o.) Do not overwrite but append");
            parser.AddOption(append, 'a', "append");

            var formatParam = new StringParameter(true, "FORMAT", "real %f\nuser %f\nsys %f\n");
            var format = OptionFactory.Create( 
                false,
                "Specify output format, possibly overriding the format specified in the environment variable TIME.",
                formatParam
            );
            parser.AddOption(format, 'f', "format");

            var help = OptionFactory.Create(false, "Print a usage message on standard output and exit successfully.");
            parser.AddOption(help, "help");

            var portability = OptionFactory.Create(false, "Use the portable output format");
            parser.AddOption(portability, 'p', "portability");

            var verbose = OptionFactory.Create(false, "Give very verbose output about all the program knows about.");
            parser.AddOption(verbose, 'v', "verbose");

            var quiet = OptionFactory.Create(false, "Do not report the status of the program even if it is different");
            parser.AddOption(quiet, "quiet");

            var version = OptionFactory.Create(false, "Print version information on standard output, then exit successfully.");
            parser.AddOption(version, 'V', "version");

            parser.UsageDescription = " time [options] command [arguments...]";

            try
            {
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}:\n{1}\n\n", e.GetType().Name, e.Message);
            }

            Console.WriteLine("{0}: {1}", "output", output.Value);
            Console.WriteLine("{0}: {1}", "append", append.Value);
            Console.WriteLine("{0}: {1}", "format", format.Value);
            Console.WriteLine("{0}: {1}", "help", help.Value);
            Console.WriteLine("{0}: {1}", "portability", portability.Value);
            Console.WriteLine("{0}: {1}", "verbose", verbose.Value);
            Console.WriteLine("{0}: {1}", "quiet", quiet.Value);
            Console.WriteLine("{0}: {1}", "version", version.Value);

            var arguments = parser.GetArguments();
            Console.WriteLine("\nArguments: {0}", String.Join(" ", (String[]) arguments));
            Console.WriteLine("\nUsage text:");
            Console.WriteLine(parser.GetUsage());
            Console.ReadLine();
        }
    }
}
