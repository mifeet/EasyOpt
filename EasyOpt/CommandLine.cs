using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Base class for exceptions thrown by EasyOpt components.
     */
    public abstract class EasyOptException : Exception
    {
        public EasyOptException(string message)
            : base(message)
        { }

        public EasyOptException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }



    /**
     * Class thrown when there is an error parsing an argument.
     * 
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
        public ParseException(string message, Exception exception)
            : base(message, exception)
        { }
    }

    /**
     * Parse process is done in three phases.
     * 
     * 1. option: parses an argument as an option.
     *    Can go to phase 2 is the parameter is missing
     *    Can go to phase 3 is division argument is parsed.
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
     * Class used as an abstraction of the command line. 
     * Operations in this class should be done in three steps.
     * 
     * 1. Configuration: methods addOption()
     * 2. Parse: method parse()
     * 3. Query: method XXX
     */
     
    public class CommandLine
    {
        //Object used to map option identifiers with option configurations
        private Dictionary<String, IOption> options;
        //Object that stores the arguments entered by the user
        private String[] unparsedArguments;

        //Object that stores the parse phase
        private ParsePhase phase;

        private List<string> programArguments = new List<string>();

        /**
         * Returns the program arguments for the given command line arguments.
         */ 
        public String[] getProgramArguments()
        {
            return programArguments.ToArray();
        }

        /**
         * Initializes a CommandLine instance.
         * 
         * Param: Paremeters received from the console.
         */
        public CommandLine(String[] unparsedArguments)
        {
            this.options = new Dictionary<string, IOption>();
            this.unparsedArguments = unparsedArguments;
            this.phase = ParsePhase.Option;
        }

        public void AddOption(IOption option, params String[] names)
        {
            // checkConfiguration(option); ?
            foreach (String name in names)
            {
                options.Add(name, option);
            }
        }

        public void AddOption(IOption option, char shortName)
        {
            AddOption(option, new String[] { shortName.ToString() });
        }

        public void AddOption(IOption option, char shortName, String longName)
        {
            AddOption(option, new String[] { shortName.ToString(), longName });
        }

        public void Parse()
        {
            IOption lastParsedOption = null;

            foreach (string unParsedArgument in unparsedArguments)
            {
                if (phase.Equals(ParsePhase.Option))
                {
                    lastParsedOption = parseOption(unParsedArgument);
                }
                else if (phase.Equals(ParsePhase.WaitingForArgument))
                {
                    lastParsedOption = parseWaitingForArgument(lastParsedOption, unParsedArgument);
                }
                else if (phase.Equals(ParsePhase.ProgramArgument))
                {
                    this.programArguments.Add(unParsedArgument);
                }
            }

            checkParserFinalState(lastParsedOption);
        }

        /**
         * Attempts to parse the given unParsedArgument as a parameter for the last parsedOption.
         * In case is optional creates a new Option.
         * @throw ParseException if it is not possible to parse the argument and the parameter is required.
         */ 
        private IOption parseWaitingForArgument(IOption lastParsedOption, string unParsedArgument)
        {
            if (lastParsedOption == null)
            {
                throw new ParseException("Internal error: lastParsedOption is null.");
            }

            this.phase = ParsePhase.Option;

            IOption option = null;

            if (lastParsedOption.Argument.Type.Equals(ArgumentType.LongOption))
            {
                if (lastParsedOption.IsParameterRequired)
                {
                    setOptionValue(lastParsedOption, unParsedArgument);
                }
                else
                {
                    try
                    {
                        setOptionValue(lastParsedOption, unParsedArgument);
                    }
                    catch (ParseException)
                    {
                        option = parseOption(unParsedArgument);
                    }
                }
            }
            else
            {
                option = parseOption(unParsedArgument);
            }
            

            return option;
        }


        /**
         * Verify the final state of the parser.
         * @throw ParseException when the parser phase has a non valid ending state.
         */ 
        private void checkParserFinalState(IOption lastParsedOption)
        {
            if (this.phase.Equals(ParsePhase.WaitingForArgument) && lastParsedOption.IsParameterRequired)
            {
                //TODO discuss a data structure to retrieve synonyms to options, maybe another hash table.
                throw new ParseException ("Missing argument for option: ");
            }
        }

        /**
         * Procedure that parses a command line argument.
         * 
         * @return Option associated with the argument.
         */ 
        private IOption parseOption(string unParsedArgument)
        {

            Argument argument = Argument.Create(unParsedArgument);
            IOption option = null;

            if (argument.Type.Equals(ArgumentType.Division))
            {
                this.phase = ParsePhase.ProgramArgument;
            }
            else if (argument.Type.Equals(ArgumentType.ProgramArgument))
            {
                this.phase = ParsePhase.ProgramArgument;
                this.programArguments.Add(argument.ProgramArgument);
            }
            else if (argument.Type.IsOption())
            {
                checkOptionName(argument);

                option = options[argument.Name];
                option.IsPresent = true;
                option.Argument = argument;

                if (option.IsParameterRequired)
                {
                    parseRequiredParameter(option, argument);
                }

                if (argument.Parameter == null)
                {
                    this.phase = ParsePhase.WaitingForArgument;
                }
                else
                {
                    setOptionValue(option, argument.Parameter);
                }
            }
            return option;
        }

        /**
         * Sets the option's value.
         * @param option the option that is being parsed
         * @param argument the parsed argument received from the command line.
         * @throw ParseException when is not possible to specify the value for the object.
         */
        private void setOptionValue(IOption option, string value)
        {
            try
            {
                option.SetValue(value);
            }
            catch (ForbiddenParameterException)
            {
                throw new ParseException("Option: " + option.Argument.UnparsedText + " can't have a parameter.");
            }
            catch (Exception e)
            {
                throw new ParseException("Parameter of option: " + option.Argument.UnparsedText + " is invalid.", e);
            }
        }

        /**
         * Checks that the argument's name is valid
         * @param argument the parsed argument received from the command line. 
         * @throw ParseException if the name is not valid
         */
        private void checkOptionName(Argument argument)
        {
            if (!options.ContainsKey(argument.Name))
            {
                throw new ParseException("Option: " + argument.Name + " is not defined.");
            }
        }

        /**
         * Parse procedure for a parameter that is required.
         * 
         * @param option the option that is being parsed
         * @param argument the parsed argument received from the command line.
         * 
         * @throw ParseException if the parameter is required and it is a short option.
         */
        private void parseRequiredParameter(IOption option, Argument argument)
        {
            if (
                argument.Parameter == null && 
                argument.Type.Equals(ArgumentType.ShortOption)
            )
            {
                throw new ParseException("Option: " + argument.Name + " requires a parameter.");

            }
        }

        public IList<String> GetArguments() // return non-option arguments
        {
            //throw new NotImplementedException();
            return new String[] { };
        }

        public String UsageText { get; set; }

        public String GetUsage()
        {
            //throw new NotImplementedException();
            return "";
        }

    }
}
