using System;
using EasyOptLibrary;

namespace EasyOptSampleTouch
{
    // A custom constraint
    class TimeConstraint : IConstraint<String>
    {
        public bool IsValid(String parameter)
        {
            var r = new System.Text.RegularExpressions.Regex(@"^((\d\d)?\d\d)?(\d){8}(\.\d\d)?$");
            return r.IsMatch(parameter);
        }
    }

    enum Time
    {
        Access, Atime, Use
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create a new parser instance
            EasyOpt parser = new EasyOpt();

            // Add some options
            var accessTime = OptionFactory.Create(false, "change only the access time");
            parser.AddOption(accessTime, 'a');

            var noCreate = OptionFactory.Create(false, "do not create any files");
            parser.AddOption(noCreate, 'c', "no-create");

            var dateParam = new StringParameter(true, "STRING");
            var date = OptionFactory.Create(false, "parse STRING and use it instead of current time", dateParam);
            parser.AddOption(date, 'd', "date");

            var referenceParam = new StringParameter(true, "FILE");
            // ExistingFileConstraint ensures that the parameter value is a valid path to an existing file
            referenceParam.AddConstraint(new ExistingFileConstraint());
            var reference = OptionFactory.Create(false, "use this file's times instead of current time", referenceParam);
            parser.AddOption(reference, 'r', "reference");

            var stampParam = new StringParameter(true, "STAMP");
            // Apply the custom constraint defined earlier to the parameter
            stampParam.AddConstraint(new TimeConstraint());
            var stamp = OptionFactory.Create(false, "use [[CC]YY]MMDDhhmm[.ss] instead of current time", stampParam);
            parser.AddOption(stamp, 't');

            // Use EnumParameter to restrict the range of parameter values and convert the value to Time
            var timeParam = new EnumParameter<Time>(true, "WORD");
            var time = OptionFactory.Create(false, "change the specified time: WORD is access, atime, or use", timeParam);
            parser.AddOption(time, "time");

            var version = OptionFactory.Create(false, "output version information and exit");
            parser.AddOption(version, "version");

            // Description displayed in the usage text
            parser.UsageDescription = " touch [OPTION]... [FILE]...";

            try
            {
                // The actual argument parsing
                parser.Parse(args);
            }
            catch (EasyOptException e)
            {
                // Some options were invalid, print usage text and exit
                Console.Write(parser.GetUsage());
                return;
            }

            // Retrieve some option values
            bool accessTimeValue = accessTime.Value;
            String dateValue = date.Value;
            String referenceValue = reference.Value;
            String stampValue = stamp.Value;
            Time timeValue = time.Value;

            // Get list of non-option arguments
            String[] arguments = parser.GetArguments();
        }
    }
}