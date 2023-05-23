namespace Plisky.Test {

    using System.Collections.Generic;
    using Plisky.Plumbing;

    public class MockConfigHub : ConfigHub {
        private Dictionary<string, string> envVars = new Dictionary<string, string>();

        public MockConfigHub() {
            Mock = new Mocking(this);
        }

        public string EntryPath { get; set; }

        #region mocking implementation

        public Mocking Mock;

        public class Mocking {
            private MockConfigHub parent;

            public Mocking(MockConfigHub p) {
                parent = p;
            }

            public string GetDirectoryName(string inDir) {
                return parent.GetDirectoryName(inDir);
            }

            public void Mock_MockingBird() {
            }

            internal void AddEnvironmentVariable(string v1, string v2) {
                parent.envVars.Add(v1, v2);
            }
        }

        #endregion mocking implementation

        protected override string ActualExpandForEnvironmentVariables(string result) {
            foreach (string l in envVars.Keys) {
                string rep = "%" + l + "%";
                result = result.Replace(rep, envVars[l]);
            }
            return result;
        }

        protected override string ActualGetEntryPointPath() {
            if (EntryPath != null) {
                return EntryPath;
            }
            return base.ActualGetEntryPointPath();
        }
    }
}