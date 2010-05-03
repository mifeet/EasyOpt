﻿using System;
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
            Parser parser = new Parser();

            var outputParam = new StringParameter(true, "FILE");
            var output = Parser.CreateOption(
                false,
                "Do not send the results to stderr, but overwrite the specified file.",
                outputParam
            );
            parser.AddOption(output, 'o', "output");

            var append = Parser.CreateOption(false, "(Used together with -o.) Do not overwrite but append");
            parser.AddOption(append, 'a', "append");

            var formatParam = new StringParameter(true, "FORMAT", "real %f\nuser %f\nsys %f\n");
            var format = Parser.CreateOption( 
                false,
                "Specify output format, possibly overriding the format specified in the environment variable TIME.",
                formatParam
            );
            parser.AddOption(format, 'f', "format");

            var help = Parser.CreateOption(false, "Print a usage message on standard output and exit successfully.");
            parser.AddOption(help, "help");

            var portability = Parser.CreateOption(false, "Use the portable output format");
            parser.AddOption(portability, 'p', "portability");

            var verbose = Parser.CreateOption(false, "Give very verbose output about all the program knows about.");
            parser.AddOption(verbose, 'v', "verbose");

            var quiet = Parser.CreateOption(false, "Do not report the status of the program even if it is different");
            parser.AddOption(quiet, "quiet");

            var version = Parser.CreateOption(false, "Print version information on standard output, then exit successfully.");
            parser.AddOption(version, 'V', "version");

            parser.UsageText = " time [options] command [arguments...]\n\n";

            parser.Parse(args);

            Console.WriteLine("{0}: {1}", "output", output.Value);
            Console.WriteLine("{0}: {1}", "append", append.Value);
            Console.WriteLine("{0}: {1}", "format", format.Value);
            Console.WriteLine("{0}: {1}", "help", help.Value);
            Console.WriteLine("{0}: {1}", "portability", portability.Value);
            Console.WriteLine("{0}: {1}", "verbose", verbose.Value);
            Console.WriteLine("{0}: {1}", "quiet", quiet.Value);
            Console.WriteLine("{0}: {1}", "version", version.Value);

            var arguments = parser.GetArguments();
            Console.WriteLine("\nArguments: {0}", String.Join(" ", arguments));
            Console.WriteLine("\nUsage text:");
            Console.WriteLine(parser.GetUsage());
            Console.ReadLine();
        }
    }
}