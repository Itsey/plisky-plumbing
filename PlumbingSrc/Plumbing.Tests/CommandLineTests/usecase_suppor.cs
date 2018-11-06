
namespace Plisky.Test {

    using Plisky.Helpers;
    using System;

    [CommandLineArgumentsAttribute()]
    public class Kev_TFS_UseCase {

        [CommandLineArgAttribute("BUILDNAME", Description = "Pass the build name e.g. TFSSupport_DevDesktopPack_Main_Certified_20090423.8")]
        public string BuildName { get; set; }

        [CommandLineArgAttribute("ATTACHMENT", Description = "Pass a filename, in 8.3 format or quoted")]
        public string Attachment { get; set; }

        [CommandLineArgDefault]
        public string[] Remainder { get; set; }
    }

    [CommandLineArgumentsAttribute()]
    public class Sean_DateUseCase {

        [CommandLineArgAttribute("f", Description = "Pass a filename, in 8.3 format or quoted")]
        public DateTime from { get; set; }

        [CommandLineArgAttribute("t")]
        public DateTime to { get; set; }
    }

}
