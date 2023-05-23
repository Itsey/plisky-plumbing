namespace Plisky.Test {

    using Plisky.Plumbing;

    [CommandLineArguments]
    public class TFSBuildToolArgs {

        [CommandLineArg("agent")]
        [CommandLineArg("agenturi")]
        public string agentUri;

        [CommandLineArg("buildDefinition")]
        public string buildDefinition;

        [CommandLineArg("teamProject")]
        public string teamProject;

        [CommandLineArg("tfs")]
        public string tfs;
    }
}