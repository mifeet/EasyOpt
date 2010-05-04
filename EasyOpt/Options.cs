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
        bool IsPresent
        {
            get;
            set;
        }

        bool IsParameterRequired
        {
            get;
        }

        bool IsRequired
        {
            get;
        }

        bool HasParameter
        {
            get;
        }

        string UsageText
        {
            get;
        }

        string ParameterUsageName
        {
            get;
        }

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
        public abstract T Value
        {
            get;
        }

        /**
         * Return reference to the corresponding parameter object.
         * If the option has no parameter, returns null.
         */
        public abstract Parameter<T> Parameter
        {
            get;
        }

        /**
         * Returns true if the option has a parameter, false otherwise.
         */
        public abstract bool HasParameter { get; }

        /**
         * True, if the option has a required parameter, false otherwise.
         */
        public abstract bool IsParameterRequired
        {
            get;
        }

        /**
         * Returns usage name of the parameter, or an empty string if
         * the option has no parameter.
         */
        public abstract string ParameterUsageName
        {
            get;
        }

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
         */
        protected override void setValue(string value)
        {
            // TODO: perhaps it would be better to throw the exception in the parser after all 
            // because the parser has information about the option name
            throw new ForbiddenParameterException(""); // TODO: on the command line, a parameter was passed to an option without a parameter
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
     * Class thrown when a value is specified for a SimpleOption.
     * SimpleOption does not have a parameter 
     * 
     */
    public class ForbiddenParameterException : EasyOptException
    {
        public ForbiddenParameterException(string message)
            : base(message)
        { }

        public ForbiddenParameterException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    internal interface IOptionContainer
    {
        void Add(IOption option, params String[] names);

        void Add(IOption option, char shortName);

        void Add(IOption option, char shortName, String longName);

        IOption FindByName(string name);

        bool ContainsName(string name);

        List<string> ListUniqueNames();

        string[] FindSynonymsByName(string name);
    }

    internal class OptionContainer : IOptionContainer
    {
        private Dictionary<String, IOption> options;

        private Dictionary<String, String[]> names;

        public OptionContainer()
        {
            this.options = new Dictionary<string, IOption>();
            this.names = new Dictionary<string, string[]>();
        }

        public List<string> ListUniqueNames()
        {
            return new List<string>(this.names.Keys);

        }

        public string[] FindSynonymsByName(string name)
        {
            return this.names[name];
        }

        public void Add(IOption option, params String[] names)
        {
            // checkConfiguration(option); ?
            foreach (String name in names)
            {
                this.options.Add(name, option);
            }

            this.names.Add(names[0], names);
        }

        public void Add(IOption option, char shortName)
        {
            Add(option, new String[] { shortName.ToString() });
        }

        public void Add(IOption option, char shortName, String longName)
        {
            Add(option, new String[] { shortName.ToString(), longName });
        }

        public IOption FindByName(string name)
        {
            return this.options[name];
        }

        public bool ContainsName(string name)
        {
            return this.options.ContainsKey(name);
        }
    }

}