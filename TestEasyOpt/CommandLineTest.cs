using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyOpt;

namespace TestEasyOpt
{
    /// <summary>
    /// Summary description for CommandLineTest
    /// </summary>
    [TestClass]
    public class CommandLineTest
    {
        public CommandLineTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestParseSimple()
        {
            Option<bool> format = OptionFactory.Create(true, "Format 24h");

            CommandLine commandLine = new CommandLine(new string [] {"-f"});

            commandLine.AddOption(format, 'f');

            commandLine.Parse();

            Assert.AreEqual(true, format.IsPresent);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException), "Throw exception because a is not defined as a single option.")]
        public void TestParseSimpleWithParam()
        {
            Option<bool> format = OptionFactory.Create(true, "Format 24h");

            CommandLine commandLine = new CommandLine(new string[] { "-fa" });

            commandLine.AddOption(format, 'f');

            commandLine.Parse();
        }

        [TestMethod]
        public void TestParseABCD()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");
            Option<bool> b = OptionFactory.Create(true, "Format 24h");

            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> c = OptionFactory.Create<string>(true, "help", stringParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });


            commandLine.AddOption(a, 'a');
            commandLine.AddOption(b, 'b');
            commandLine.AddOption(c, 'c');

            commandLine.Parse();


            Assert.IsTrue(a.IsPresent);
            Assert.IsTrue(b.IsPresent);
            Assert.IsTrue(c.IsPresent);

            Assert.AreEqual("d", c.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException), "Throw exception because c is not present in the command line arguments.")]
        public void TestParseABCDRequired()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");
            Option<bool> b = OptionFactory.Create(true, "Format 24h");

            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> c = OptionFactory.Create<string>(true, "help", stringParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-ab" });


            commandLine.AddOption(a, 'a');
            commandLine.AddOption(b, 'b');
            commandLine.AddOption(c, 'c');

            commandLine.Parse();


            Assert.IsTrue(a.IsPresent);
            Assert.IsTrue(b.IsPresent);
            Assert.IsTrue(c.IsPresent);

            Assert.AreEqual("d", c.Value);
        }

        [TestMethod]
        public void TestParseABCDOptional()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");
            Option<bool> b = OptionFactory.Create(true, "Format 24h");

            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> c = OptionFactory.Create<string>(false, "help", stringParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-ab" });


            commandLine.AddOption(a, 'a');
            commandLine.AddOption(b, 'b');
            commandLine.AddOption(c, 'c');

            commandLine.Parse();


            Assert.IsTrue(a.IsPresent);
            Assert.IsTrue(b.IsPresent);
            Assert.IsFalse(c.IsPresent);

        }




        [TestMethod]
        public void TestParseInt()
        {
            IntParameter intParameter = new IntParameter(true, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i233" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

            Assert.AreEqual(233, intOption.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException), "Invalid integer parameter.")]
        public void TestParseIntParameterException()
        {
            IntParameter intParameter = new IntParameter(true, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-iaaa" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

        }

        [TestMethod]
        [ExpectedException(typeof(ParseException), "Option requires a parameter.")]
        public void TestParseIntMissingParameterException()
        {
            IntParameter intParameter = new IntParameter(true, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

        }

        [TestMethod]
        public void TestParseIntOptionalParameterMissing()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

            Assert.AreEqual(0, intOption.Value);
        }

        [TestMethod]
        public void TestParseIntOptionalParameter()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i334" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

            Assert.AreEqual(334, intOption.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException), "Unknown option.")]
        public void TestParseUnknownOption()
        {
            IntParameter intParameter = new IntParameter(true, "Integer parameter");

            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-f334" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

        }

        [TestMethod]
        public void TestParseOnlyProgramArguments()
        {
            CommandLine commandLine = new CommandLine(new string[] { "334", "28", "a" });

            commandLine.Parse();

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(3, arguments.Length);
            Assert.AreEqual("334", arguments[0]);
            Assert.AreEqual("28", arguments[1]);
            Assert.AreEqual("a", arguments[2]);
        }

        [TestMethod]
        public void TestParseShortOptionAndProgramArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i", "334", "28", "a" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(3, arguments.Length);
            Assert.AreEqual("334", arguments[0]);
            Assert.AreEqual("28", arguments[1]);
            Assert.AreEqual("a", arguments[2]);
        }

        [TestMethod]
        public void TestParseShortOptionDivisionAndProgramArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-i", "--", "334", "28", "a" });

            commandLine.AddOption(intOption, 'i');

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(3, arguments.Length);
            Assert.AreEqual("334", arguments[0]);
            Assert.AreEqual("28", arguments[1]);
            Assert.AreEqual("a", arguments[2]);
        }

        [TestMethod]
        public void TestParseLongOptionAndProgramArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);
            CommandLine commandLine = new CommandLine(new string[] { "--integer", "334", "28", "a" });

            commandLine.AddOption(intOption, 'i', "integer");

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(2, arguments.Length);
            Assert.AreEqual("28", arguments[0]);
            Assert.AreEqual("a", arguments[1]);
        }

        [TestMethod]
        public void TestParseLongOptionDivisionAndProgramArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);
            CommandLine commandLine = new CommandLine(new string[] { "--integer", "--", "334", "28", "a" });

            commandLine.AddOption(intOption, 'i', "integer");

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(3, arguments.Length);
            Assert.AreEqual("334", arguments[0]);
            Assert.AreEqual("28", arguments[1]);
            Assert.AreEqual("a", arguments[2]);
        }

        [TestMethod]
        public void TestParseLongOptionWithEqualAndProgramArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);
            CommandLine commandLine = new CommandLine(new string[] { "--integer=334", "28", "a" });

            commandLine.AddOption(intOption, 'i', "integer");

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);
            Assert.AreEqual(334, intOption.Value);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(2, arguments.Length);
            Assert.AreEqual("28", arguments[0]);
            Assert.AreEqual("a", arguments[1]);
        }

        [TestMethod]
        public void TestParseArguments()
        {
            IntParameter intParameter = new IntParameter(false, "Integer parameter");
            Option<int> intOption = OptionFactory.Create<int>(true, "Integer option", intParameter);

            CommandLine commandLine = new CommandLine(new string[] { "--integer=334", "28", "a" });

            commandLine.AddOption(intOption, 'i', "integer");

            commandLine.Parse();

            Assert.AreEqual(true, intOption.IsPresent);
            Assert.AreEqual(334, intOption.Value);

            String[] arguments = commandLine.GetArguments();

            Assert.AreEqual(2, arguments.Length);
            Assert.AreEqual("28", arguments[0]);
            Assert.AreEqual("a", arguments[1]);

            Console.Out.Write(commandLine.GetUsage());

        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateOptionNameException), "Option names should be unique.")]
        public void TestAddDuplicateName()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");
            Option<bool> b = OptionFactory.Create(true, "Format 24h");

            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> c = OptionFactory.Create<string>(true, "help", stringParameter);

            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });


            commandLine.AddOption(a, 'a');
            commandLine.AddOption(b, 'b');
            commandLine.AddOption(c, 'a');


        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateOptionNameException), "Option names should be unique.")]
        public void TestAddDuplicateNameSameOption()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");


            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });


            commandLine.AddOption(a, 'a', "a");


        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException), "Option names can not be null.")]
        public void TestAddNullNames()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");


            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });

            string[] names = null;

            commandLine.AddOption(a, names);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException), "Option names can not be null.")]
        public void TestAddNullNames2()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");


            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });

            string[] names = new String[0];

            commandLine.AddOption(a, names);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException), "Option names can not be null.")]
        public void TestAddNullNames3()
        {
            Option<bool> a = OptionFactory.Create(true, "Format 24h");


            CommandLine commandLine = new CommandLine(new string[] { "-abcd" });

            string[] names = new String[1];

            commandLine.AddOption(a, names);
        }
    }
}
