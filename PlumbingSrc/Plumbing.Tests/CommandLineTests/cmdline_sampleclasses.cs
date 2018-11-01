namespace Plisky.Test {
    using Plisky.Helpers;
    using System;


    /// Sample command line class to use the command line attributes for unit testing.
    /// </summary>
    [CommandLineArgumentsAttribute()]
    public class SampleCommandLine_C1 {

        [CommandLineArg("N:", Description = "The First filename")]
        public string NameParameterOne;

        [CommandLineArg("A:", Description = "The second filename")]
        public string NameParameterTwo;

        [CommandLineArg("COUNT:", FullDescription = "This is the count of artifacts that are counted when you want to count something")]
        [CommandLineArg("C:", Description = "The count")]
        public int NumberParameterOne;

        [CommandLineArg("REPEATS:", FullDescription = "This is the number of repeats that are had")]
        public long NumberParameterTwo;

        [CommandLineArg("OP1")]
        public bool OptionParameterOne;

        [CommandLineArg("OP2")]
        public bool OptionParameterTwo;

        public SampleCommandLine_C1() {
            NameParameterOne = NameParameterTwo = String.Empty;
        }
    }

    [CommandLineArguments]
    public class SampleCommandLine_C2 {

        [CommandLineArg("Filename", Description = "The filename to be ~~MatchShortDescrFilename~~ passed into the application", IsSingleParameterDefault = true)]
        public string Filename;

        [CommandLineArg("INTVALUE")]
        public int NumberParam1 { get; set; }

        [CommandLineArg("LONGVALUE")]
        public long NumberParam2 { get; set; }
    }


    [CommandLineArguments]
    public class SampleCommandLine_C3 {
        

        [CommandLineArg("IntArray")]
        public int[] NumArray { get; set; }

        [CommandLineArg("StrArray")]
        public string[] StrArray { get; set; }
    }
}
