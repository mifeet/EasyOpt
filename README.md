EasyOpt
=======

**EasyOpt is a library offering an easy-to-use API for work with program options and arguments.** It is written in C# and can be used by any application written for .NET.

### Features

With EasyOpt, you can:

* Specify program options, whether an option is optional/required, define multiple synonymous names for an option
* Define option parameters and access their values converted to a proper type
* Apply constraints parameter values
* Validate that options passed by the user are correct and satisfy all constraints
* Retrieve non-option arguments
* Use predefined kinds of parameters and constraints or define your own ones
* Easily generate usage screen


### Usage example

Here is a simple example showing how you can use EasyOpt:

```c#
static void Main(string[] args) {
  EasyOpt parser = new EasyOpt();
 
  var verbose = OptionFactory.Create(false, "Be more verbose");
  parser.AddOption(verbose, 'v', "verbose");
 
  var fileParam = new StringParameter(true, "FILE");
  var file = OptionFactory.Create(false, "Write output to FILE", fileParam);
  parser.AddOption(file, 'f', "file");
 
  var lineWidthParam = new IntParameter(true, "WIDTH", 80);
  lineWidthParam.AddConstraint(new LowerBoundConstraint(1));
  var lineWidth = OptionFactory.Create(false, "Wrap lines at WIDTH", lineWidthParam);
  parser.AddOption(lineWidth, "line-width");
 
  parser.UsageDescription = "program [OPTION]... [FILE]...";
 
  try {
    parser.Parse(args);
  }
  catch (EasyOptException e) {
    Console.Write( parser.GetUsage() );
    return;
  }
 
  String[] arguments = parser.GetArguments();
 
  bool isVerbose = verbose.Value;
  String outputFile = file.Value;
  int maxLineWidth = lineWidth.Value;
}
```

The usage message for the program above would look like this:

```
program [OPTION]... [FILE]...

    -v, --verbose
        Be more verbose

    -f, --file=FILE
        Write output to FILE

    --line-width=WIDTH
        Wrap lines at WIDTH
```

### How To Use EasyOpt

In order to start using EasyOpt, you need to add reference to EasyOpt library .dll file to your project. All classes that EasyOpt consists of are in namespace EasyOptLibrary. In C# you can import this namespace with

    using EasyOptLibrary;
    
Using the library to parse program arguments can be divided into three steps:

1. Define program options, their parameters and constraints.
2. Parse arguments from the command line.
3. Retrieve values of options and non-option arguments.


#### Defining program options

First, you need to create an instance of EasyOpt:

```c#
EasyOpt parser = new EasyOpt();
```

Each option is represented by an instance of generic class `Option<T>`. To create an instance of `Option<T>`, use factory methods provided by `OptionFactory`.

There are two basic types of options: with and without an option parameter.

To create an option without a parameter, you simply call `OptionFactory.Create()` with two arguments — the first one specifies whether the option is required (`true`) or optional (`false`), the second one contains option description for usage text. Example:

```c#
var verbose = OptionFactory.Create(false, "Be more verbose");
```

In order to create an option with a parameter, you need to create instance of class `Parameter<T>`. This is an abstract generic class. EasyOpt provides derived classes for several concrete types: `StringParameter`, `IntParameter`, `FloatParameter` and `EnumParameter<T>`. Each of these classes automatically convert parameter value to their respective type — `string`, `int`, `float` and an enum type `T`.

In constructors for these classes, you provide three arguments — whether the option is required (`true`) or optional (`false`), name displayed in the usage text and default value. Example:

```c#
var lineWidthParam = new IntParameter(true, "WIDTH", 80);
```

Next, you create an option object the same way as in case of an option without a parameter, but you pass `Parameter<T>` object as the third argument of `OptionFactory.Create()`. This method is generic, but thanks to type inference in C# you don't need to specify the type explicitely. Thus the following two lines of code are equivalent:

```c#
var lineWidth = OptionFactory.Create(false, "Wrap lines at WIDTH", lineWidthParam);
Option<int> lineWidth = OptionFactory.Create<int>(false, "Wrap lines at WIDTH", lineWidthParam);
```

Finally, you register the option object to EasyOpt instance and assign it names with `AddOption()`. You can specify multiple names. Names one character long are processed as short option names, longer names are processed as long option names. For convenience there are several overloaded versions of `AddOption()`. The following lines are equivalent:

```c#
parser.AddOption(verbose, 'v', "verbose");
parser.AddOption(verbose, "v", "verbose");
parser.AddOption(verbose, new string[] { "v", "verbose" } );
```

The last feature EasyOpt provides for options and parameters are constraints. Constraints define what parameter values are valid. For example you can define that an option parameter must be a number greater than zero or that a parameter value must be in a certain list of accepted values. In EasyOpt, constraints are classes implementing `IConstraint<T>` interface. EasyOpt has several predefined constraints — `LowerBoundConstraint`, `UpperBoundConstraint`, `StringEnumerationConstraint` and `ExistingFileConstraint`. Every constraint must define method `IsValid(T parameter)`. It accepts parameter value converted to a proper type (e.g. `int` for `IntParameter`). If `IsValid()` returns `true`, the parameter is accepted, otherwise the parser throws an exception.

To apply a constraint to an option parameter, create its instance and register it to the parameter object. You can define multiple constraints for a single parameter.

```c#
lineWidthParam.AddConstraint( new LowerBoundConstraint(1) );
lineWidthParam.AddConstraint( new UpperBoundConstraint(100) );
```

#### Parsing arguments

After you've set all options, it's time to get to the actual argument parsing. You do that simply by calling `EasyOpt.Parse()`. As its only argument you pass arguments from the command line in a string array:

```c#
parser.Parse(args);
```

If the parsed options do not comply with option settings (e.g. unknown option name, option parameter doesn't satisfy a constraint etc.), an exception is thrown. All parsing exceptions are derived from `EasyOptException`, so that you can easily catch them and display the usage text:

```c#
try {
  parser.Parse(args);
}
catch (EasyOptException e) {
  Console.Write( parser.GetUsage() );
}
```

You can customize the usage text by setting `EasyOpt.UsageDescription` property. `GetUsage()` returns the value of `EasyOpt.UsageDescription` plus the description of all registered options.

#### Retrieving option values & non-option arguments

After you've called the `Parse()` method, option and parameter instances are filled with the parsed values. You can find out whether an option was present among the parsed arguments using the `IsPresent` property:

```c#
bool isVerbose = verbose.IsPresent;
```

To retrieve a parameter value, use the `Parameter<T>.Value` property. If the value hasn't been specified in the parsed arguments, it returns the default value.

```c#
int maxLineWidth = lineWidthParam.Value;
```

Since in case of options without a parameter, one is usually interested in `IsPresent` value and in case of options with a parameter, one is interested in the parameter's value, you can easily access the relevant information uniformly with the `Option<T>.Value` property:

```c#
bool isVerbose = verbose.Value;
int maxLineWidth = lineWidth.Value;
```

Non-option arguments can be retrieved as a string array:

```c#
String[] arguments = parser.GetArguments();
```

### Detailed description

You can find more detailed description of EasyOpt in the following wiki pages:

* [Configuring options](https://github.com/mifeet/EasyOpt/wiki/Configuring-Options)
* [Argument parsing](https://github.com/mifeet/EasyOpt/wiki/Argument-Parsing)
* [Retrieving values & arguments](https://github.com/mifeet/EasyOpt/wiki/Retrieving-Values-&-Arguments)
* [Usage text](https://github.com/mifeet/EasyOpt/wiki/Usage-Text)

### More examples

More examples of how EasyOpt can be used can be found on a [special page](https://github.com/mifeet/EasyOpt/wiki/Usage-Examples).
