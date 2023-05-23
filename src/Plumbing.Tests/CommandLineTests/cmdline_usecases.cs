
namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using Xunit;
    using Xunit.Abstractions;

    public class CommandLineSupport_UseCases {
        private Bilge b = new Bilge();
        private readonly ITestOutputHelper output;

        public CommandLineSupport_UseCases(ITestOutputHelper output) {
            this.output = output;

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



        [Fact(DisplayName = nameof(Sean_DateTime_UseCase))]
        [Trait("age", "fresh")]
        public void Sean_DateTime_UseCase() {
            b.Info.Flow();

            var suc = new Sean_DateUseCase();
            var clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";

            DateTime first = new DateTime(2018, 11, 22);
            DateTime to = new DateTime(2018, 12, 30);

            string[] expectedArgs = new string[] {
                "/f22-11-2018",
                "/t30-12-2018"
            };
            clas.ProcessArguments(suc, expectedArgs);

            Assert.Equal(first, suc.from);
            Assert.Equal(to, suc.to);

        }


        /// <summary>
        /// Used so that when we change the definition of SampleCommandLine_C1 we dont break all of the unit tests in a strange way.
        /// </summary>
        /// <param name="sc">A SampleCommandLine_C1 class</param>
        private static void VerifySampleCommandLine_InitialState(SampleCommandLine_C1 sc) {
            Assert.Equal(string.Empty, sc.NameParameterOne);
            Assert.Equal(string.Empty, sc.NameParameterTwo);

            Assert.False(sc.OptionParameterOne, "Boolean parameter one invalid start state");
            Assert.False(sc.OptionParameterTwo, "Boolean argument two invalid start state");

            Assert.Equal(0, sc.NumberParameterOne);
            Assert.Equal(0, sc.NumberParameterTwo);
        }

        [Fact(DisplayName = nameof(TFSUseCaseRemainderCheck))]
        
        public void TFSUseCaseRemainderCheck() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();

            string[] expectedArgs = new string[] {
                "/BUILDNAME:TFSSupport_DevDesktopPack_Main_Certified_20090423.8",
                "/ATTACHMENT:\"c:\\temp\\list.txt\"",
                "/Remainder:True",
                "RemainderTwo"
            };

            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";

            clas.ProcessArguments(tcc, expectedArgs);

            Assert.Equal(2, tcc.Remainder.Length);
            Assert.Equal("/Remainder:True", tcc.Remainder[0]);
            Assert.Equal("RemainderTwo", tcc.Remainder[1]);
        }

        [Fact]
        
        public void TFSUseCaseOptionalPrefix() {
            var tcc1 = new Kev_TFS_UseCase();
            var tcc2 = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();

            string[] expectedArgs = new string[] {
                "/BUILDNAME:TFSSupport_DevDesktopPack_Main_Certified_20090423.8",
                "/ATTACHMENT:\"c:\\temp\\list.txt\""
            };

            string[] expectedArgs2 = new string[] {
                "BUILDNAME:TFSSupport_DevDesktopPack_Main_Certified_20090423.8",
                "ATTACHMENT:\"c:\\temp\\list.txt\""
            };

            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";
            clas.ArgumentPrefixOptional = true;

            clas.ProcessArguments(tcc1, expectedArgs);
            clas.ProcessArguments(tcc2, expectedArgs2);

            Assert.Equal("TFSSupport_DevDesktopPack_Main_Certified_20090423.8", tcc1.BuildName);
            Assert.Equal("\"c:\\temp\\list.txt\"", tcc1.Attachment);

            Assert.Equal(tcc1.BuildName, tcc2.BuildName);
            Assert.Equal(tcc1.Attachment, tcc2.Attachment);
        }

        [Fact]
        
        public void TFSUseCaseUsesProperties() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();

            string[] expectedArgs = new string[] {
                "/BUILDNAME:TFSSupport_DevDesktopPack_Main_Certified_20090423.8",
                "/ATTACHMENT:\"c:\\temp\\list.txt\""
            };

            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";

            clas.ProcessArguments(tcc, expectedArgs);

            Assert.Equal("TFSSupport_DevDesktopPack_Main_Certified_20090423.8", tcc.BuildName);
            Assert.Equal("\"c:\\temp\\list.txt\"", tcc.Attachment);
            b.Info.Log(tcc.Attachment);
        }

        [Fact]
        
        public void TestingKevsUseCase2() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();
            string longHelp = clas.GenerateHelp(tcc, "Bugger");
            output.WriteLine(longHelp);
            Assert.True(longHelp.Contains("Bugger"), "The application name was not present in long help");
        }

        [Fact]
        
        public void TestingKevsUseCase3() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();

            string shortHelp = clas.GenerateShortHelp(tcc, "Bugger");
            output.WriteLine(shortHelp);
            Assert.True(shortHelp.Contains("Bugger"), "the application name was not present in long help");
            Assert.True(shortHelp.Contains("Pass the build"), "one of the descriptions did not make it into short help");
            Assert.True(shortHelp.Contains("Pass a filename"), "one of the descriptions did not make it into short help");
        }

        [Fact]
        [Trait(Traits.Style, Traits.Fresh)]
        public void TestingKevsUseCase4() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();
            for (int i = 1; i <= 5; i++) {
                clas.AddExample($"Example{i}", $"Details{i}");
            }
            string longHelp = clas.GenerateHelp(tcc, "Bugger");
            output.WriteLine(longHelp);
            for (int i = 1; i <= 5; i++) {
                Assert.True(longHelp.Contains($"Example{i}"), $"The #{i} Example was not in long help");
                Assert.True(longHelp.Contains($"Details{i}"), $"The #{i} Example detail was not in long help");
            }

        }

        [Fact]
        [Trait(Traits.Style, Traits.Fresh)]
        public void TestingKevsUseCase5() {
            var tcc = new Kev_TFS_UseCase();
            var clas = new CommandArgumentSupport();
            for (int i = 1; i <= 5; i++) {
                clas.AddExample($"Example{i}", $"Details{i}");
            }
            string shortHelp = clas.GenerateShortHelp(tcc, "Bugger");
            output.WriteLine(shortHelp);

            Assert.True(shortHelp.Contains($"Example{1}"), $"The #{1} Example was not in long help");
            Assert.True(shortHelp.Contains($"Details{1}"), $"The #{1} Example detail was not in long help");


        }

        [Fact]
        
        public void UseCase1_BasicFilenameParameter() {
            b.Info.Log("Starting Usecase1 Basic testing");

            #region string constants used to avoid types

            const string FILENAME1 = "Test1.Xslt";
            const string FILENAME2 = "Test2.xslt";
            const string FILENAME3 = "Test.xml";

            #endregion

            UC1_BasicFilenameParameters uc1Class = new UC1_BasicFilenameParameters();

            #region verify the initial state of the class

            Assert.True(uc1Class.TransformFilename1.Length == 0, "The initial state of filename 1 is wrong");
            Assert.True(uc1Class.TransformFilename2.Length == 0, "The initial state of filename 2 is wrong");
            Assert.True(uc1Class.OutputFilename.Length == 0, "The initial state of output filename is wrong");
            Assert.False(uc1Class.OverwriteOutput, "The initial state of overwrite is wrong");

            #endregion

            string[] expectedArguments = new string[] { "/X1:" + FILENAME1, "/x2:" + FILENAME2, "/o:" + FILENAME3, "/Y" };

            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";

            clas.ProcessArguments(uc1Class, expectedArguments);

            // Actual test case verification.
            Assert.Equal(uc1Class.TransformFilename1, FILENAME1);
            Assert.Equal(uc1Class.TransformFilename2, FILENAME2);
            Assert.Equal(uc1Class.OutputFilename, FILENAME3);
            Assert.True(uc1Class.OverwriteOutput, "The boolean overwrite flag did not get passed correctly");
        }

        [Fact]
        
        public void UseCase2_OptionalParameterDefaultFilenames() {
            b.Info.Log("Starting UseCase2 Testing");

            #region predefined string values used to avoid typos

            string[] fileNames = new string[] { "file1.xml", "file2arflebarfleglooopmakethestringmuchlongerifwecan.xml", "c:\\temp\\files\\file3.xml", "\\s816244\\c$\\afile\\file4.xml", "file://testingfile/file5.xml" };

            #endregion

            UC2_OptionPlusDefaultFilenames uc2Class = new UC2_OptionPlusDefaultFilenames();

            #region verify the initial state of the class

            b.Info.Log("About to verify the initial state of the UC2 Class");
            Assert.True(uc2Class.Filenames.Length == 0, "The initial state of the default array is wrong");
            Assert.True(uc2Class.Overwrite == false, "the initial state of the overwrite flag is false");

            #endregion

            string[] expectedArguments = new string[] { fileNames[0], fileNames[1], fileNames[2], fileNames[3], fileNames[4], "/O" };
            b.Info.Log("About to perform the actual test case");
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";

            clas.ProcessArguments(uc2Class, expectedArguments);

            b.Info.Log("Verifying the findings of the test case");

            Assert.True(uc2Class.Filenames.Length == 5, "The filenames were not passed through to the default array correctly");

            for (int i = 0; i < fileNames.Length; i++) {
                Assert.Equal(fileNames[i], uc2Class.Filenames[i]);
            }

            Assert.True(uc2Class.Overwrite, "The overwrite paramteter was not passed correctly");
        }

        [Fact]
        
        public void UseCase3_TFSBuildToolSampleArguments() {
            b.Info.Log("Starting UseCase3 - TBuildtool Sample UseCase");

            var argsupport = new CommandArgumentSupport();
            argsupport.ArgumentPrefix = "-";
            argsupport.ArgumentPostfix = "=";
            var parsedArgs = new TFSBuildToolArgs();

            string[] expectedArguments = new string[] { "-tfs=http://appsd1011:8080", "-teamproject=Acme", "-builddefinition=BuildTasks_CERTIFIED", "-agenturi=wks3090852" };
            argsupport.ProcessArguments(parsedArgs, expectedArguments);

            Assert.Equal("wks3090852", parsedArgs.agentUri);
            Assert.Equal("BuildTasks_CERTIFIED", parsedArgs.buildDefinition);
            Assert.Equal("http://appsd1011:8080", parsedArgs.tfs);
            Assert.Equal("Acme", parsedArgs.teamProject);

            expectedArguments[3] = "-agent=wks3090852";  // Test alternative parameter naming.
            parsedArgs = new TFSBuildToolArgs();

            argsupport.ProcessArguments(parsedArgs, expectedArguments);

            Assert.Equal("wks3090852", parsedArgs.agentUri);
            Assert.Equal("BuildTasks_CERTIFIED", parsedArgs.buildDefinition);
            Assert.Equal("http://appsd1011:8080", parsedArgs.tfs);
            Assert.Equal("Acme", parsedArgs.teamProject);
        }
    }
}
