using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
#if DEBUG

    public partial class ConfigHub {
        public string Test_GetDirectoryName(string directory) {
            return GetDirectoryName(directory);
        }
    }

#endif
}
