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

    internal static class Extensions
    {
        public static bool IsOption(this TokenType tokenType)
        {
            bool isAnOption = tokenType.Equals(TokenType.ShortOption) || 
                tokenType.Equals(TokenType.LongOption);

            return isAnOption; 
        }
    }

    internal interface IToken
    {
        IOption Option
        {
            get;
            set;
        }

        TokenType Type
        {
            get;
        }

        string UnparsedText
        {
            get;
        }
    }

    /**
     * Class that represents the data stored inside a token received through an argument of the command line.
     */
    internal class Token : IToken
    {
        /** String that represents the division argument between options and program arguments */
        private const string divisionTokenString = "--";

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

        /** Reference to token that originated the option */
        private IOption option;

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
        private TokenType type;

        public TokenType Type
        {
            get
            {
                return this.type;
            }
        }
        //Regular expression of a short name
        private const string shortName = "[a-zA-Z0-9]+";

        //Regular expression of a long name
        private const string longName = "[a-zA-Z0-9][a-zA-Z0-9-]+";

        /** Object used to check whether a given short name is valid */
        private static Regex shortNamePattern = new Regex("^" + shortName + "$");

        /** Object used to check whether a given long name is valid */
        private static Regex longNamePattern = new Regex("^" + longName + "$");

        /** Valid regular expression of a short option */
        private static Regex shortOptionPattern = new Regex("^-" + shortName +"[.]*");

        /** Valid regular expression of a short option  */
        private static Regex longOptionPattern = new Regex("^--" + longName + "(=[.]*)?");

        /** Parameter start index for the short unparsed argument */
        private const int indexStartParameter = 2;

        /** Offset used parse the name for the long option arguent */
        private const int longPatternOffset = 2;

        /** Constructor that initializes an instance.
         *  Method Create() should be called to create an instance.*/
        private Token()
        {
            this.name = null;
            this.parameter = null;
            this.programArgument = null;
            this.unparsedText = null;
        }

        /**
         * Factory method to create an instance given an unparsed argument.
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

                    if (!(option is SimpleOption))
                    {
                        if (i + 1 < unparsedArgument.Length)
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
         * Checks that the argument's name is valid
         * @param name, name received from the arguments in the command line. 
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
