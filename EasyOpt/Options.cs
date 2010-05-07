using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Auxiliary option interface that enables work
     * with all generic versions of Option< T > as with one type
     */
    internal interface IOption
    {
        /**
         * True, if the option is within command line arguments, false otherwise.
         */
        bool IsPresent { get; set; }

        /**
         * True, if the option has a required parameter, false otherwise.
         */
        bool IsParameterRequired { get; }

        /**
         * True, if the option is required to appear on the commnad line
         */
        bool IsRequired { get; }

        /**
         * Returns true if the option has a parameter, false otherwise.
         */
        bool HasParameter { get; }

        /**
         * Description of option usage.
         */
        string UsageText { get; }

        /**
         * Returns usage name of the parameter, or an empty string if
         * the option has no parameter.
         */
        string ParameterUsageName { get; }

        /**
         * Sets parameter value.
         */
        void SetValue(String value);
    }

    /**
     * Class representing a command line option.
     * 
     * Instances of this class are created solely by OptionFactory.
     * 
     * The class is generic in order to provide access to typed option
     * parameter and its value.
     * Options without a parameter are treated as boolean
     * (the value is true when the option is present on the command
     * line, false otherwise).
     * 
     * For example of usage see OptionFactory.
     */
    public abstract class Option<T> : IOption
    {
        /**
         * If the option has a parameter, get its value, otherwise
         * return whether the option is present on the command line
         */
        public abstract T Value { get; }

        /**
         * Return reference to the corresponding parameter object.
         * If the option has no parameter, returns null.
         */
        public abstract Parameter<T> Parameter { get; }

        /**
         * Returns true if the option has a parameter, false otherwise.
         */
        public abstract bool HasParameter { get; }

        /**
         * True, if the option has a required parameter, false otherwise.
         */
        public abstract bool IsParameterRequired { get; }

        /**
         * Returns usage name of the parameter, or an empty string if
         * the option has no parameter.
         */
        public abstract string ParameterUsageName { get; }

        /**
         * True, if the option is required to appear on the commnad line
         */
        public bool IsRequired { get; set; }

        /**
         * Description of option usage.
         */
        public String UsageText { get; set; }

        /**
         * True, if the option is within command line arguments, false otherwise.
         */
        private bool isPresent = false;

        /**
         * True, if the option is within command line arguments, false otherwise.
         */
        public bool IsPresent
        {
            get { return isPresent; }
        }

        /**
         * Sets IsPresent value. 
         * Implemented only for internal interface IOption in order
         * to hide the setter from the user.
         */
        bool IOption.IsPresent
        {
            get { return isPresent; }
            set { isPresent = value; }
        }

        /**
         * Internal implementation of SetValue() method.
         */
        protected abstract void setValue(String value);

        /**
         * Sets parameter value.
         * Implemented only for internal interface IOption in order
         * to hide the setter from the user.
         * @throw ParameterConversionException
         * @throw ForbiddenParameterException
         */
        void IOption.SetValue(String value)
        {
            this.setValue(value);
        }

        /**
         * This class should not be extended by the user thus internal constructor.
         * @param isRequired true, if the option is required to appear on the commnad line, false otherwise
         * @param usageText description of option usage
         */
        internal Option(bool isRequired, String usageText)
        {
            this.IsRequired = isRequired;
            this.UsageText = usageText;
        }
    }

    /**
     * Class representing an option without any parameter.
     */
    internal class SimpleOption : Option<bool>
    {

        /**
         * Return whether the option is present on the command line
         * (i.e. the same as SimpleOption.IsPresent)
         */
        public override bool Value
        {
            get { return IsPresent; }
        }

        /**
         * Returns null since SimpleOption has no parameter
         */
        public override Parameter<bool> Parameter
        {
            get { return null; }
        }

        /**
         * Returns false since SimpleOption has no parameter
         */
        public override bool HasParameter
        {
            get { return false; }
        }

        /**
         * Setter for parameter value.
         * This method shouldn't be called since SimpleOption has no parameter.
         * @throw ForbiddenParameterOption
         */
        protected override void setValue(string value)
        {
            throw new ForbiddenParameterException();
        }

        /**
         * Retruen false, since SimpleOption has no parameter.
         */
        public override bool IsParameterRequired
        {
            get { return false; }
        }

        /**
         * Returns an empty string since SimpleOption has no parameter.
         */
        public override string ParameterUsageName
        {
            get { return ""; }
        }

        /**
         * @param isRequired indicates whether the option is required to appear on the commnad line
         * @param usageText description of option usage.
         */
        public SimpleOption(bool isRequired, String usageText)
            : base(isRequired, usageText)
        { }
    }

    /**
     * Class representing an option with parameter of type T.
     */
    internal class ParameterOption<T> : Option<T>
    {
        /**
         * Associated parameter object.
         * Invariant: not null
         */
        private Parameter<T> parameter;

        /**
         * Gets associated parameter object.
         */
        public override Parameter<T> Parameter
        {
            get { return parameter; }
        }

        /**
         * Gets parameter value.
         */
        public override T Value
        {
            get { return this.parameter.Value; }
        }

        /**
         * Returns true since ParameterOption has always an option attached.
         */
        public override bool HasParameter
        {
            get { return true; }
        }

        /**
         * Sets value of the parameter on the command line.
         * @throw ParameterConversionException
         */
        protected override void setValue(string value)
        {
            this.parameter.SetValue(value);
        }

        /**
         * Gets true, if the parameter is required.
         */
        public override bool IsParameterRequired
        {
            get { return parameter.IsRequired; }
        }

        /**
         * Gets usage name of the parameter.
         */
        public override string ParameterUsageName
        {
            get { return parameter.UsageName; }
        }

        /**
         * Creates an option with parameter with value of type T.
         * @param isRequired indicates whether the option is required to appear on the commnad line
         * @param usageText description of option usage.
         * @param parameter object represention the option's parameter; must not be null
         * @throw ArgumentNullException thrown when parameter is null
         */
        public ParameterOption(bool isRequired, String usageText, Parameter<T> parameter)
            : base(isRequired, usageText)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            this.parameter = parameter;
        }
    }

    /**
     * Class thrown when a parameter value is specified for a SimpleOption.
     * SimpleOption does not have a parameter 
     */
    public class ForbiddenParameterException : EasyOptException
    { }

    /**
     * Interface for container of option objects.
     */
    internal interface IOptionContainer
    {
        /**
         * Add an option to the container using names as its
         * option's synonymous option names.
         * @param option Option object
         * @param names List of synonymous names for the option.
         */
        void Add(IOption option, String[] names);

        /**
         * Return option by its option name
         * @param name option name
         */
        IOption FindByName(string name);

        /**
         * Returns true if the container contains an option with the specified name,
         * false otherwise.
         * @param name option name
         */
        bool ContainsName(string name);

        /**
         * Returns collection of the first option synonym for all options.
         */
        IEnumerable<string> ListUniqueNames();

        /**
         * Returns list of synonyms for the option's first synonymous name.
         */
        string[] FindSynonymsByName(string name);
    }

    /**
     * Container for option objects.
     * Stores both options as IOption objects and lists of synonyms,
     * both can be accessed by name.
     */
    internal class OptionContainer : IOptionContainer
    {
        /**
         * Dictionary containing all IOption objects passed by
         * CommandLine.AddOption() indexed by all option synonymous names.
         */
        private Dictionary<String, IOption> options;

        /**
         * Dictionary containing lists of synonymous names of options
         * indexed by the first synonym in the list.
         */
        private Dictionary<String, String[]> names;

        /**
         * Initialize data containers
         */
        public OptionContainer()
        {
            this.options = new Dictionary<string, IOption>();
            this.names = new Dictionary<string, string[]>();
        }

        /**
         * Returns collection of the first option synonym for all options.
         */
        public IEnumerable<String> ListUniqueNames()
        {
            return this.names.Keys;
        }

        /**
         * Add an option to the container using names as its
         * option's synonymous option names.
         * @param option Option object
         * @param names List of synonymous names for the option.
         * @throw DuplicateOptionNameException
         * @throw InvalidNameException
         */
        public void Add(IOption option, String[] names)
        {
            if (names == null || names.Length == 0)
            {
                throw new InvalidNameException("An Option must have at least one name.");
            }

            // TODO checkConfiguration(option);
            foreach (String name in names)
            {
                Token.CheckName(name);

                if (this.ContainsName(name))
                {
                    throw new DuplicateOptionNameException (
                        "Option name: " + 
                        name + 
                        " is already assigned," +
                        " please choose another name.");
                }

                this.options.Add(name, option);
            }

            this.names.Add(names[0], names);
        }

        /**
         * Returns list of synonyms for the option's first synonymous name.
         * This methods expects name to be contained in the container.
         */
        public string[] FindSynonymsByName(string name)
        {
            return this.names[name];
        }

        /**
         * Return option by its option name
         * This methods expects option identified by name to be contained in the container.
         * Use ContainsName() to check for presence of name in the container.
         * @param name option name
         */
        public IOption FindByName(string name)
        {
            return this.options[name];
        }

        /**
         * Returns true if the container contains an option with the specified name,
         * false otherwise.
         * @param name option name
         */
        public bool ContainsName(string name)
        {
            return this.options.ContainsKey(name);
        }
    }
}

