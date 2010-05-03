using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Enumeration with the possible kinds of arguments that can be parsed.
     */ 
    public enum ArgumentType
    {
        division, shortOption, longOption, programArgument
    }

    public static class Extensions
    {
        public static bool IsOption(this ArgumentType argumentType)
        {
            bool isAnOption = argumentType.Equals(ArgumentType.shortOption) || 
                argumentType.Equals(ArgumentType.longOption);

            return isAnOption; 
        }
    }



    /**
     * Class that represents the data stored inside an argument received through the command line.
     */
    class Argument
    {
        /** String that represents the division argument between options and program arguments */
        private const string divisionArgument = "--";

        /** String that represents the equal symbol */
        private const string equalSymbol = "=";

        /** Name to identify an option */
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /** String representation of an option's parameter */
        private string parameter;

        public string Parameter
        {
            get
            {
                return this.parameter;
            }
        }

        /** String that stores the program argument */
        private string programArgument;

        public string ProgramArgument
        {
            get
            {
                return this.programArgument;
            }
        }

        /** Type of argument */
        private ArgumentType type;

        public ArgumentType Type
        {
            get
            {
                return this.type;
            }
        }

        /** Valid regular expression of a short option argument */
        private static Regex shortPattern = new Regex("^-[a-zA-Z0-9][.]*");

        /** Valid regular expression of a short option argument */
        private static Regex longPattern = new Regex("^--[a-zA-Z0-9-]+(=[.]*)?");

        /** Parameter start index for the short unparsed argument */
        private const int indexStartParameter = 2;

        /** Offset used parse the name for the long option arguent */
        private const int longPatternOffset = 2;

        /** Constructor that initializes an instance.
         *  Method Create() should be called to create an instance.*/
        private Argument()
        {
            this.name = null;
            this.parameter = null;
            this.programArgument = null;
        }

        /**
         * Factory method to create an instance given an unparsed argument.
         */
        public static Argument Create(string unParsedArgument)
        {
            Argument argument = new Argument();

            if (unParsedArgument.Equals(divisionArgument))
            {   argument.type = ArgumentType.division;
            }
            else if (shortPattern.IsMatch(unParsedArgument))
            {
                argument.type = ArgumentType.shortOption;
                argument.name = unParsedArgument[1].ToString();
                if (unParsedArgument.Length > indexStartParameter)
                {
                    argument.parameter = unParsedArgument.Substring(indexStartParameter);
                }
            }
            else if (longPattern.IsMatch(unParsedArgument))
            {
                argument.type = ArgumentType.longOption;

                if (unParsedArgument.Contains(equalSymbol))
                {
                    int equalSymbolIndex = unParsedArgument.IndexOf(equalSymbol);

                    argument.name = unParsedArgument.Substring(
                        longPatternOffset,
                        equalSymbolIndex - longPatternOffset
                        );
                    argument.parameter = unParsedArgument.Substring(equalSymbolIndex + 1);
                }
                else
                {
                    argument.name = unParsedArgument.Substring(longPatternOffset);
                }
            }
            else
            {
                argument.type = ArgumentType.programArgument;
                argument.programArgument = unParsedArgument;
            }
            
            return argument;
        }
    }
}
