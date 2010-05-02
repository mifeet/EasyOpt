using EasyOpt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Argument actualArgument = Argument.Create("--");
            Assert.AreEqual(ArgumentType.division, actualArgument.Type);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.Name);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestShortOption()
        {
            Argument actualArgument = Argument.Create("-v");
            Assert.AreEqual(ArgumentType.shortOption, actualArgument.Type);
            Assert.AreEqual("v", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestShortOptionWithArgument()
        {
            Argument actualArgument = Argument.Create("-vparameter");
            Assert.AreEqual(ArgumentType.shortOption, actualArgument.Type);
            Assert.AreEqual("v", actualArgument.Name);
            Assert.AreEqual("parameter", actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }
        [TestMethod()]
        public void CreateTestLongOption()
        {
            Argument actualArgument = Argument.Create("--v");
            Assert.AreEqual(ArgumentType.longOption, actualArgument.Type);
            Assert.AreEqual("v", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestLongOptionWithDash()
        {
            Argument actualArgument = Argument.Create("--long-option");
            Assert.AreEqual(ArgumentType.longOption, actualArgument.Type);
            Assert.AreEqual("long-option", actualArgument.Name);
            Assert.IsNull(actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

        [TestMethod()]
        public void CreateTestLongOptionEqual()
        {
            Argument actualArgument = Argument.Create("--option=3test");
            Assert.AreEqual(ArgumentType.longOption, actualArgument.Type);
            Assert.AreEqual("option", actualArgument.Name);
            Assert.AreEqual("3test", actualArgument.Parameter);
            Assert.IsNull(actualArgument.ProgramArgument);
        }

    }
}
