using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Enumeration with the possible kinds of tokens that can be parsed.
     */ 
    internal enum TokenType
    {
        Division, ShortOption, LongOption, ProgramArgument
    }

    /**
     * Class providing extension methods for TokenType.
     */
    internal static class TokenExtensions
    {
        /**
         * Returns true when tokenType represents an option, false otherwise
         */
        public static bool IsOption(this TokenType tokenType)
        {
            bool isAnOption = tokenType.Equals(TokenType.ShortOption) || 
                tokenType.Equals(TokenType.LongOption);
            
            return isAnOption; 
        }
    }

    /**
     * Class is an abstraction of the tokens used to parse the command line
     * arguments. It is responsible for creating those tokens and handles
     * low level parsing of the data using regular expressions.
     */
    internal class Token
    {
        /** String that represents the division argument between options and program arguments */
        private const string divisionTokenString = "--";

        /** String that separates long option and its parameter */
        private const string equalSymbol = "=";

        /** Name to identify an option */
        private string name;

        /** Stores the unparsed text of a given argument */
        private string unparsedText;

        /** Gets the unparsed text of a given argument */
        public string UnparsedText
        {
            get
            {
                return this.unparsedText;
            }
        }

        /** Gets name identifying a corresponding option */
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /** Reference to the option corresponding to this token */
        private IOption option;

        /** Gets reference to the option corresponding to this token */
        public IOption Option
        {
            get
            {
                return this.option;
            }
            set 
            {
                this.option = value;
            }
        }

        /** String representation of the corresponding option's parameter */
        private string parameter;

        /** Gets string representation of the corresponding option's parameter */
        public string Parameter
        {
            get
            {
                return this.parameter;
            }
        }

        /** String that stores the program argument (a non-optional argument) */
        private string programArgument;

        /** Gets program argument (a non-optional argument) */
        public string ProgramArgument
        {
            get
            {
                return this.programArgument;
            }
        }

        /** Type of the token */
        private TokenType type;

        /** Gets type of the token */
        public TokenType Type
        {
            get
            {
                return this.type;
            }
        }
        /** Regular expression of a short name */
        private const string shortName = "[^-=\\s]+";

        /** Regular expression of a long name */
        private const string longName = "[^-=\\s][^=\\s]+";

        /** Object used to check whether a given short name is valid */
        private static Regex shortNamePattern = new Regex("^" + shortName + "$");

        /** Object used to check whether a given long name is valid */
        private static Regex longNamePattern = new Regex("^" + longName + "$");

        /** Valid regular expression of a short option */
        private static Regex shortOptionPattern = new Regex("^-" + shortName +"[.]*");

        /** Valid regular expression of a short option  */
        private static Regex longOptionPattern = new Regex("^--" + longName + "(=[.]*)?");

        /** Regular expression for an ilegal option */
        private static Regex ilegalOptionPattern = new Regex("(^--[^-=\\s]$)|(^--(-)+)");

        /** Parameter start index for the short unparsed argument */
        private const int indexStartParameter = 2;

        /** Offset used parse the name for the long option arguent */
        private const int longPatternOffset = 2;

        /**
         * Constructor that initializes an instance.
         * Method Create() should be called to create an instance.
         */
        private Token()
        {
            this.name = null;
            this.parameter = null;
            this.programArgument = null;
            this.unparsedText = null;
        }

        /**
         * Factory method to create list of tokens from a unparsed argument value.
         * @param unparsedArgument A single unparsed argument from the commnad line.
         * @param optionContainer Container object of all defined options.
         */
        public static List<Token> Create(string unparsedArgument, IOptionContainer optionContainer)
        {
            List<Token> tokens = new List<Token>();

            if (unparsedArgument.Equals(divisionTokenString))
            {
                Token divisionToken = new Token();
                divisionToken.unparsedText = unparsedArgument;
                divisionToken.type = TokenType.Division;

                tokens.Add(divisionToken);
            }
            else if (ilegalOptionPattern.IsMatch(unparsedArgument))
            {
                throw new ParseException("Ilegal option: " + unparsedArgument);
            }
            else if (shortOptionPattern.IsMatch(unparsedArgument))
            {

                for (int i = 1; i < unparsedArgument.Length; i++)
                {
                    string optionName = unparsedArgument[i].ToString();

                    checkOptionName(optionName, optionContainer);

                    IOption option = optionContainer.FindByName(optionName);

                    Token shortOptionToken = new Token();
                    shortOptionToken.unparsedText = unparsedArgument;
                    shortOptionToken.type = TokenType.ShortOption;
                    shortOptionToken.name = optionName;

                    tokens.Add(shortOptionToken);

                    if (option.HasParameter)
                    {
                        if ((i + 1) < unparsedArgument.Length)
                        {
                            shortOptionToken.parameter = unparsedArgument.Substring(i + 1);
                        }
                        break;
                    }
                }
            }
            else if (longOptionPattern.IsMatch(unparsedArgument))
            {
                Token longOptionToken = new Token();
                longOptionToken.unparsedText = unparsedArgument;
                longOptionToken.type = TokenType.LongOption;

                if (unparsedArgument.Contains(equalSymbol))
                {
                    int equalSymbolIndex = unparsedArgument.IndexOf(equalSymbol);

                    longOptionToken.name = unparsedArgument.Substring(
                        longPatternOffset,
                        equalSymbolIndex - longPatternOffset
                        );
                    longOptionToken.parameter = unparsedArgument.Substring(equalSymbolIndex + 1);
                }
                else
                {
                    longOptionToken.name = unparsedArgument.Substring(longPatternOffset);
                }

                checkOptionName(longOptionToken.name, optionContainer);

                tokens.Add(longOptionToken);
            }
            else
            {
                Token argumentToken = new Token();
                argumentToken.unparsedText = unparsedArgument;
                argumentToken.type = TokenType.ProgramArgument;
                argumentToken.programArgument = unparsedArgument;

                tokens.Add(argumentToken);
            }
            
            return tokens;
        }

        /**
         * Checks that the options's name is valid (a corresponding option object exists)
         * @param name, name received from the arguments in the command line. 
         * @param optionContainer Container objec of all defined options.
         * @throw ParseException if the name is not valid
         */
        private static void checkOptionName(string name, IOptionContainer optionContainer)
        {
            if (!optionContainer.ContainsName( name))
            {
                throw new ParseException("Option: " + name + " is not defined.");
            }
        }

        /**
         * Method used to validate a name
         * @param shortName name to be checked
         * @throw InvalidNameException when the name is not valid.
         */
        public static void CheckName (String name)
        {
            if (name == null)
            {
                throw new InvalidNameException("An Option name can not be null.");
            }
            
            if (
                !longNamePattern.IsMatch(name) && 
                !shortNamePattern.IsMatch(name)
            ){
                throw new InvalidNameException("Option name: " + name + " is invalid");
            }
        }
    }
}
