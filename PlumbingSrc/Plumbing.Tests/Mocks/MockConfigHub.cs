using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plisky.Plumbing;

namespace Plisky.Test {
    

    public class MockConfigHub  : ConfigHub{
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
        }
        #endregion

        public MockConfigHub() {
            
            Mock = new Mocking(this);

        }

    }

}
