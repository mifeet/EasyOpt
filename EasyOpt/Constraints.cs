using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOptLibrary
{
    /**
     * Interface of a parameter constraint.
     * @param T Type to which the constraint can be applied
     * @see EasyOpt.Parameter< T >.AddConstraint()
     */
    public interface IConstraint<T>
    {
        /**
         * Checks whether the constraint holds for parameter's argument.
         * @param parameterValue value of a parameter's argument converted to type T
         */
        bool IsValid(T parameterValue);
    }

    /**
     * Parameter constraint validating lower bound for its argument.
     */
    public sealed class LowerBoundConstraint : IConstraint<int>
    {
        /**
         * Lower bound value
         */
        private int limit;

        /**
         * @param limit lower bound value
         */
        public LowerBoundConstraint(int limit)
        {
            this.limit = limit;
        }

        /**
         * Check whether parameter is grater or equal to the specified
         * lower bound.
         * @param parameter value of a Parameter's argument
         */
        public bool IsValid(int parameter)
        {
            return parameter >= limit;
        }
    }

    /**
     * Parameter constraint validating upper bound for its argument.
     */
    public sealed class UpperBoundConstraint : IConstraint<int>
    {
        /**
         * Uppper bound value
         */
        private int limit;

        /**
         * @param limit upper bound value
         */
        public UpperBoundConstraint(int limit)
        {
            this.limit = limit;
        }

        /**
         * Check whether parameter is less or equal to the specified
         * upper bound.
         * @param parameter value of a Parameter's argument
         */
        public bool IsValid(int parameter)
        {
            return parameter <= limit;
        }
    }

    /**
     * Parameter constraint validating whether its argument is within
     * the specified list of String values.
     */
    public sealed class StringEnumerationConstraint : IConstraint<String>
    {
        /**
         * Collection of accepted values.
         */
        private IEnumerable<String> acceptedValues;

        /**
         * Comparer used to compare arguments to the list of accepted values.
         */
        private IEqualityComparer<String> comparer;

        /**
         * @param acceptedValues Collection of accepted values. Must not be null.
         * @param Comparer used to compare arguments to the list of accepted values. Must not be null.
         * @throw ArgumentNullException thrown when either of the parameters is null
         */
        public StringEnumerationConstraint(IEnumerable<String> acceptedValues, IEqualityComparer<String> comparer)
        {
            if (acceptedValues == null)
            {
                throw new ArgumentNullException("acceptedValues");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            this.acceptedValues = acceptedValues;
            this.comparer = comparer;
        }

        /**
         * @param acceptedValues Collection of accepted values. Must not be null.
         * @param If true, ignore case; otherwise, regard case.
         * @throw ArgumentNullException thrown when acceptedValues is null
         */
        public StringEnumerationConstraint(IEnumerable<String> acceptedValues, bool ignoreCase)
            : this(
                acceptedValues,
                ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture
            )
        { }

        /**
         * Check whether parameter is within the specified range of accepted values.
         * @param parameter value of a Parameter's argument
         */
        public bool IsValid(String parameter)
        {
            return acceptedValues.Contains(parameter, comparer);
        }
    }

    /**
     * Parameter constraint validating whether its argument represent
     * an existing file.
     */
    public sealed class ExistingFileConstraint : IConstraint<String>
    {
        /**
         * Check whether parameter represents path to an existing file.
         * The path can be relative or absolute. Relative path information
         * is interpreted as relative to the current working directory
         * @param parameter value of a Parameter's argument
         */
        public bool IsValid(String parameter)
        {
            return System.IO.File.Exists(parameter);
        }
    }
}