﻿using System;
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
        private IOptionContainer optionContainer;
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
            this.optionContainer = new OptionContainer();
            this.unparsedArguments = unparsedArguments;
            this.phase = ParsePhase.Option;
        }

        public void AddOption<T>(Option<T> option, params String[] names)
        {
            this.optionContainer.Add(option, names);
        }

        public void AddOption<T>(Option<T> option, char shortName)
        {
            this.optionContainer.Add(option, new String[] { shortName.ToString() });
        }

        public void AddOption<T>(Option<T> option, char shortName, String longName)
        {
            this.optionContainer.Add(option, new String[] { shortName.ToString(), longName });
        }

        public void Parse()
        {
            Token lastParsedToken = null;

            foreach (string unParsedArgument in unparsedArguments)
            {
                if (phase.Equals(ParsePhase.Option))
                {
                    lastParsedToken = parseOption(unParsedArgument);
                }
                else if (phase.Equals(ParsePhase.WaitingForArgument))
                {
                    lastParsedToken = parseWaitingForArgument(lastParsedToken, unParsedArgument);
                }
                else if (phase.Equals(ParsePhase.ProgramArgument))
                {
                    this.programArguments.Add(unParsedArgument);
                }
            }

            checkParserFinalState(lastParsedToken);

            checkRequiredOptions();
        }

        /**
         * Check that all required options are given in the command line arguments.
         */ 
        private void checkRequiredOptions()
        {
            List<string> uniqueNames = this.optionContainer.ListUniqueNames();

            foreach (string uniqueName in uniqueNames)
            {
                IOption option = this.optionContainer.FindByName(uniqueName);
                if (option.IsRequired && !option.IsPresent)
                {
                    throw new ParseException("Option: " + uniqueName + " is required but not specified.");
                }
            }

        }

        /**
         * Attempts to parse the given unParsedArgument as a parameter for the last parsedOption.
         * In case is optional creates a new Option.
         * @throw ParseException if it is not possible to parse the argument and the parameter is required.
         */ 
        private Token parseWaitingForArgument(Token lastParsedToken, string unParsedArgument)
        {
            if (lastParsedToken == null)
            {
                throw new ParseException("Internal error: lastParsedToken is null.");
            }

            this.phase = ParsePhase.Option;

            Token currentToken = null;

            if (lastParsedToken.Type.Equals(TokenType.LongOption))
            {
                if (lastParsedToken.Option.IsParameterRequired)
                {
                    setOptionValue(lastParsedToken, unParsedArgument);
                }
                else
                {
                    try
                    {
                        setOptionValue(lastParsedToken, unParsedArgument);
                    }
                    catch (ParseException)
                    {
                        currentToken = parseOption(unParsedArgument);
                    }
                }
            }
            else
            {
                currentToken = parseOption(unParsedArgument);
            }
            

            return currentToken;
        }


        /**
         * Verify the final state of the parser.
         * @throw ParseException when the parser phase has a non valid ending state.
         */ 
        private void checkParserFinalState(Token lastParsedToken)
        {
            if (this.phase.Equals(ParsePhase.WaitingForArgument) && lastParsedToken.Option.IsParameterRequired)
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
        private Token parseOption(string unParsedArgument)
        {
            Token lastParsedToken = null;
            
            List<Token> tokens = Token.Create(unParsedArgument, optionContainer);

            foreach (Token token in tokens)
            {
                lastParsedToken = token;

                if (token.Type.Equals(TokenType.Division))
                {
                    this.phase = ParsePhase.ProgramArgument;
                }
                else if (token.Type.Equals(TokenType.ProgramArgument))
                {
                    this.phase = ParsePhase.ProgramArgument;
                    this.programArguments.Add(token.ProgramArgument);
                }
                else if (token.Type.IsOption())
                {
                    IOption option = optionContainer.FindByName(token.Name);
                    option.IsPresent = true;
                    token.Option = option;

                    if (option.IsParameterRequired)
                    {
                        parseRequiredParameter(option, token);
                    }

                    if (token.Parameter == null)
                    {
                        if (token.Type.Equals(TokenType.LongOption)){
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
         */
        private void setOptionValue(Token token, string value)
        {
            try
            {
                token.Option.SetValue(value);
            }
            catch (ForbiddenParameterException)
            {
                throw new ParseException("Option: " + token.UnparsedText + " can't have a parameter.");
            }
            catch (Exception e)
            {
                throw new ParseException("Parameter of option: " + token.UnparsedText + " is invalid.", e);
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
        private void parseRequiredParameter(IOption option, Token argument)
        {
            if (
                argument.Parameter == null && 
                argument.Type.Equals(TokenType.ShortOption)
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
