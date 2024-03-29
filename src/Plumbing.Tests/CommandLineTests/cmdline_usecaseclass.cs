﻿namespace Plisky.Test {

    using Plisky.Plumbing;

    [CommandLineArgumentsAttribute()]
    public class UC1_BasicFilenameParameters {

        [CommandLineArg("O", Description = "The name of the output file to generate")]
        [CommandLineArg("Output")]
        public string OutputFilename;

        [CommandLineArg("Y", Description = "If Y is passed then the output file will be overwritten")]
        public bool OverwriteOutput;

        [CommandLineArg("X1", Description = "The name of an XSLT file to use in the transform.")]
        public string TransformFilename1;

        [CommandLineArg("X2", Description = "The name of a second XSLT file to use in the transform.")]
        public string TransformFilename2;

        public UC1_BasicFilenameParameters() {
            TransformFilename1 = TransformFilename2 = OutputFilename = string.Empty;
        }
    }

    [CommandLineArgumentsAttribute()]
    public class UC2_OptionPlusDefaultFilenames {

        [CommandLineArgDefault]
        public string[] Filenames;

        [CommandLineArg("O")]
        public bool Overwrite;

        public UC2_OptionPlusDefaultFilenames() {
            Filenames = new string[0];
        }
    }
}