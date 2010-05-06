using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Exception thrown option parameter is invalid.
     */
    public class ParameterException : EasyOptException
    {
        /**
         * The original value of the parameter before attempt to convert it.
         */
        private string parameterValue;

        /**
         * The original value of the parameter before attempt to convert it.
         */
        public string ParameterValue
        {
            get
            {
                return parameterValue;
            }
        }

        /**
         * Reference to the parameter object where the exception was thrown.
         * Stored as an Object in order to be able to handle all generic 
         * versions of Parameter< T >.
         */
        private Object parameter;

        /*
         * Reference to the parameter object where the exception was thrown.
         */
        public Object Parameter
        {
            get { return parameter; }
        }

        /**
         * Gets a message that describes the current exception
         */
        public override string Message
        {
            get
            {
                string message = String.Format(
                    "Invalid parameter value \"{0}\"",
                    parameterValue
                );
                return message;
            }
        }

        public ParameterException(string parameterValue, Object parameter)
            : base()
        {
            this.parameterValue = parameterValue;
            this.parameter = parameter;
        }

        public ParameterException(string parameterValue, Object parameter, Exception innerException)
            : base("", innerException)
        {
            this.parameterValue = parameterValue;
            this.parameter = parameter;
        }
    }

    /**
     * Exception thrown when conversion of parameter from string to its
     * respective typed value fails.
     */
    public class ParameterConversionException : ParameterException
    {
        public ParameterConversionException(string parameterValue, Object parameter)
            : base(parameterValue, parameter)
        { }

        public ParameterConversionException(string parameterValue, Object parameter, Exception innerException)
            : base(parameterValue, parameter, innerException)
        { }
    }

    /**
     * Exception thrown when parameter constraints don't hold.
     */
    public class ConstraintException : ParameterException
    {
        public ConstraintException(string parameterValue, Object parameter)
            : base(parameterValue, parameter)
        { }

        public ConstraintException(string parameterValue, Object parameter, Exception innerException)
            : base(parameterValue, parameter, innerException)
        { }
    }

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
         * Throw ParameterConversionException if conversion fails.
         * @param parameterValue parameter's value as a string
         * @return parameter value converted to T
         */
        abstract protected T convert(String parameterValue);

        /**
         * List of constraints that must hold for the converted parameter's value.
         * Invariant: All constraints are not null
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
         * If SetValue() has been called, returns the converted value,
         * otherwise returns the default value.
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
         * @param stringValue Parameter value as string. Must not be null.
         * @throw ArgumentNullException
         * @throw ParameterException
         */
        internal void SetValue(String stringValue)
        {
            if (stringValue == null)
            {
                throw new ArgumentNullException("stringValue");
            }
            this.value = this.convert(stringValue);
            if (!this.CheckConstraints(this.value))
            {
                throw new ConstraintException(stringValue, this);
            }
            this.isValueValid = true;
        }

        /**
         * @param isRequired Indicates whether the corresponding Option requires the parameter
         *      when the option itself is present.
         * @param usageName Name of the parameter used in the usage text.
         * @param defaultValue Default value used when the parameter's value hasn't been set explicitly
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
         * The constraint is applied to converted paremeter value (to type T)
         * @param constraint Must not be null.
         * @throw ArgumentNullException
         */
        public void AddConstraint(IConstraint<T> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            constraints.Add(constraint);
        }

        /**
         * Return true if all constraints hold for the parameter's value,
         * false otherwise.
         */
        protected bool CheckConstraints(T value)
        {
            foreach (var constraint in constraints)
            {
                if (!constraint.IsValid(value))
                {
                    return false;
                }
            }
            // All constraints hold
            return true;
        }
    }

    /**
     * String parameter. Perfoms no parameter value conversion.
     */
    public class StringParameter : Parameter<String>
    {
        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         */
        public StringParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, "")
        { }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @param defaultValue Default value used if no value is specified on the command line.
         */
        public StringParameter(bool isRequired, String usageName, String defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        /**
         * Returns the string parameter without change.
         * @see EasyOpt.Parameter< T >.convert()
         */
        protected override string convert(string parameterValue)
        {
            return parameterValue;
        }
    }

    /**
     * Integer parameter. Converts parameter value to int.
     */
    public class IntParameter : Parameter<int>
    {
        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         */
        public IntParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, 0)
        { }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @param defaultValue Default value used if no value is specified on the command line.
         */
        public IntParameter(bool isRequired, String usageName, int defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        /**
         * Convert parameter to an integer value. 
         * @throw ParameterConversionException thrown when conversion fails.
         * @see EasyOpt.Parameter< T >.convert()
         */
        protected override int convert(string parameterValue)
        {
            int result;
            if (int.TryParse(parameterValue, out result))
            {
                return result;
            }
            else
            {
                throw new ParameterConversionException(parameterValue, this);
            }
        }
    }

    /**
     * Flaot parameter. Converts parameter value to float.
     */
    public class FloatParameter : Parameter<float>
    {
        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         */
        public FloatParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, 0)
        { }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @param defaultValue Default value used if no value is specified on the command line.
         */
        public FloatParameter(bool isRequired, String usageName, float defaultValue)
            : base(isRequired, usageName, defaultValue)
        { }

        /**
         * Convert parameter to a float value. 
         * @throw ParameterConversionException thrown when conversion fails.
         * @see EasyOpt.Parameter< T >.convert()
         */
        protected override float convert(string parameterValue)
        {
            float result;
            if (float.TryParse(parameterValue, out result))
            {
                return result;
            }
            else
            {
                throw new ParameterConversionException(parameterValue, this);
            }
        }
    }

    /**
     * Exception thrown when type parameter T in EnumParameter< T >
     * is not an enum.
     */
    public class EnumParameterException : EasyOptException
    {
        /**
         * Type given instead of an enum type
         */
        Type type;

        /**
         * Type given instead of an enum type
         */
        public Type Type
        {
            get { return type; }
        }

        /**
         * Gets a message that describes the current exception
         */
        public override string Message
        {
            get
            {
                string message = String.Format(
                    "An enum type expected as type parameter for EnumParameter, {0} given",
                    type.Name
                );
                return message;
            }
        }

        /**
         * @param type Type given instead of an enum type
         */
        public EnumParameterException(Type type)
        {
            this.type = type;
        }
    }

    /**
     * Enum type parameter.
     * Converts parameter to a value of an arbitrary enum type T.
     * @param T enum type to which the parameter is converted
     */
    public class EnumParameter<T> : Parameter<T>
    {
        /**
         * List of accepted values - names of enum members.
         */
        private String[] acceptedValues;

        /**
         * If true, conversion to enum value is case-insensitive, otherwise it is case-sensitive.
         */
        private bool ignoreCase = true;

        /**
         * If true, conversion to enum value is case-insensitive, otherwise it is case-sensitive.
         * Default value is true.
         */
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @throw EnumParameterException thrown when T is not an enum type
         */
        public EnumParameter(bool isRequired, String usageName)
            : this(isRequired, usageName, default(T))
        { }

        /**
         * @param isRequired Indicates whether the corresponding Option requires specification
         *      of parameter's value when the option is present.
         * @param usageName Name of the parameter used in usage text.
         * @param defaultValue Default value used if no value is specified on the command line.
         * @throw EnumParameterException thrown when T is not an enum type
         */
        public EnumParameter(bool isRequired, String usageName, T defaultValue)
            : base(isRequired, usageName, defaultValue)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new EnumParameterException(type);
            }
            acceptedValues = Enum.GetNames(type);
        }

        /**
         * Convert parameter to enum type T.
         * Takes parameter as the name of an enumerated constant and converts it
         * to an equivalent enumerated object. 
         * Only string names are accepted, conversion of numeric values or 
         * more enumerated constants fails.
         * @throw ParameterConversionException thrown when conversion fails.
         * @see EasyOpt.Parameter< T >.convert()
         */
        protected override T convert(string parameterValue)
        {
            /*
             * Compare to the list of enum members as strings
             * -> This prevents conversion of corresponding numeric values
             *  or more enum constants by Enum.Parse() to an enum type and 
             *  only correct string names are converted successfully
             */
            StringComparer comparer = IgnoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            if (!acceptedValues.Contains(parameterValue, comparer))
            {
                throw new ParameterConversionException(parameterValue, this);
            }

            try
            {
                return (T)Enum.Parse(typeof(T), parameterValue, ignoreCase);
            }
            catch (ArgumentException e)
            {
                throw new ParameterConversionException(parameterValue, this, e);
            }
        }
    }
}