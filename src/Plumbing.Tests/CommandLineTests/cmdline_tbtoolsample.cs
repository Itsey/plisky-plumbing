namespace Plisky.Test {    
    using Plisky.Plumbing;

    [CommandLineArguments]
    public class TFSBuildToolArgs {

        [CommandLineArg("tfs")]
        public string tfs;

        [CommandLineArg("teamProject")]
        public string teamProject;

        [CommandLineArg("buildDefinition")]
        public string buildDefinition;

        [CommandLineArg("agent")]
        [CommandLineArg("agenturi")]
        public string agentUri;
    }
}
