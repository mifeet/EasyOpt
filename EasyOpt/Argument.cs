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
        Division, ShortOption, LongOption, ProgramArgument
    }

    public static class Extensions
    {
        public static bool IsOption(this ArgumentType argumentType)
        {
            bool isAnOption = argumentType.Equals(ArgumentType.ShortOption) || 
                argumentType.Equals(ArgumentType.LongOption);

            return isAnOption; 
        }
    }

    public interface IArgument
    {
        ArgumentType Type
        {
            get;
        }

        string UnparsedText
        {
            get;
        }
    }

    /**
     * Class that represents the data stored inside an argument received through the command line.
     */
    class Argument : IArgument
    {
        /** String that represents the division argument between options and program arguments */
        private const string divisionArgument = "--";

        /** String that represents the equal symbol */
        private const string equalSymbol = "=";

        /** Name to identify an option */
        private string name;

        /** Stores the unparsed text of a given argument */
        private string unparsedText;

        public string UnparsedText
        {
            get
            {
                return this.unparsedText;
            }
        }

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
        //Regular expression of a short name
        private const string shortName = "[a-zA-Z0-9]";

        //Regular expression of a long name
        private const string longName = shortName + "[a-zA-Z0-9-]+";

        /** Object used to check whether a given short name is valid */
        private static Regex shortNamePattern = new Regex("^" + shortName + "$");

        /** Object used to check whether a given long name is valid */
        private static Regex longNamePattern = new Regex("^" + longName + "$");

        /** Valid regular expression of a short option argument */
        private static Regex shortArgumentPattern = new Regex("^-" + shortName +"[.]*");

        /** Valid regular expression of a short option argument */
        private static Regex longArgumentPattern = new Regex("^--" + longName + "(=[.]*)?");

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
            this.unparsedText = null;
        }

        /**
         * Factory method to create an instance given an unparsed argument.
         */
        public static Argument Create(string unparsedArgument)
        {
            Argument argument = new Argument();
            argument.unparsedText = unparsedArgument;

            if (unparsedArgument.Equals(divisionArgument))
            {   argument.type = ArgumentType.Division;
            }
            else if (shortArgumentPattern.IsMatch(unparsedArgument))
            {
                argument.type = ArgumentType.ShortOption;
                argument.name = unparsedArgument[1].ToString();
                if (unparsedArgument.Length > indexStartParameter)
                {
                    argument.parameter = unparsedArgument.Substring(indexStartParameter);
                }
            }
            else if (longArgumentPattern.IsMatch(unparsedArgument))
            {
                argument.type = ArgumentType.LongOption;

                if (unparsedArgument.Contains(equalSymbol))
                {
                    int equalSymbolIndex = unparsedArgument.IndexOf(equalSymbol);

                    argument.name = unparsedArgument.Substring(
                        longPatternOffset,
                        equalSymbolIndex - longPatternOffset
                        );
                    argument.parameter = unparsedArgument.Substring(equalSymbolIndex + 1);
                }
                else
                {
                    argument.name = unparsedArgument.Substring(longPatternOffset);
                }
            }
            else
            {
                argument.type = ArgumentType.ProgramArgument;
                argument.programArgument = unparsedArgument;
            }
            
            return argument;
        }

        /**
         * Method used to validate a short name
         * @param shortName name to be checked
         * @return true if the name is valid
         */
        public static bool IsShortNameValid (String shortName)
        {
            return shortNamePattern.IsMatch(shortName);
        }

        /**
         * Method used to validate a long name
         * @param longName name to be checked
         * @return true if the name is valid
         */
        public static bool IsLongNameValid(String longName)
        {
            return longNamePattern.IsMatch(longName);
        }

    }
}
