using System;
using System.Collections.Generic;
using Plisky.Plumbing;

namespace Plisky.Test.Mocks {
    internal class MockHttpHelper : HttpHelper {
        public int CallsMade { get; internal set; }

        internal void SetResponse(int v) {
            throw new NotImplementedException();
        }
    }
}