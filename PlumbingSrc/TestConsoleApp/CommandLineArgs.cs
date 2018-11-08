using Plisky.Helpers;

namespace Plisky.Test {

    [CommandLineArgumentsAttribute]
    internal class CommandLineArgs {

        [CommandLineArg("x",Description ="This is a short description", FullDescription ="this is the full description")]
        public int IntegerParam { get; set; }
        public CommandLineArgs() {
        }
    }
}