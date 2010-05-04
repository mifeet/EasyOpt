using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Factory class for Option<T> objects. 
     * Use this class for creating Option<T> objects. A proper instance is returned
     * based on the type of option's parameter.
     * Options without a parameter are represented as Option<bool>.
     * 
     * Example of usage:
     * var outputParam = new StringParameter(true, "FILE");
     * var output = OptionFactory.Create(false, "Output to FILE.", outputParam);
     *  // type of output.Value is String
     * 
     * var help = OptionFactory.Create(false, "Print a usage message.");
     *  // type of help.Value is bool
     */
    public class OptionFactory
    {
        /**
         * Creates an option with no parameters.
         */
        public static Option<bool> Create(bool isRequired, String usageText)
        {
            return new SimpleOption(isRequired, usageText);
        }

        /**
         * Creates an option with parameter with value of type T.
         */
        public static Option<T> Create<T>(bool isRequired, String usageText, Parameter<T> parameter)
        {
            return new ParameterOption<T>(isRequired, usageText, parameter);
        }
    }
}
