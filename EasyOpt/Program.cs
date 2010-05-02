using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    public class Parser
    {
        private Dictionary<String, IOption> options;

        public Parser()
        {
            options = new Dictionary<string, IOption>();
        }

        public static Option<bool> createOption(bool isRequired, String usageText)
        {
            return new SimpleOption(isRequired, usageText);
        }

        public static Option<T> createOption<T>(bool isRequired, String usageText, Parameter<T> parameter)
        {
            return new ParameterOption<T>(isRequired, usageText, parameter);
        }

        public void addOption(IOption option, params String[] names)
        {
            // checkConfiguration(option); ?
            foreach (String name in names)
            {
                options.Add(name, option);
            }
        }

        public void addOption(IOption option, char shortName)
        {
            addOption(option, new String[] { shortName.ToString() });
        }

        public void addOption(IOption option, char shortName, String longName)
        {
            addOption(option, new String[] { shortName.ToString(), longName });
        }

        public void Parse(String[] args)
        {
            // throw new NotImplementedException();
        }

        public String[] GetArguments() // return non-option arguments
        {
            throw new NotImplementedException();
        }

        public String UsageText { get; set; }

        public String getUsage()
        {
            throw new NotImplementedException();
        }
    }

    public interface IConstraint<T>
    {
        bool IsValid(T parameterValue);
    }

    public interface IOption
    {
        bool IsPresent
        {
            set;
        }

        void SetValue(String value);
    }

    public abstract class Option<T> : IOption
    {
        public abstract T Value
        {
            get;
        }

        public abstract Parameter<T> Parameter
        {
            get;
        }

        public abstract void SetValue(String value);

        public bool IsRequired { get; set; }

        public String UsageText { get; set; }

        public bool IsPresent { get; set; }

        internal Option(bool isRequired, String usageText)
        {
            this.IsRequired = isRequired;
            this.UsageText = usageText;
        }
    }

    internal class SimpleOption : Option<bool>
    {
        public override bool Value
        {
            get { return IsPresent; }
        }

        public override Parameter<bool> Parameter
        {
            get { return null; }
        }

        public override void SetValue(string value)
        {
            throw new Exception(); // TODO: on the command line, a parameter was passed to an option without a parameter
        }

        public SimpleOption(bool isRequired, String usageText)
            : base(isRequired, usageText)
        { }
    }

    internal class ParameterOption<T> : Option<T>
    {
        private Parameter<T> parameter;

        public override Parameter<T> Parameter
        {
            get { return parameter; }
        }

        public override T Value
        {
            get { return this.parameter.Value; }
        }

        public override void SetValue(string value)
        {
            this.parameter.SetValue(value);
        }

        public ParameterOption(bool isRequired, String usageText, Parameter<T> parameter)
            : base(isRequired, usageText)
        {
            if (parameter == null)
                throw new Exception(); // TODO ?
            this.parameter = parameter;
        }
    }

    public abstract class Parameter<T>
    {
        abstract protected T convert(String parameterValue);

        private List<IConstraint<T>> constraints;

        public T DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public String UsageName { get; set; }

        private T value;
        private bool isValueValid = false; // indicates whether setValue() has been called
        public T Value
        {
            get
            {
                return isValueValid ? this.value : DefaultValue;
            }
        }
        internal void SetValue(String stringValue)
        {
            this.value = this.convert(stringValue);
            this.isValueValid = true;
        }

        protected Parameter(bool isRequired, String usageName, T defaultValue)
        {
            this.constraints = new List<IConstraint<T>>();
            this.IsRequired = isRequired;
            this.UsageName = usageName;
            this.DefaultValue = defaultValue;
        }

        public void AddConstraint(IConstraint<T> constraint)
        {
            constraints.Add(constraint);
        }

        public bool CheckConstraints()
        {
            T value = this.Value;
            foreach (var constraint in constraints)
            {
                if (!constraint.IsValid(value))
                {
                    return false;
                }
            }

            return true; // All constraints hold
        }
    }

    class StringParameter : Parameter<String>
    {
        public StringParameter(bool isRequired, String usageName) // defaultValue is ""
            : this(isRequired, usageName, "")
        { }
        public StringParameter(bool isRequired, String usageName, String defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        protected override string convert(string parameterValue)
        {
            return parameterValue;
        }
    }

    class IntParameter : Parameter<int>
    {
        public IntParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, 0)
        { }
        public IntParameter(bool isRequired, String usageName, int defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        protected override int convert(string parameterValue)
        {
            int result;
            if (int.TryParse(parameterValue, out result))
            {
                return result;
            }
            else
            {
                throw new Exception(); // TODO: replace by a parser-specific exception
            }
        }
    }

    class FloatParameter : Parameter<float>
    {
        public FloatParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, 0)
        { }
        public FloatParameter(bool isRequired, String usageName, float defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        protected override float convert(string parameterValue)
        {
            float result;
            if (float.TryParse(parameterValue, out result))
            {
                return result;
            }
            else
            {
                throw new Exception(); // TODO: replace by a parser-specific exception
            }
        }
    }

    class EnumParameter<T> : Parameter<T>
    {
        private String[] acceptedValues;

        private bool ignoreCase = true;
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }

        public EnumParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, default(T))
        { }
        public EnumParameter(bool isRequired, String usageName, T defaultValue)
            : base(isRequired, usageName, defaultValue)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new Exception(); // TODO
            acceptedValues = Enum.GetNames(type);
        }

        protected override T convert(string parameterValue)
        {
            StringComparer comparer = IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (!acceptedValues.Contains(parameterValue, comparer))
            {
                throw new Exception(""); // TODO
            }
            try
            {
                return (T)Enum.Parse(typeof(T), parameterValue, ignoreCase);
            }
            catch (ArgumentException e)
            {
                throw new Exception("", e); // TODO
            }
        }
    }

    sealed class LowerBoundConstraint : IConstraint<int>
    {
        private int limit;

        public LowerBoundConstraint(int limit)
        {
            this.limit = limit;
        }

        public bool IsValid(int parameter)
        {
            return parameter >= limit;
        }
    }

    sealed class UpperBoundConstraint : IConstraint<int>
    {
        private int limit;

        public UpperBoundConstraint(int limit)
        {
            this.limit = limit;
        }

        public bool IsValid(int parameter)
        {
            return parameter <= limit;
        }
    }

    sealed class StringEnumerationConstraint : IConstraint<String>
    {
        private IEnumerable<String> acceptedValues;
        private IEqualityComparer<String> comparer;

        public StringEnumerationConstraint(IEnumerable<String> acceptedValues, IEqualityComparer<String> comparer)
        {
            if (acceptedValues == null)
            {
                throw new Exception(); // TODO
            }
            this.acceptedValues = acceptedValues;
            this.comparer = comparer;
        }

        public StringEnumerationConstraint(IEnumerable<String> acceptedValues, bool ignoreCase)
            : this(
                acceptedValues,
                ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture
            )
        { }

        public bool IsValid(String parameter)
        {
            return acceptedValues.Contains(parameter, comparer);
        }
    }

    sealed class ExistingFileConstraint : IConstraint<String>
    {
        public bool IsValid(String parameter)
        {
            return System.IO.File.Exists(parameter);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();

            // GNU Time example:            
            var formatParam = new StringParameter(true, "FORMAT", "real %f\nuser %f\nsys %f\n");
            var format = Parser.createOption( // or new ParameterOption<String>(
                false,
                "Specify output format, possibly overriding the format specified in the environment variable TIME.",
                formatParam
            );
            p.addOption(format, 'f', "format");
            // these versions also work:
            // p.addOption(format, "f", "format");
            // p.addOption(format, new String[] { "f", "format" });

            var portability = Parser.createOption(false, "Use the portable output format");
            p.addOption(portability, 'p', "portability");

            var outputParam = new StringParameter(true, "FILE");
            var output = Parser.createOption(
                false,
                "Do not send the results to stderr, but overwrite the specified file.",
                outputParam
            );
            p.addOption(format, 'o', "output");

            var append = Parser.createOption(false, "(Used together with -o.) Do not overwrite but append");
            p.addOption(portability, 'a', "append");

            var verbose = Parser.createOption(false, "Give very verbose output about all the program knows about.");
            p.addOption(verbose, 'v', "verbose");

            var help = Parser.createOption(false, "Print a usage message on standard output and exit successfully.");
            //p.addOption(help, "help");

            var version = Parser.createOption(false, "Print version information on standard output, then exit successfully.");
            p.addOption(version, 'V', "version");

            p.UsageText = " time [options] command [arguments...]\n\n";

            p.Parse(args);

            // usage:
            String formatString = format.Value; // returns String
            bool isPortable = portability.Value; // returns boolean


            var enumParam = new EnumParameter<myEnum>(false, "");
            var enumOption = Parser.createOption(false, "", enumParam);
            p.addOption(enumOption, 'e');

            var stringParam = new StringParameter(false, "");
            var stringConstraint = new StringEnumerationConstraint(new String[] { "one", "two" }, true);
            stringParam.AddConstraint(stringConstraint);
        }
        public enum myEnum { a, b };

    }
}