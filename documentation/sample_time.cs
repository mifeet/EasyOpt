using System;
using EasyOptLibrary;

namespace EasyOptSampleTime
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a parser instance
            EasyOpt parser = new EasyOpt();

            // Create a new string parameter.
            var outputParam = new StringParameter(true, "FILE");
            // Create an option with the string parameter
            var output = OptionFactory.Create(
              false, // The option is not required
              "Do not send the results to stderr, but overwrite the specified file.", // Description for usage text
              outputParam // Option parameter
            );
            // Register to the parser. The option will have a short name -o and a long name --output
            parser.AddOption(output, 'o', "output");

            // Create a new option without a parameter
            var append = OptionFactory.Create(false, "(Used together with -o.) Do not overwrite but append");
            parser.AddOption(append, 'a', "append");

            // Create a new string parameter. The third argument defines its default value.
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

            // Description displayed in the usage text
            parser.UsageDescription = " time [options] command [arguments...]";

            try
            {
                // The actual argument parsing
                parser.Parse(args);
            }
            catch (EasyOptException)
            {
                // Some options were invalid, print usage text and exit
                Console.Write(parser.GetUsage());
                return;
            }

            // Find out whether --output option was defined
            bool isOutputPresent = output.IsPresent;
            // Retrieve value of the string parameter of --output option
            String outputValue = output.Value;

            // Find out whether --append option was defined
            bool appendValue = append.IsPresent;
            // Alternatively, for options without a parameter, you can equivalently use the following
            appendValue = append.Value;

            // Retrieve --format option parameter; if the option is not defined, retrieves the default value
            String formatValue = format.Value;

            // Values of other options can be retrieved in the same way
            bool helpValue = help.Value;
            bool portabilityValue = portability.Value;
            bool verboseValue = verbose.Value;
            bool quietValue = quiet.Value;
            bool versionValue = version.Value;

            // Get list of non-option arguments
            String[] arguments = parser.GetArguments();
        }
    }
}