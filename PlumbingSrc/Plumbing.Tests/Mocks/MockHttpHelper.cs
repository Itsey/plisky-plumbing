using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Plisky.Test.Mocks {
    internal class MockHttpHelper : HttpHelper {
        private HttpStatusCode responseCode;
        private string responseBody;
        public int CallsMade { get; internal set; }
        public string LastUsedVerb { get; internal set; }
        public string LastUsedBody { get; private set; }

        public MockHttpHelper(string val) : base(val) {
            responseCode = HttpStatusCode.OK;
            responseBody = "<html>Hi</html>";
        }


        /// <summary>
        /// Sets the response that will come back from Actual Call.
        /// </summary>
        /// <param name="newResponseCode"></param>
        /// <param name="newResponseBody"></param>
        internal void SetResponse(HttpStatusCode newResponseCode, string newResponseBody = null) {
            responseCode = newResponseCode;
            responseBody = newResponseBody;
        }


        protected override async Task<WebCallResponse> ActualCall(WebCallRequest wcr) {
            return await this.ActualCall_Test(wcr);
        }

        public async Task<WebCallResponse> ActualCall_Test(WebCallRequest wcr) {
            CallsMade++;
            LastUsedVerb = wcr.Verb.Method;
            LastUsedBody = wcr.Body;

            var result = new WebCallResponse {
                Status = responseCode
            };
            return await Task.FromResult<WebCallResponse>(result);
        }

        internal string GetHeaderValue(string headerName) {

            return headers[headerName];
            

        }

        internal string GetBodyValue() {
            return LastUsedBody;
        }
    }
}