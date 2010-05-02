using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Class representing an option parameter.
     * 
     * This class is used to specify an option's parameter, conversion method,
     * used to convert the parameter's string value to type T, constraints
     * and other relevant details.
     * 
     * Extend this class if you want to provide means to convert the parameter's value
     * from its command line string representation to another type T.
     * 
     * @param T Type of the parameter's value.
     *      The string representation on the command line is converted to this type.
     * @see EasyOpt.Parser.CreateOption< T >
     */
    public abstract class Parameter<T>
    {
        /**
         * Convert parameter's value from the command line string representation
         * to type T. 
         * Throw an exception if conversion fails.
         * @param parameterValue parameter's value as a string
         * @return parameter value converted to T
         */
        abstract protected T convert(String parameterValue);

        /**
         * List of constraints that must hold for the converted parameter's value
         */
        private List<IConstraint<T>> constraints;

        /**
         * Default value used if no value is specified on the command line.
         */
        public T DefaultValue { get; set; }

        /**
         * Indicates whether the corresponding Option requires specification
         * of parameter's value when the option is present.
         */
        public bool IsRequired { get; set; }

        /**
         * Name of the parameter used in usage text.
         * @see EasyOpt.Parser.GetUsage()
         */
        public String UsageName { get; set; }

        /**
         * Converted value of the parameter from the command line.
         */
        private T value;

        /**
         * Indicates whether SetValue() has been called.
         * @see EasyOpt.Parameter< T >.SetValue()
         */
        private bool isValueValid = false;

        /**
         * Value of the parameter.
         * If SetValue() has been called, contains the converted value,
         * otherwise contains the default value.
         * @see EasyOpt.Parameter< T >.DefaultValue
         * @see EasyOpt.Parameter< T >.SetValue()
         */
        public T Value
        {
            get
            {
                return isValueValid ? this.value : DefaultValue;
            }
        }

        /**
         * Set the parameter's value from the command line.
         * This method is intended to be called by Parser only.
         */
        internal void SetValue(String stringValue)
        {
            this.value = this.convert(stringValue);
            this.isValueValid = true;
        }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @param defaultValue Default value used when SetValue() hasn't been called.
         */
        protected Parameter(bool isRequired, String usageName, T defaultValue)
        {
            this.constraints = new List<IConstraint<T>>();
            this.IsRequired = isRequired;
            this.UsageName = usageName;
            this.DefaultValue = defaultValue;
        }

        /**
         * Add a constraint that must hold for the parameter's value.
         */
        public void AddConstraint(IConstraint<T> constraint)
        {
            constraints.Add(constraint);
        }

        /**
         * Return true if all constraints hold for the parameter's value,
         * false otherwise.
         */
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

    /**
     * String parameter. Perfoms no parameter's value conversion.
     */
    public class StringParameter : Parameter<String>
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

    public class IntParameter : Parameter<int>
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

    public class FloatParameter : Parameter<float>
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

    public class EnumParameter<T> : Parameter<T>
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
}