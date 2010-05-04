using EasyOpt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace TestEasyOpt
{
    
    
    /// <summary>
    ///This is a test class for ArgumentTest and is intended
    ///to contain all ArgumentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArgumentTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestDivision()
        {
            IOptionContainer optionContainer = null;

            List<Token> arguments = Token.Create("--", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.Division, actualArgument.Type);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.Name);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestShortOption()
        {
            IOptionContainer optionContainer = new OptionContainer();
            IOption option = OptionFactory.Create(true, "help");
            optionContainer.Add(option, 'v');

            List<Token> arguments = Token.Create("-v", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.ShortOption, actualArgument.Type);
            Assert.AreEqual("v", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestShortOptionWithArgument()
        {
            IOptionContainer optionContainer = new OptionContainer();
            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> option = OptionFactory.Create<string>(true, "help", stringParameter);
            optionContainer.Add(option, 'v');

            List<Token> arguments = Token.Create("-vparameter", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.ShortOption, actualArgument.Type);
            Assert.AreEqual("v", actualArgument.Name);
            Assert.AreEqual("parameter", actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }
        [TestMethod()]
        public void CreateTestLongOption()
        {
            IOptionContainer optionContainer = null;

            List<Token> arguments = Token.Create("--v", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.ProgramArgument, actualArgument.Type);
            Assert.IsNull(actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.AreEqual("--v", actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestLongOptionWithDash()
        {
            IOptionContainer optionContainer = new OptionContainer();
            IOption option = OptionFactory.Create(true, "help");
            optionContainer.Add(option,'l', "long-option");

            List<Token> arguments = Token.Create("--long-option", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.LongOption, actualArgument.Type);
            Assert.AreEqual("long-option", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestLongOptionEqual()
        {
            IOptionContainer optionContainer = new OptionContainer();
            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> option = OptionFactory.Create<string>(true, "help", stringParameter);
            optionContainer.Add(option, 'o', "option");

            List<Token> arguments = Token.Create("--option=3test", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(1, arguments.Count);

            Assert.AreEqual(TokenType.LongOption, actualArgument.Type);
            Assert.AreEqual("option", actualArgument.Name);
            Assert.AreEqual("3test", actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestShortName()
        {

            Assert.AreEqual(true, Token.IsShortNameValid("a"));
            Assert.AreEqual(false, Token.IsShortNameValid(";"));

            Assert.AreEqual(false, Token.IsShortNameValid("long-integer"));
            Assert.AreEqual(false, Token.IsShortNameValid("long;"));

        }

        [TestMethod()]
        public void CreateTestLongName()
        {
            Assert.AreEqual(true, Token.IsLongNameValid("long-integer"));
            Assert.AreEqual(false, Token.IsLongNameValid("long;"));

            Assert.AreEqual(false, Token.IsLongNameValid("a"));
            Assert.AreEqual(false, Token.IsLongNameValid(";"));
        }

        [TestMethod()]
        public void CreateTestABC()
        {
            IOptionContainer optionContainer = new OptionContainer();
            IOption option = OptionFactory.Create(true, "help");
            optionContainer.Add(option, 'a');
            optionContainer.Add(option, 'b');
            optionContainer.Add(option, 'c');

            List<Token> arguments = Token.Create("-abc", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(3, arguments.Count);

            Assert.AreEqual(TokenType.ShortOption, actualArgument.Type);
            Assert.AreEqual("a", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);

            Token actualArgument2 = arguments[1];

            Assert.AreEqual(TokenType.ShortOption, actualArgument2.Type);
            Assert.AreEqual("b", actualArgument2.Name);
            Assert.IsNull(actualArgument2.Parameter);
            Assert.IsNull(actualArgument2.ProgramArgument);

            Token actualArgument3 = arguments[2];

            Assert.AreEqual(TokenType.ShortOption, actualArgument3.Type);
            Assert.AreEqual("c", actualArgument3.Name);
            Assert.IsNull(actualArgument3.Parameter);
            Assert.IsNull(actualArgument3.ProgramArgument);
        }


        [TestMethod()]
        public void CreateTestABCD()
        {
            IOptionContainer optionContainer = new OptionContainer();
            IOption option = OptionFactory.Create(true, "help");
            optionContainer.Add(option, 'a');
            optionContainer.Add(option, 'b');

            StringParameter stringParameter = new StringParameter(true, "help");
            Option<string> optionC = OptionFactory.Create<string>(true, "help", stringParameter);
            optionContainer.Add(optionC, 'c');

            List<Token> arguments = Token.Create("-abcd", optionContainer);
            Token actualArgument = arguments[0];

            Assert.AreEqual(3, arguments.Count);

            Assert.AreEqual(TokenType.ShortOption, actualArgument.Type);
            Assert.AreEqual("a", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);

            Token actualArgument2 = arguments[1];

            Assert.AreEqual(TokenType.ShortOption, actualArgument2.Type);
            Assert.AreEqual("b", actualArgument2.Name);
            Assert.IsNull(actualArgument2.Parameter);
            Assert.IsNull(actualArgument2.ProgramArgument);

            Token actualArgument3 = arguments[2];

            Assert.AreEqual(TokenType.ShortOption, actualArgument3.Type);
            Assert.AreEqual("c", actualArgument3.Name);
            Assert.AreEqual("d", actualArgument3.Parameter);
            Assert.IsNull(actualArgument3.ProgramArgument);
        }

    }
}
