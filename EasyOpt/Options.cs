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
            set;
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
            throw new Exception(); // TODO: on the command line, a parameter was passed to an option without a parameter
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

        public ParameterOption(bool isRequired, String usageText, Parameter<T> parameter)
            : base(isRequired, usageText)
        {
            if (parameter == null)
                throw new Exception(); // TODO ?
            this.parameter = parameter;
        }
    }
}