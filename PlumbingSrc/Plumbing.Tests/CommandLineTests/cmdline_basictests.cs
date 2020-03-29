namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class CommandLineSupportUnitTests {
        private Bilge b = new Bilge(nameof(CommandLineSupportUnitTests));

        public CommandLineSupportUnitTests() {
        }



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


        [Fact(DisplayName = nameof(Bug_IfBilgePassed_NullRefException))]
        [Trait(Traits.Style, Traits.Regression)]
        public void Bug_IfBilgePassed_NullRefException() {
            b.Info.Flow();
            var clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            string[] args = new string[] {
                "firstonethenanother:gloop",
            };

            var argsClass = new SampleCommandLine_C6();
            clas.ProcessArguments(argsClass, args);
        }



        [Fact(DisplayName = nameof(Bug_IfPostfixSpecified_ItMustBeUsed))]
        [Trait(Traits.Style, Traits.Regression)]
        public void Bug_IfPostfixSpecified_ItMustBeUsed() {
            b.Info.Flow();
            var clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;


            var argsClass = new SampleCommandLine_C6();
            string[] args = new string[] {
                "firstonethenanother:gloop",
                "firstone:barfle",
                "first:arfle"

            };

            clas.ProcessArguments(argsClass, args);

            Assert.Equal("arfle", argsClass.first);
            Assert.Equal("barfle", argsClass.second);
            Assert.Equal("gloop", argsClass.third);
        }



        [Fact(DisplayName = nameof(Required_ThrowsIfNotPresent))]
        [Trait(Traits.Style, Traits.Regression)]
        public void Required_ThrowsIfNotPresent() {
            b.Info.Flow();

            var clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;


            var argsClass = new SampleCommandLine_C5();
            string[] args = new string[] {
                "firstx:hello",
                "second:1"
            };


            Assert.Throws<ArgumentNullException>(() => {
                clas.ProcessArguments(argsClass, args);

                if (argsClass.first != null) {
                    throw new InvalidOperationException("This cant be right, the value should not be set");
                }
            });

        }


        public static IEnumerable<object[]> DateTimeData {
            get {
                // Or this could read from a file. :)
                return new[]
                {
                new object[] { new DateTime(2018,1, 1), "1-1-2018" },
                new object[] { new DateTime(2018,1, 1), "01-1-2018" },
                new object[] { new DateTime(2018,1, 1), "01-01-2018" },
                new object[] { new DateTime(2018,1, 1), "1-01-2018" },
                new object[] { new DateTime(2018, 12, 31), "31-12-2018" },
                new object[] { new DateTime(2018, 1, 30), "30-1-2018" }
            };
            }
        }


        [Theory(DisplayName = nameof(DateTime_StringParse_Works))]
        [Trait("age", "fresh")]
        [Trait(Traits.Style, "exploratory")]
        [MemberData(nameof(DateTimeData))]
        public void DateTime_StringParse_Works(DateTime exp, string text) {
            b.Info.Flow();

            var clas = new CommandArgumentSupport();
            clas.DateTimeParseFormat = "d-M-yyyy";
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;


            var argsClass = new SampleCommandLine_C4();
            string[] args = new string[] {
                $"dt1:{text}"
            };

            clas.ProcessArguments(argsClass, args);

            Assert.Equal(exp, argsClass.datey1);
        }


        [Theory(DisplayName = nameof(DateTime_BasicParse_Works))]
        [Trait(Traits.Style, Traits.Regression)]
        [InlineData(2018, 11, 22)]
        [InlineData(2000, 1, 1)]
        [InlineData(1945, 11, 11)]
        [InlineData(2099, 12, 12)]
        public void DateTime_BasicParse_Works(int year, int month, int day) {
            b.Info.Flow();

            DateTime target = new DateTime(year, month, day);

            var clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;


            var argsClass = new SampleCommandLine_C4();
            string[] args = new string[] {
                "dt1:"+target.ToString("dd-MM-yyyy")
            };

            clas.ProcessArguments(argsClass, args);

            Assert.Equal<DateTime>(target, argsClass.datey1);
        }


        [Fact(DisplayName = nameof(IntegerParameter_TooLarge_ReturnsCorrectError))]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void IntegerParameter_TooLarge_ReturnsCorrectError() {
            var clas = new CommandArgumentSupport();
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefix = "";
            clas.ArgumentPrefixOptional = true;

            var argsClass = new SampleCommandLine_C2();
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

        [Fact(DisplayName = nameof(TestIntAndLogMaxValues))]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
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



        [Fact(DisplayName = nameof(Basic_ArrayOfInts_Works))]
        [Trait(Traits.Style, "exploratory")] // Legacy Tests, replace when working on them.
        public void Basic_ArrayOfInts_Works() {

            var clas = new CommandArgumentSupport {
                ArgumentPostfix = ":",
                ArgumentPrefix = "",
                ArgumentPrefixOptional = true
            };

            int[] nums = new int[] { 1, 2, 3, 4, 5 };
            string numsAsParam = "";
            foreach (var f in nums) {
                numsAsParam += f + ",";
            }

            var argsClass = new SampleCommandLine_C3();
            string[] args = new string[] {
                $"IntArray:{numsAsParam}",
            };

            clas.ProcessArguments(argsClass, args);

            Assert.Equal(nums.Length, argsClass.NumArray.Length);
            for (int i = 0; i < nums.Length; i++) {
                Assert.Equal(nums[i], argsClass.NumArray[i]);
            }


        }


        [Fact(DisplayName = nameof(Basic_ArrayOfStrs_Works))]
        [Trait(Traits.Style, "exploratory")] // Legacy Tests, replace when working on them.
        public void Basic_ArrayOfStrs_Works() {

            var clas = new CommandArgumentSupport {
                ArgumentPostfix = ":",
                ArgumentPrefix = "",
                ArgumentPrefixOptional = true
            };

            string[] parms = new string[] { "one", "two", "three", "four", "five" };

            string strArrayConcat = "";
            foreach (var f in parms) {
                strArrayConcat += f + ",";
            }


            var argsClass = new SampleCommandLine_C3();
            string[] args = new string[] {
                $"StrArray:{strArrayConcat}",
            };
            clas.ProcessArguments(argsClass, args);

            Assert.Equal(parms.Length, argsClass.StrArray.Length);
            for (int i = 0; i < parms.Length; i++) {
                Assert.Equal(parms[i], argsClass.StrArray[i]);
            }
        }


        [Fact(DisplayName = nameof(BasicTest_GetAndSetProperties))]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_GetAndSetProperties() {
            var clas = new CommandArgumentSupport();
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

        [Fact]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_BooleanParameters() {
            b.Info.Log("Starting  Testing boolean behaviour");

            var sc1 = new SampleCommandLine_C1();
            var sc2 = new SampleCommandLine_C1();
            var sc3 = new SampleCommandLine_C1();

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

        [Fact]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_NumberStringAndBoolParams() {
            b.Info.Log("Starting test for SimpleArguments");

            const string STRINGPAR1 = "StringParameter1";
            const string STRINGPAR2 = "StrnigParameter2";
            const int NUMVALUE = 12;

            var sc1 = new SampleCommandLine_C1();

            VerifySampleCommandLine1_InitialState(sc1);

            string[] args = new string[] { "/N:" + STRINGPAR1, "/A:" + STRINGPAR2, "/C:" + NUMVALUE.ToString(), "/OP1" };

            var clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";

            clas.ProcessArguments(sc1, args);

            Assert.Equal(STRINGPAR1, sc1.NameParameterOne);
            Assert.Equal(STRINGPAR2, sc1.NameParameterTwo);

            Assert.True(sc1.OptionParameterOne, "Boolean parameter one failed");
            Assert.False(sc1.OptionParameterTwo, "Boolean argument two failed");

            Assert.Equal(12, sc1.NumberParameterOne);
            Assert.Equal(0, sc1.NumberParameterTwo);
        }

        [Fact]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
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


        [Fact(DisplayName = nameof(BasicTest_DefaultArguments))]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_DefaultArguments() {
            var c2 = new SampleCommandLine_C2();
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

        [Fact]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_GenerateShortHelp() {
            var clas = new CommandArgumentSupport();
            var c2 = new SampleCommandLine_C2();

            string help = clas.GenerateShortHelp(c2, "Tests");
            b.Info.Log("Help: " + help);
            Assert.True(help.Contains("~~MatchShortDescrFilename~~"), "The help message does not contain the short string");
        }

        [Fact(DisplayName = nameof(BasicTest_MultipleArgumentsSameValue))]
        [Trait(Traits.Style, Traits.Regression)] // Legacy Tests, replace when working on them.
        public void BasicTest_MultipleArgumentsSameValue() {
            var clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = "=";

            var tbta1 = new TFSBuildToolArgs();
            var tbta2 = new TFSBuildToolArgs();

            string[] args = new string[] { "/tfs=first", "/agent=second", "/teamproject=acme" };
            string[] args2 = new string[] { "/tfs=first", "/agenturi=second", "/teamproject=acme" };

            clas.ProcessArguments(tbta1, args);
            clas.ProcessArguments(tbta2, args2);

            Assert.Equal(tbta1.agentUri, tbta2.agentUri);
        }
    }
}
