namespace Plisky.Test {

    using System;
    using Plisky.Plumbing;

    [CommandLineArgumentsAttribute()]
    public class Kev_TFS_UseCase {

        [CommandLineArgAttribute("BUILDNAME", Description = "Pass the build name e.g. TFSSupport_DevDesktopPack_Main_Certified_20090423.8")]
        public string BuildName { get; set; }

        [CommandLineArgAttribute("ATTACHMENT", Description = "Pass a filename, in 8.3 format or quoted")]
        public string Attachment { get; set; }

        [CommandLineArgDefault]
        public string[] Remainder { get; set; }
    }

    // Special case where using someone elses class to test a known bug, but coding convention from other persons class
    // does not meet current preferred appraoch therefore disabled warnings to keep test case valid.
#pragma warning disable IDE1006

    [CommandLineArgumentsAttribute()]
    public class Sean_DateUseCase {

        [CommandLineArgAttribute("f", Description = "Pass a filename, in 8.3 format or quoted")]
        public DateTime from { get; set; }

        [CommandLineArgAttribute("t")]
        public DateTime to { get; set; }
    }

#pragma warning restore IDE1006
}