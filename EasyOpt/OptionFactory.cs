using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Class that implements the pattern factory method. Used to retrieve instances of the class Option.
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
         * Creates an option with parameter T.
         */
        public static Option<T> Create<T>(bool isRequired, String usageText, Parameter<T> parameter)
        {
            return new ParameterOption<T>(isRequired, usageText, parameter);
        }
    }
}
