using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOptLibrary
{
    /**
     * Base class for exceptions thrown by EasyOpt components.
     */
    public abstract class EasyOptException : Exception
    {
        public EasyOptException()
            : base()
        { }

        public EasyOptException(string message)
            : base(message)
        { }

        public EasyOptException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /**
     * Class thrown when there is an error parsing a command line argument 
     * that does not allow the parse process to continue.
     */
    public class ParseException : EasyOptException
    {
        /**
         * Constructor method
         * @param message exception's detail message
         */
        public ParseException(string message)
            : base(message)
        { }

        /**
         * Constructor method
         * @param message exception's detail message
         * @param exception inner exception
         */
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /**
     * Exception for option errors.
     */
    public abstract class OptionException : EasyOptException
    {
        private String optionName;

        /** Option name */
        public String OptionName
        {
            get
            {
                return optionName;
            }
            private set
            {
                if (value.StartsWith("-")) {
                    optionName = value;
                }
                else if (value.Length == 1)
                {
                    optionName = "-" + value;
                }
                else
                {
                    optionName = "--" + value;
                }
            }
        }

        /**
         * @param option Option name. Accepts both names with or without -/-- at the beginning.
         */
        protected OptionException(String option)
        {
            this.OptionName = option;
        }

        /**
         * @param option Option name. Accepts both names with or without -/-- at the beginning.
         */
        protected OptionException(String option, Exception innerException)
            : base("", innerException)
        {
            this.OptionName = option;
        }
    }

    /**
     * Exception thrown when a required option is missing
     */
    public class OptionMissingException : OptionException
    {
        /**
         * @param option Option name
         */
        public OptionMissingException(String option)
            : base(option)
        { }

        /**
         * Gets a message that describes the current exception
         */
        public override string Message
        {
            get
            {
                return "Required option " + OptionName + " is missing";
            }
        }
    }

    /**
     * Exception thrown when a required option parameter is missing
     */
    public class ParameterMissingException : OptionException
    {
        /**
         * @param option Option name
         */
        public ParameterMissingException(String option)
            : base(option)
        { }

        /**
         * Gets a message that describes the current exception
         */
        public override string Message
        {
            get
            {
                return "Required parameter of " + OptionName + " is missing";
            }
        }
    }

    /**
     * Exception thrown when parameter's value is invalid.
     */
    public class OptionParameterException : OptionException
    {
        /**
         * The original value of the parameter before conversion.
         */
        public string ParameterValue
        {
            get
            {
                if (innerParameterException == null)
                {
                    return "";
                }
                else
                {
                    return InnerParameterException.ParameterValue;
                }
            }
        }

        private ParameterException innerParameterException;
        /**
         * Inner exception
         */
        public ParameterException InnerParameterException
        {
            get { return innerParameterException; }
        }

        /**
         * @param option Option name
         * @param parameter Parameter value before conversion
         */
        public OptionParameterException(String option, ParameterException innerException)
            : base(option, innerException)
        {
            this.innerParameterException = innerException;
        }

        /**
         * Gets a message that describes the current exception
         */
        public override string Message
        {
            get
            {
                string message = "Invalid " + OptionName + " parameter";
                if (innerParameterException != null)
                {
                    message += ": " + innerParameterException.Message;
                }
                return message;
            }
        }
    }

    /**
     * Parse process is done in three phases.
     * 
     * 1. option: parses an argument as an option.
     *    Can go to phase 2 if the parameter is missing and it is a long option
     *    Can go to phase 3 if the division token (--) is found.
     * 2. waitingForArgument: defines the next unparsed argument as an option parameter.
     *    Go directly to phase 1.
     * 3. programArgument: defines a program argument as an unparsed argument.
     *    Final state, can not go to any other phase.
     */
    internal enum ParsePhase
    {
        Option, WaitingForArgument, ProgramArgument
    }

    /**
     * This class is abstraction of the command line. Create an instance of 
     * this class to start working with EasyOpt. It is responsible 
     * for all the procedures directly related to the user of this library.
     * It provides methods to add options, parse the command line
     * argument based and generate a text usage.
     * 
     * 
     * Use this class in three steps.
     * 
     * 1. Create a new instance with the command line arguments as a parameter
     * 2. Configuration step: call AddOption(). One call per each Option.
     * 3. Parse: call parse() to begin the parse process.
     * 
     * The parse method fill your options with all the needed information.
     */
    public class EasyOpt
    {
        /**
         * Container of all option objects passed by AddOption< T >()
         */
        private IOptionContainer optionContainer;

        /**
         * Stores the parser's current phase
         */
        private ParsePhase phase;

        /**
         * List of non-option arguments.
         */
        private List<string> programArguments;

        /**
         * Returns non-option arguments.
         * Arguments are listed in the same order they were on the input.
         */
        public String[] GetArguments()
        {
            return programArguments.ToArray();
        }

        /**
         * Initializes an EasyOpt parser instance.
         * 
         * @See OptionFactory
         */
        public EasyOpt()
        {
            this.optionContainer = new OptionContainer();
            this.programArguments = new List<string>();
            this.phase = ParsePhase.Option;
        }

        /**
         * Description of program usage. Can be null.
         */
        public String UsageDescription { get; set; }

        /**
         * Add option using names as its synonymous option names.
         * Names one-character long are treated as short options (e.g. -v), 
         * longer names are treated as long option names (e.g. --verbose)
         * 
         * At least one name for the option must be listed.
         * 
         * Option name must not contain '=' character nor whitespace and it must not start with '-'.
         * Option names are case-sensitive.
         * 
         * An option should not be registered multiple times.
         * 
         * @param option Object representing an option. Must not be null.
         * @param names List of synonymous names for the option. One-character long name
         *      is treated as a short option name, longer name is treated as a long option name.
         *      Must not be empty.
         * @throw DuplicateOptionNameException
         * @throw InvalidNameException
         */
        public void AddOption<T>(Option<T> option, params String[] names)
        {
            this.optionContainer.Add(option, names);
        }

        /**
         * @see AddOption< T >(Option<T> option, params String[] names)
         * @param option Object representing an option. Must not be null.
         * @param shortName Short option name
         */
        public void AddOption<T>(Option<T> option, char shortName)
        {
            this.optionContainer.Add(option, new String[] { shortName.ToString() });
        }

        /**
         * @see AddOption< T >(Option<T> option, params String[] names)
         * @param option Object representing an option. Must not be null.
         * @param shortName Short option name.
         * @param longName Long option name.
         */
        public void AddOption<T>(Option<T> option, char shortName, String longName)
        {
            this.optionContainer.Add(option, new String[] { shortName.ToString(), longName });
        }

        /**
         * This method parses the command line arguments based on the configuration.
         * It must be called after all Options are added.
         * 
         * Each Option instance is filled with the information needed during
         * the parse process.
         * 
         * @see Option<T> to know how to retrieve the command line options
         * after the parse process.
         * 
         * @param unparsedArguments Array of arguments from the command line.
         * 
         * @throw ParseException if it is not possible to parse an argument
         */
        public void Parse(String[] unparsedArguments)
        {
            Token lastParsedToken = null;

            foreach (string unparsedArgument in unparsedArguments)
            {
                if (phase.Equals(ParsePhase.Option))
                {
                    lastParsedToken = parseOption(unparsedArgument);
                }
                else if (phase.Equals(ParsePhase.WaitingForArgument))
                {
                    lastParsedToken = parseWaitingForArgument(lastParsedToken, unparsedArgument);
                }
                else if (phase.Equals(ParsePhase.ProgramArgument))
                {
                    this.programArguments.Add(unparsedArgument);
                }
            }

            checkParserFinalState(lastParsedToken);

            checkRequiredOptions();
        }

        /**
         * Check that all required options are given within the command line arguments.
         * 
         * @throw OptionMissingException if there is a missing option in the arguments.
         */
        private void checkRequiredOptions()
        {
            IEnumerable<string> firstNames = this.optionContainer.ListFirstNames();

            foreach (string firstName in firstNames)
            {
                IOption option = this.optionContainer.FindByName(firstName);
                if (option.IsRequired && !option.IsPresent)
                {
                    throw new OptionMissingException(firstName);
                }
            }

        }

        /**
         * Attempts to parse the given unparsedArgument as a parameter for the last parsedOption.
         * In the case that is optional, it creates a new Option.
         * @throw ParseException if it is not possible to parse the argument and the parameter is required.
         */
        private Token parseWaitingForArgument(Token lastParsedToken, string unparsedArgument)
        {
            if (lastParsedToken == null)
            {
                throw new ParseException("Internal error: lastParsedToken is null.");
            }

            this.phase = ParsePhase.Option;

            Token currentToken = null;

            if (
                lastParsedToken.Type.IsOption() && 
                lastParsedToken.Option.IsParameterRequired
            ){
                setOptionValue(lastParsedToken, unparsedArgument);
            }
            else
            {
                currentToken = parseOption(unparsedArgument);
            }

            return currentToken;
        }

        /**
         * Verify that the final phase of the parser is a valid ending phase.
         * @throw ParameterMissingException when the parser phase has a non valid ending state.
         */
        private void checkParserFinalState(Token lastParsedToken)
        {
            if (this.phase.Equals(ParsePhase.WaitingForArgument) && lastParsedToken.Option.IsParameterRequired)
            {
                throw new ParameterMissingException(lastParsedToken.Name);
            }
        }

        /**
         * Procedure that parses a command line argument into Tokens.
         * 
         * @return Token last processed token.
         */
        private Token parseOption(string unparsedArgument)
        {
            Token lastParsedToken = null;

            List<Token> tokens = Token.Create(unparsedArgument, optionContainer);

            foreach (Token token in tokens)
            {
                lastParsedToken = token;

                if (token.Type.Equals(TokenType.Division))
                {
                    this.phase = ParsePhase.ProgramArgument;
                }
                else if (token.Type.Equals(TokenType.ProgramArgument))
                {
                    // uncomment the following line in order to disable non-option arguments within option arguments:
                    //this.phase = ParsePhase.ProgramArgument;

                    this.programArguments.Add(token.ProgramArgument);
                }
                else if (token.Type.IsOption())
                {
                    IOption option = optionContainer.FindByName(token.Name);
                    option.IsPresent = true;
                    token.Option = option;

                    if (token.Parameter == null)
                    {
                        if (option.HasParameter)
                        {
                            this.phase = ParsePhase.WaitingForArgument;
                        }
                    }
                    else
                    {
                        setOptionValue(token, token.Parameter);
                    }
                }

            }

            return lastParsedToken;
        }

        /**
         * Sets the option's value.
         * @param token the token that is being parsed
         * @param argument the parsed argument received from the command line.
         * @throw ParseException when is not possible to specify the value for the object.
         * @throw OptionParameterException thrown when parameter value is invalid.
         */
        private void setOptionValue(Token token, string value)
        {
            try
            {
                token.Option.SetValue(value);
            }
            catch (ForbiddenParameterException e)
            {
                throw new ParseException("Option: " + token.UnparsedText + " can't have a parameter.", e);
            }
            catch (ParameterException e)
            {
                throw new OptionParameterException(token.Name, e);
            }
        }

        /**
         * Returns usage message.
         * Usage message consists of UsageDescription and description
         * of all options passed by AddOption< T >() and their synonyms.
         */
        public String GetUsage()
        {
            UsageBuilder builder = new UsageBuilder(UsageDescription, optionContainer);
            return builder.GetUsage();
        }
    }
}
