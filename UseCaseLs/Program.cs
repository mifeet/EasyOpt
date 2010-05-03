using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyOpt;

/**
 * Use case of EasyOpt: selected GNU ls command line arguments.
 */
namespace UseCaseLs
{
    enum SizeUnit {
       kB, MB, GB, TB, PB, EB, ZB, YB
    }

    class Size
    {
        private SizeUnit unit;
        public SizeUnit Unit
        {
            get { return unit; }
        }

        private int value;
        public int Value
        {
            get { return value; }
        }

        public Size(int value, SizeUnit unit)
        {
            this.value = value;
            this.unit = unit;
        }
    }

    class SizeParameter : Parameter<Size>
    {
        public SizeParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, default(Size))
        { }
        public SizeParameter(bool isRequired, String usageName, Size defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        public Size x(String s) { return convert(s); }

        protected override Size convert(string parameterValue)
        {
            String[] valuePair = parameterValue.Split(new char[] { ' ' });
            if (valuePair.Length != 2)
            {
                throw new Exception("");
            }

            SizeUnit unit;
            try
            {
                unit = (SizeUnit) Enum.Parse(typeof(SizeUnit), valuePair[0], true);
            }
            catch (ArgumentException e)
            {
                throw new Exception("", e); // TODO
            }

            int value = 1;
            String[] factors = valuePair[1].Split(new char[] { '*' });
            foreach (String factor in factors)
            {
                int factorValue;
                if (Int32.TryParse(factor, out factorValue))
                {
                    value *= factorValue;
                }
                else
                {
                    throw new Exception(""); // TODO
                }
            }

            return new Size(value, unit);
        }
    }

    class Program
    {
        enum Color
        {
            Never, Always, Auto
        }

        static void Main(string[] args)
        {
            CommandLine parser = new CommandLine(args);

            var all = OptionFactory.Create(false, "do not ignore entries starting with .");
            parser.AddOption(all, 'a', "all");

            var author = OptionFactory.Create(false, "with -l, print the author or each file");
            parser.AddOption(author, "author");

            var blockSizeParam = new SizeParameter(true, "SIZE", new Size(0, SizeUnit.kB));
            var blockSize = OptionFactory.Create(
                false,
                "use SIZE-byte blocks; SIZE  may  be one of following: kB 1000, K 1024, MB 1000*1000,"+
                "M 1024*1024, and so on for G, T, P, E, Z, Y.",
                blockSizeParam
            );
            parser.AddOption(blockSize, "block-size");

            var colorParam = new EnumParameter<Color>(false, "WHEN", Color.Auto);
            colorParam.IgnoreCase = true;
            var color = OptionFactory.Create(
                false,
                "control whether color is used to distinguish file types. " +
                "WHEN may be 'never', 'always' or 'auto'",
                colorParam
            );
            parser.AddOption(color);

            var formatParam = new StringParameter(true, "WORD");
            var format = OptionFactory.Create(
                false,
                "across  -x, commas -m, horizontal -x, long -l, single-column -1, verbose -l, vertical -C",
                formatParam
            );
            parser.AddOption(format, "format");

            var quotingStyleParam = new StringParameter(true, "WORD");
            var quotingStyles = new String[] { "literal", "locale", "shell", "shell-always", "c", "escape" };
            quotingStyleParam.AddConstraint(new StringEnumerationConstraint(quotingStyles, true));
            var quotingStyle = OptionFactory.Create(
                false,
                "use quoting style WORD for entry names: literal, locale, shell, shell-always, c, escape",
                quotingStyleParam
            );
            parser.AddOption(quotingStyle, "quoting-style");

            var tabsizeParam = new IntParameter(true, "COLS", 8);
            tabsizeParam.AddConstraint(new LowerBoundConstraint(0));
            var tabsize = OptionFactory.Create(false, "assume tab stops at each COLS instead of 8", tabsizeParam);
            parser.AddOption(tabsize, 'T', "tabsize");

            parser.UsageText = " ls [OPTION]... [FILE]...\n\n";

            try
            {
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}:\n{1}\n\n", e.GetType().Name, e.Message);
            }

            Console.WriteLine("{0}: {1}", "all", all.Value);
            Console.WriteLine("{0}: {1}", "author", author.Value);
            Console.WriteLine("{0}: {1} {2}", "block-size", blockSize.Value.Value, blockSize.Value.Unit);
            Console.WriteLine("{0}: {1}", "color", color.Value);
            Console.WriteLine("{0}: {1}", "format", format.Value);
            Console.WriteLine("{0}: {1}", "quoting-style", quotingStyle.Value);
            Console.WriteLine("{0}: {1}", "tabsize", tabsize.Value);

            var arguments = parser.GetArguments();
            Console.WriteLine("\nArguments: {0}", String.Join(" ", (String[]) arguments));
            Console.WriteLine("\nUsage text:");
            Console.WriteLine(parser.GetUsage());
            Console.ReadLine();
        }
    }
}