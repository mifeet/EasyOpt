using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    internal class UsageBuilder
    {
        /** Contains program usage description */
        private String usageDescription;

        /** Container of options displayed in the usage text */
        private IOptionContainer optionContainer;

        /**
         * Used to build usage text.
         * Formatting methods can all append to the intermediate
         * result stored here.
         * Invariant: not null, after constructor call filled with the complete usage text
         */
        private StringBuilder usageText;

        /** String used for indenting */
        private const string indent = "    ";

        /** String that separates long option and its parameter */
        private const string equalSymbol = "=";

        /** Width to which the usage text is wrapped */
        private const int maxWidth = 80;

        /**
         * @param usageDescription Brief program usage description
         * @param optionContainer Container of options displayed in the usage text
         */
        public UsageBuilder(String usageDescription, IOptionContainer optionContainer)
        {
            this.usageDescription = usageDescription;
            this.optionContainer = optionContainer;
            this.usageText = new StringBuilder();
            formatUsage(); // TODO move from the constructor?
        }

        /**
         * Returns formatted usage text
         */
        public String GetUsage()
        {
            return usageText.ToString();
        }

        /**
         * Format the whole usage text and store it in usageText field.
         */
        private void formatUsage() {
            usageText.Append(this.usageDescription);
            usageText.AppendLine();
            usageText.AppendLine();

            IEnumerable<string> uniqueNames = optionContainer.ListUniqueNames();

            foreach (String uniqueName in uniqueNames)
            {
                IOption option = optionContainer.FindByName(uniqueName);
                String[] names = optionContainer.FindSynonymsByName(uniqueName);

                formatNames(names, option);
                formatOptionUsage(option);
                usageText.AppendLine();
            }
        }

        /**
         * Format list of synonymous option names with the corresponding parameter
         * and append it to usageText
         * @param names List of synonymous option names
         * @param option the option being described
         */
        private String formatNames(String[] names, IOption option)
        {
            usageText.Append(indent);
            for (int i = 0; i < names.Length; i++)
            {
                String name = names[i];
                bool isShortOption = name.Length == 1;
                
                if (isShortOption)
                {
                    formatShortOption(name);
                }
                else // long option
                {
                    formatLongOption(name);
                }

                
                bool hasNextName = (i + 1) < names.Length;
                if (hasNextName)
                {
                    usageText.Append(", ");
                }
                else
                {
                    String paramSeparator = isShortOption ? " " : equalSymbol;
                    formatParameter(paramSeparator , option);
                    // append parameter name (when present) and newline after the last option
                    usageText.AppendLine();
                }
            }
            return "";
        }

        /**
         * Format short option and append it to usageText
         * @param names short option name
         */
        private void formatShortOption(string name)
        {
            usageText.Append('-');
            usageText.Append(name);
        }

        /**
         * Format long option and append it to usageText
         * @param names long option name
         */
        private void formatLongOption(string name)
        {
            usageText.Append("--");
            usageText.Append(name);
        }

        /**
         * Format option parameter and append it to usageText.
         * If the option has no parameter, appends nothing.
         * @param parameterSeparator string that separates parameter from option name
         * @param option the corresponding option object
         */
        private void formatParameter(string parameterSeparator, IOption option)
        {
            if (!option.HasParameter)
            {
                return;
            }

            if (!option.IsParameterRequired)
            {
                usageText.Append('[');
            }

            usageText.Append(parameterSeparator);
            usageText.Append(option.ParameterUsageName);

            if (!option.IsParameterRequired)
            {
                usageText.Append(']');
            }
        }

        /**
         * Format option usage description.
         * Wrapped to maximum of maxWidth characters.
         * Append result to usageText.
         * @param option the corresponding option object
         */
        private void formatOptionUsage(IOption option)
        {
            usageText.Append(indent);
            usageText.Append(indent);
            usageText.AppendLine(option.UsageText); // TODO format in block to maxWidth
        }
    }
}