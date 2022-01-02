namespace Plisky.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Plisky.Plumbing;

    public class MockConfigHub  : ConfigHub{
        private Dictionary<string, string> envVars = new Dictionary<string, string>();

        public string EntryPath { get; set; }

        #region mocking implementation
        public Mocking Mock;
        public class Mocking {
            private MockConfigHub parent;

            public Mocking(MockConfigHub p) {
                parent = p;
            }

            public void Mock_MockingBird() {

            }


            public string GetDirectoryName(string inDir) {
                return parent.GetDirectoryName(inDir);
            }

            internal void AddEnvironmentVariable(string v1, string v2) {
                parent.envVars.Add(v1, v2);

            }
        }
        #endregion

        protected override string ActualGetEntryPointPath() {
            if (EntryPath!=null) {
                return EntryPath;
            }
            return base.ActualGetEntryPointPath();
        }

        protected override string ActualExpandForEnvironmentVariables(string result) {
            foreach(string l in envVars.Keys) {
                string rep = "%" + l + "%";
                result = result.Replace(rep, envVars[l]);
            }
            return result;
        }
        public MockConfigHub() {
            
            Mock = new Mocking(this);

        }

    }

}
