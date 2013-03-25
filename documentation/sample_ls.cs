using System;
using EasyOptLibrary;

namespace EasyOptSampleLs
{
    enum SizeUnit
    {
        kB, MB, GB, TB, PB, EB, ZB, YB
    }

    class Size
    {
        public SizeUnit Unit { get; set; }
        public int Value { get; set; }
        public Size(int value, SizeUnit unit)
        {
            this.Value = value;
            this.Unit = unit;
        }
    }

    // Custom parameter class that converts option parameter to type Size
    class SizeParameter : Parameter<Size>
    {
        public SizeParameter(bool isRequired, String usageName)
            : base(isRequired, usageName, default(Size))
        { }

        // Custom parameter has to override this abstract method
        // It defines a conversion function from string parameter value to type Size
        protected override Size convert(string parameterValue)
        {
            var match = System.Text.RegularExpressions.Regex.Match(parameterValue, @"^(\w+)(\d+)$");
            if (!match.Success)
            {
                // In case that conversion fails, throw an exception
                throw new ParameterConversionException(parameterValue, this);
            }
            try
            {
                SizeUnit unit = (SizeUnit)Enum.Parse(typeof(SizeUnit), match.Groups[1].Value, true);
                int value = Int32.Parse(match.Groups[2].Value);
                return new Size(value, unit);
            }
            catch (Exception)
            {
                throw new ParameterConversionException(parameterValue, this);
            }
        }
    }

    // This enum type will be used in conjuction with EnumParameter
    enum Color
    {
        Never, Always, Auto
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create a new parser instance
            EasyOpt parser = new EasyOpt();

            // Create options without a parameter
            var all = OptionFactory.Create(false, "do not ignore entries starting with .");
            parser.AddOption(all, 'a', "all");

            var author = OptionFactory.Create(false, "with -l, print the author or each file");
            parser.AddOption(author, "author");

            // Create a new option with the custom parameter type defined earlier
            var blockSizeParam = new SizeParameter(true, "SIZE");
            // Factory method Create() infers the proper option type Option<Size> from blockSizeParam type
            var blockSize = OptionFactory.Create(
                false,
                "use SIZE-byte blocks; SIZE  may  be one of following:" +
                " kB1000, KB1024, MB1000 and so on for G, T, P, E, Z, Y.",
                blockSizeParam
            );
            parser.AddOption(blockSize, "block-size");

            // With EnumParameter<T> you can easily define range of accepted values
            // and EnumParameter will convert parameter to the specified enum type.
            var colorParam = new EnumParameter<Color>(false, "WHEN", Color.Auto);
            // You can set whether the conversion is case-sensitive (default is case-insensitive)
            colorParam.IgnoreCase = true;
            var color = OptionFactory.Create(
                false,
                "control whether color is used to distinguish file types. " +
                "WHEN may be 'never', 'always' or 'auto'",
                colorParam
            );
            parser.AddOption(color, "color");

            // Create an option with a string parameter
            var formatParam = new StringParameter(true, "WORD");
            var format = OptionFactory.Create(
                false,
                "across  -x, commas -m, horizontal -x, long -l, single-column -1, verbose -l, vertical -C",
                formatParam
            );
            parser.AddOption(format, "format");

            // Create an option with a string parameter
            var quotingStyleParam = new StringParameter(true, "WORD");
            var quotingStyles = new String[] { "literal", "locale", "shell", "shell-always", "c", "escape" };
            // StringEnumerationConstraint restricts the range of parameter values to the defined list of values
            quotingStyleParam.AddConstraint(new StringEnumerationConstraint(quotingStyles, true));
            var quotingStyle = OptionFactory.Create(
                false,
                "use quoting style WORD for entry names: literal, locale, shell, shell-always, c, escape",
                quotingStyleParam
            );
            parser.AddOption(quotingStyle, "quoting-style");

            // Create an option with int parameter
            var tabsizeParam = new IntParameter(true, "COLS", 8);
            // Parameter value must be non-negative
            tabsizeParam.AddConstraint(new LowerBoundConstraint(0));
            var tabsize = OptionFactory.Create(false, "assume tab stops at each COLS instead of 8", tabsizeParam);
            parser.AddOption(tabsize, 'T', "tabsize");

            // Description displayed in the usage text
            parser.UsageDescription = " ls [OPTION]... [FILE]...";

            try
            {
                // The actual argument parsing
                parser.Parse(args);
            }
            catch (EasyOptException e)
            {
                // Some options were invalid (e.g. a parameter constraint or conversion failed)
                // Print usage text and exit
                Console.Write(parser.GetUsage());
                return;
            }

            // Retrieve some option values
            // Retrieve whether --all option is on
            bool allValue = all.Value;
            // Retrieve --block-size parameter converted to Size
            Size blockSizeValue = blockSize.Value;
            // Retrieve --color parameter converted to Color enum type
            Color colorValue = color.Value;
            // Retrieve --quoting-style value; the value must me one of the strings in quotingStyles or default value ("")
            String quotingValue = quotingStyle.Value;

            // Get list of non-option arguments
            String[] arguments = parser.GetArguments();
        }
    }
}