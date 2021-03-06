﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyOptLibrary;

/**
 * Use case of EasyOpt: selected GNU touch command line arguments.
 */
namespace UseCaseTouch
{
    class TimeConstraint : IConstraint<String>
    {
        public bool IsValid(String parameter)
        {
            var r = new System.Text.RegularExpressions.Regex(@"^((\d\d)?\d\d)?(\d){8}(\.\d\d)?$");
            return r.IsMatch(parameter);
        }
    }

    class Program
    {
        enum Time
        {
            Access, Atime, Use
        }

        static void Main(string[] args)
        {
            EasyOpt parser = new EasyOpt();

            var accessTime = OptionFactory.Create(false, "change only the access time");
            parser.AddOption(accessTime, 'a');

            var noCreate = OptionFactory.Create(false, "do not create any files");
            parser.AddOption(noCreate, 'c', "no-create");

            var dateParam = new StringParameter(true, "STRING");
            var date = OptionFactory.Create(false, "parse STRING and use it instead of current time", dateParam);
            parser.AddOption(date, 'd', "date");

            var referenceParam = new StringParameter(true, "FILE");
            referenceParam.AddConstraint(new ExistingFileConstraint());
            var reference = OptionFactory.Create(false, "use this file's times instead of current time", referenceParam);
            parser.AddOption(reference, 'r', "reference");

            var stampParam = new StringParameter(true, "STAMP");
            stampParam.AddConstraint(new TimeConstraint());
            var stamp = OptionFactory.Create(false, "use [[CC]YY]MMDDhhmm[.ss] instead of current time", stampParam);
            parser.AddOption(stamp, 't');

            var timeParam = new EnumParameter<Time>(true, "WORD");
            var time = OptionFactory.Create(false, "change the specified time: WORD is access, atime, or use", timeParam);
            parser.AddOption(time, "time");

            var version = OptionFactory.Create(false, "output version information and exit");
            parser.AddOption(version, "version");

            parser.UsageDescription = " touch [OPTION]... [FILE]...";

            try
            {
                parser.Parse(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}:\n{1}\n\n", e.GetType().Name, e.Message);
            }

            Console.WriteLine("{0}: {1}", "a", accessTime.Value);
            Console.WriteLine("{0}: {1}", "no-create", noCreate.Value);
            Console.WriteLine("{0}: {1}", "date", date.Value);
            Console.WriteLine("{0}: {1}", "reference", reference.Value);
            Console.WriteLine("{0}: {1}", "t", stamp.Value);
            Console.WriteLine("{0}: {1}", "time", time.Value);
            Console.WriteLine("{0}: {1}", "version", version.Value);

            var arguments = parser.GetArguments();
            Console.WriteLine("\nArguments: {0}", String.Join(" ", (String[]) arguments));
            Console.WriteLine("\nUsage text:");
            Console.WriteLine(parser.GetUsage());
        }
    }
}
