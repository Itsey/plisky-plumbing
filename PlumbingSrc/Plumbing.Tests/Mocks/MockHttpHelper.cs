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

            var result = new WebCallResponse {
                Status = responseCode
            };
            return await Task.FromResult<WebCallResponse>(result);
        }

        internal string GetHeaderValue(string headerName) {
            foreach(var f in headers) {
                if (f.Item1 == headerName) {
                    return f.Item2;
                }
            }
            throw new InvalidOperationException("Name not found");
        }
    }
}