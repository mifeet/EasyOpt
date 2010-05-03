using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    /**
     * Class thrown when there is an error parsing an argument.
     * 
     */ 
    public class ParseException : Exception
    {
        public ParseException(string message)
        : base(message)
        {
            
        }
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
        option, waitingForArgument, programArgument
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
        private String[] unParsedArguments;

        //Object that stores the parse phase
        private ParsePhase phase;

        
        /**
         * Initializes a CommandLine instance.
         * 
         * Param: Paremeters received from the console.
         */
        public CommandLine(String[] unParsedArguments)
        {
            this.options = new Dictionary<string, IOption>();
            this.unParsedArguments = unParsedArguments;
            this.phase = ParsePhase.option;
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
            foreach (string unParsedArgument in unParsedArguments)
            {
                if (phase.Equals(ParsePhase.option))
                {
                    Argument argument = Argument.Create(unParsedArgument);

                    if (argument.Type.Equals(ArgumentType.division))
                    {
                        this.phase = ParsePhase.programArgument;
                    }
                    else if (
                        argument.Type.Equals(ArgumentType.shortOption) ||
                        argument.Type.Equals(ArgumentType.longOption)
                    ){
                        if (options.ContainsKey(argument.Name))
                        {
                            IOption option = options[argument.Name];
                            option.SetValue(argument.Parameter);
                            option.IsPresent=true;
                        }
                        else
                        {
                            throw new ParseException("Option: " + argument.Name + " is not defined.");
                        }
                    }
                }
                else if (phase.Equals(ParsePhase.waitingForArgument))
                {
                    //Do not need to parse, just set the parameter of the option
                }
                else if (phase.Equals(ParsePhase.programArgument))
                {
                    //Copy the program parameters, at the end of parsing put them in a string[]
                }
            }

            // throw new NotImplementedException();
        }

        public String[] GetArguments() // return non-option arguments
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
