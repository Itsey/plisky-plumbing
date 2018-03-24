#if false
namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Helpers;
    using Xunit;


    public class CommandLineSupport_UseCases {
        private Bilge b = new Bilge();

        public CommandLineSupport_UseCases() {
            
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

        [Fact][Trait("xunit","regression")]
        public void TFSUseCaseRemainderCheck() {
            TestingKevsClass tcc = new TestingKevsClass();
            CommandArgumentSupport clas = new CommandArgumentSupport();

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

        [Fact][Trait("xunit","regression")]
        public void TFSUseCaseOptionalPrefix() {
            TestingKevsClass tcc1 = new TestingKevsClass();
            TestingKevsClass tcc2 = new TestingKevsClass();
            CommandArgumentSupport clas = new CommandArgumentSupport();

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

            Assert.Equal(tcc1.BuildName, tcc2.BuildName );
            Assert.Equal(tcc1.Attachment, tcc2.Attachment);
        }

        [Fact][Trait("xunit","regression")]
        public void TFSUseCaseUsesProperties() {
            TestingKevsClass tcc = new TestingKevsClass();
            CommandArgumentSupport clas = new CommandArgumentSupport();

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

        [Fact][Trait("xunit","regression")]
        public void TestingKevsUseCase2() {
            TestingKevsClass tcc = new TestingKevsClass();
            CommandArgumentSupport clas = new CommandArgumentSupport();
            string longHelp = clas.GenerateHelp(tcc, "Bugger");
            Assert.True(longHelp.Contains("Bugger"), "The application name was not present in long help");
        }

        [Fact][Trait("xunit","regression")]
        public void TestingKevsUseCase3() {
            TestingKevsClass tcc = new TestingKevsClass();
            CommandArgumentSupport clas = new CommandArgumentSupport();

            string shortHelp = clas.GenerateShortHelp(tcc, "Bugger");

            Assert.True(shortHelp.Contains("Bugger"), "the application name was not present in long help");
            Assert.True(shortHelp.Contains("Pass the build"), "one of the descriptions did not make it into short help");
            Assert.True(shortHelp.Contains("Pass a filename"), "one of the descriptions did not make it into short help");
        }

        [Fact][Trait("xunit","regression")]
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

        [Fact][Trait("xunit","regression")]
        public void UseCase2_OptionalParameterDefaultFilenames() {
            b.Info.Log("Starting UseCase2 Testing");

#region string constants used to avoid typos

            string[] FILENAMES = new string[] { "file1.xml", "file2arflebarfleglooopmakethestringmuchlongerifwecan.xml", "c:\\temp\\files\\file3.xml", "\\s816244\\c$\\afile\\file4.xml", "file://testingfile/file5.xml" };

#endregion

            UC2_OptionPlusDefaultFilenames uc2Class = new UC2_OptionPlusDefaultFilenames();

#region verify the initial state of the class

            b.Info.Log("About to verify the initial state of the UC2 Class");
            Assert.True(uc2Class.Filenames.Length == 0, "The initial state of the default array is wrong");
            Assert.True(uc2Class.Overwrite == false, "the initial state of the overwrite flag is false");

#endregion

            string[] expectedArguments = new string[] { FILENAMES[0], FILENAMES[1], FILENAMES[2], FILENAMES[3], FILENAMES[4], "/O" };
            b.Info.Log("About to perform the actual test case");
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ArgumentPrefix = "/";
            clas.ArgumentPostfix = ":";

            clas.ProcessArguments(uc2Class, expectedArguments);

            b.Info.Log("Verifying the findings of the test case");

            Assert.True(uc2Class.Filenames.Length == 5, "The filenames were not passed through to the default array correctly");

            for (int i = 0; i < FILENAMES.Length; i++) {
                Assert.Equal(FILENAMES[i], uc2Class.Filenames[i]);
            }

            Assert.True(uc2Class.Overwrite, "The overwrite paramteter was not passed correctly");
        }

        [Fact][Trait("xunit","regression")]
        public void UseCase3_TFSBuildToolSampleArguments() {
            b.Info.Log("Starting UseCase3 - TBuildtool Sample UseCase");

            CommandArgumentSupport argsupport = new CommandArgumentSupport();
            argsupport.ArgumentPrefix = "-";
            argsupport.ArgumentPostfix = "=";
            TFSBuildToolArgs parsedArgs = new TFSBuildToolArgs();

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
#endif