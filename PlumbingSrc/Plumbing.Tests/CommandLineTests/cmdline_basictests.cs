#if false
namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Helpers;
    using Xunit;

    public class CommandLineSupportUnitTests {
        private Bilge b = new Bilge(nameof(CommandLineSupportUnitTests));

        public CommandLineSupportUnitTests() {
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

#region TestMethod Helpers

        /// <summary>
        /// Used so that when we change the definition of SampleCommandLine_C1 we dont break all of the unit tests in a strange way.
        /// </summary>
        /// <param name="sc">A SampleCommandLine_C1 class</param>
        private static void VerifySampleCommandLine1_InitialState(SampleCommandLine_C1 sc) {
            Assert.Equal(string.Empty, sc.NameParameterOne);
            Assert.Equal(string.Empty, sc.NameParameterTwo);

            Assert.False(sc.OptionParameterOne, "Boolean parameter one invalid start state");
            Assert.False(sc.OptionParameterTwo, "Boolean argument two invalid start state");

            Assert.Equal(0, sc.NumberParameterOne);
            Assert.Equal(0, sc.NumberParameterTwo);
        }

        private static void VerifySampleCommandLine2_InitialSate(SampleCommandLine_C2 sc) {
            Assert.True(sc.Filename == null, "The filename for SC2 is not null, when it should be");
        }

#endregion

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void IntegerParameter_TooLarge_ReturnsCorrectError() {
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;

            SampleCommandLine_C2 argsClass = new SampleCommandLine_C2();
            string[] args = new string[] {
                "filename:this is the filename",
                "INTVALUE:2200000000",
            };
            clas.ProcessArguments(argsClass, args);

            string[] erroredArguments = clas.ListArgumentsWithErrors();

            Assert.True(erroredArguments.Length > 0, "there was an errored argument");
            Assert.True(erroredArguments[0].Contains("INTVALUE"), "The correct argument was not found");
            Assert.True(erroredArguments[0].Contains("too large"), "The incorrect reason was found");
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void TestIntAndLogMaxValues() {
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;

            SampleCommandLine_C2 argsClass = new SampleCommandLine_C2();
            string[] args = new string[] {
                "filename:this is the filename",
                "INTVALUE:2147483647",
                "LONGVALUE:9223372036854775807"
            };
            clas.ProcessArguments(argsClass, args);
            Assert.Equal(int.MaxValue, argsClass.NumberParam1);
            Assert.Equal(long.MaxValue, argsClass.NumberParam2);
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_GetAndSetProperties() {
            CommandArgumentSupport clas = new CommandArgumentSupport();
            Assert.Equal(string.Empty, clas.ArgumentPostfix);
            Assert.Equal("-", clas.ArgumentPrefix);
            Assert.False(clas.ArgumentPrefixOptional);

            clas.ArgumentPostfix = "X";
            Assert.Equal("X", clas.ArgumentPostfix);

            clas.ArgumentPostfix = "Xx1;@~#+==--!2";
            Assert.Equal("Xx1;@~#+==--!2", clas.ArgumentPostfix);

            clas.ArgumentPrefix = "X";
            Assert.Equal("X", clas.ArgumentPrefix);

            clas.ArgumentPrefix = "Xx1;@~#+==--!2";
            Assert.Equal("Xx1;@~#+==--!2", clas.ArgumentPrefix);
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_BooleanParameters() {
            b.Info.Log("Starting  Testing boolean behaviour");

            SampleCommandLine_C1 sc1 = new SampleCommandLine_C1();
            SampleCommandLine_C1 sc2 = new SampleCommandLine_C1();
            SampleCommandLine_C1 sc3 = new SampleCommandLine_C1();

            VerifySampleCommandLine1_InitialState(sc1);
            VerifySampleCommandLine1_InitialState(sc2);
            VerifySampleCommandLine1_InitialState(sc3);

            string[] argsBothTrue = new string[] { "/OP1", "/OP2" };
            string[] argsBothTrue2 = new string[] { "/OP1true", "/OP2TRUE" };
            string[] argsBothTrue3 = new string[] { "/OP1YES", "/OP2y" };

            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ProcessArguments(sc1, argsBothTrue);

            Assert.True(sc1.OptionParameterOne);
            Assert.True(sc1.OptionParameterTwo);

            clas.ProcessArguments(sc2, argsBothTrue2);
            clas.ProcessArguments(sc3, argsBothTrue3);

            Assert.Equal(sc1.OptionParameterOne, sc2.OptionParameterOne);
            Assert.Equal(sc1.OptionParameterTwo, sc2.OptionParameterTwo);
            Assert.Equal(sc1.OptionParameterOne, sc3.OptionParameterOne);
            Assert.Equal(sc1.OptionParameterTwo, sc3.OptionParameterTwo);

            string[] argsBothFalse = new string[] { "/OP1false", "/OP2no" };
            string[] argsBothFalse2 = new string[] { "/OP1N", "/OP2False" };

            sc1.OptionParameterOne = true;
            sc1.OptionParameterTwo = true;
            clas.ProcessArguments(sc1, argsBothFalse);

            Assert.False(sc1.OptionParameterOne);
            Assert.False(sc1.OptionParameterTwo);

            sc1.OptionParameterOne = true;
            sc1.OptionParameterTwo = true;
            clas.ProcessArguments(sc1, argsBothFalse2);

            Assert.False(sc1.OptionParameterOne);
            Assert.False(sc1.OptionParameterTwo);
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_NumberStringAndBoolParams() {
            b.Info.Log("Starting test for SimpleArguments");

            const string STRINGPAR1 = "StringParameter1";
            const string STRINGPAR2 = "StrnigParameter2";
            const int NUMVALUE = 12;
            SampleCommandLine_C1 sc1 = new SampleCommandLine_C1();

            VerifySampleCommandLine1_InitialState(sc1);

            string[] args = new string[] { "/N:" + STRINGPAR1, "/A:" + STRINGPAR2, "/C:" + NUMVALUE.ToString(), "/OP1" };

            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";

            clas.ProcessArguments(sc1, args);

            Assert.Equal(STRINGPAR1, sc1.NameParameterOne);
            Assert.Equal(STRINGPAR2, sc1.NameParameterTwo);

            Assert.True(sc1.OptionParameterOne, "Boolean parameter one failed");
            Assert.False(sc1.OptionParameterTwo, "Boolean argument two failed");

            Assert.Equal(12, sc1.NumberParameterOne);
            Assert.Equal(0, sc1.NumberParameterTwo);
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_ArgumentPrefix() {
            b.Info.Log("Starting  Testing Argument prefix behaviour");

            SampleCommandLine_C1 sc1 = new SampleCommandLine_C1();
            VerifySampleCommandLine1_InitialState(sc1);

            // Should be identical with differing prefixes.
            string[] argsBothTrue = new string[] { "-OP1", "-OP2" };
            string[] argsBothTrue2 = new string[] { "XOP1", "XOP2" };
            string[] argsBothTrue3 = new string[] { "OP1", "OP2" };

            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "-";
            sc1.OptionParameterOne = sc1.OptionParameterTwo = false;
            clas.ProcessArguments(sc1, argsBothTrue);

            Assert.True(sc1.OptionParameterOne);
            Assert.True(sc1.OptionParameterTwo);

            sc1.OptionParameterOne = sc1.OptionParameterTwo = false;
            clas.ArgumentPrefix = "X";
            clas.ProcessArguments(sc1, argsBothTrue2);

            Assert.True(sc1.OptionParameterOne);
            Assert.True(sc1.OptionParameterTwo);

            sc1.OptionParameterOne = sc1.OptionParameterTwo = false;
            clas.ArgumentPrefix = "";
            clas.ProcessArguments(sc1, argsBothTrue3);

            Assert.True(sc1.OptionParameterOne);
            Assert.True(sc1.OptionParameterTwo);

            b.Info.Log("TESTOK: Finished Testing ArgumentPrefix");
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_DefaultArguments() {
            SampleCommandLine_C2 c2 = new SampleCommandLine_C2();
            VerifySampleCommandLine2_InitialSate(c2);
            string[] args = new string[] { "filename.txt" };

            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "-";
            clas.ArgumentPostfix = "=";
            clas.ProcessArguments(c2, args);

            Assert.Equal("filename.txt", c2.Filename);

            args[0] = "-filename=filename.txt";
            c2 = new SampleCommandLine_C2();
            VerifySampleCommandLine2_InitialSate(c2);
            clas.ProcessArguments(c2, args);

            Assert.Equal("filename.txt", c2.Filename);
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_GenerateShortHelp() {
            CommandArgumentSupport clas = new CommandArgumentSupport();
            SampleCommandLine_C2 c2 = new SampleCommandLine_C2();

            string help = clas.GenerateShortHelp(c2, "Tests");
            b.Info.Log("Help: " + help);
            Assert.True(help.Contains("~~MatchShortDescrFilename~~"), "The help message does not contain the short string");
        }

        [Fact][Trait("xunit","regression")] // Legacy Tests, replace when working on them.
        public void BasicTest_MultipleArgumentsSameValue() {
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = "=";

            TFSBuildToolArgs tbta1 = new TFSBuildToolArgs();
            TFSBuildToolArgs tbta2 = new TFSBuildToolArgs();

            string[] args = new string[] { "/tfs=first", "/agent=second", "/teamproject=acme" };
            string[] args2 = new string[] { "/tfs=first", "/agenturi=second", "/teamproject=acme" };

            clas.ProcessArguments(tbta1, args);
            clas.ProcessArguments(tbta2, args2);

            Assert.Equal(tbta1.agentUri, tbta2.agentUri);
        }
    }
}
#endif