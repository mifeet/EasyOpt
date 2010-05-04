﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyOpt
{
    public interface IOption
    {
        bool IsPresent
        {
            get;
            set;
        }

        bool IsParameterRequired
        {
            get;
        }

        bool IsRequired
        {
            get;
        }

        string UsageText
        {
            get;
        }

        string ParameterUsageName
        {
            get;
        }

        void SetValue(String value);
    }

    public abstract class Option<T> : IOption
    {
        public abstract T Value
        {
            get;
        }

        public abstract Parameter<T> Parameter
        {
            get;
        }

        public abstract bool IsParameterRequired
        {
            get;
        }

        public abstract string ParameterUsageName
        {
            get;
        }

        public abstract void SetValue(String value);

        public bool IsRequired { get; set; }

        public String UsageText { get; set; }

        public bool IsPresent { get; set; }

        internal Option(bool isRequired, String usageText)
        {
            this.IsRequired = isRequired;
            this.UsageText = usageText;
        }
    }

    internal class SimpleOption : Option<bool>
    {
        public override bool Value
        {
            get { return IsPresent; }
        }

        public override Parameter<bool> Parameter
        {
            get { return null; }
        }

        public override void SetValue(string value)
        {
            throw new ForbiddenParameterException(); // TODO: on the command line, a parameter was passed to an option without a parameter
        }

        public override bool IsParameterRequired
        {
            get { return false; }
        }

        public override string ParameterUsageName
        {
            get { return ""; }
        }

        public SimpleOption(bool isRequired, String usageText)
            : base(isRequired, usageText)
        { }
    }

    internal class ParameterOption<T> : Option<T>
    {
        private Parameter<T> parameter;

        public override Parameter<T> Parameter
        {
            get { return parameter; }
        }

        public override T Value
        {
            get { return this.parameter.Value; }
        }

        public override void SetValue(string value)
        {
            this.parameter.SetValue(value);
        }

        public override bool IsParameterRequired
        {
            get { return parameter.IsRequired; }
        }

        public override string ParameterUsageName
        {
            get { return parameter.UsageName; }
        }

        public ParameterOption(bool isRequired, String usageText, Parameter<T> parameter)
            : base(isRequired, usageText)
        {
            if (parameter == null)
                throw new Exception(); // TODO ?
            this.parameter = parameter;
        }
    }

    /**
    * Class thrown when a value is specified for a SimpleOption.
    * SimpleOption does not have a parameter 
    * 
    */
    public class ForbiddenParameterException : EasyOptException
    {
        public ForbiddenParameterException(string message)
            : base(message)
        { }

        public ForbiddenParameterException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public interface IOptionContainer
    {
        void Add(IOption option, params String[] names);

        void Add(IOption option, char shortName);

        void Add(IOption option, char shortName, String longName);

        IOption FindByName(string name);

        bool ContainsName(string name);

        List<string> ListUniqueNames();

        string[] FindSynonymsByName(string name);
    }

    public class OptionContainer : IOptionContainer
    {
        private Dictionary<String, IOption> options;

        private Dictionary<String, String[]> names;

        public OptionContainer()
        {
            this.options = new Dictionary<string, IOption>();
            this.names = new Dictionary<string, string[]>();
        }

        public List<string> ListUniqueNames()
        {
            return new List<string>(this.names.Keys);

        }

        public string[] FindSynonymsByName(string name)
        {
            return this.names[name];
        }

        public void Add(IOption option, params String[] names)
        {
            // checkConfiguration(option); ?
            foreach (String name in names)
            {
                this.options.Add(name, option);
            }

            this.names.Add(names[0], names);
        }

        public void Add(IOption option, char shortName)
        {
            Add(option, new String[] { shortName.ToString() });
        }

        public void Add(IOption option, char shortName, String longName)
        {
            Add(option, new String[] { shortName.ToString(), longName });
        }

        public IOption FindByName(string name)
        {
            return this.options[name];
        }

        public bool ContainsName(string name)
        {
            return this.options.ContainsKey(name);
        }
    }

}