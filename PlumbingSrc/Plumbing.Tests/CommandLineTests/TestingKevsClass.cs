#if false
namespace Plisky.Test {

    using Plisky.Helpers;

    [CommandLineArgumentsAttribute()]
    public class TestingKevsClass {

        [CommandLineArgAttribute("BUILDNAME", Description = "Pass the build name e.g. TFSSupport_DevDesktopPack_Main_Certified_20090423.8")]
        public string BuildName { get; set; }

        [CommandLineArgAttribute("ATTACHMENT", Description = "Pass a filename, in 8.3 format or quoted")]
        public string Attachment { get; set; }

        [CommandLineArgDefault]
        public string[] Remainder { get; set; }
    }
}
#endif