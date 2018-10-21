namespace Plisky.Plumbing {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;

    [DebuggerDisplay("{DiagnosticsSummary}")]
    public class WebCallRequest {

        public bool Success { get; set; }

        public HttpMethod Verb { get; set; }

        public string Body { get; set; }
        public string FullUri { get; internal set; }

        internal string DiagnosticSummary() {
            var bl = Body!=null ? Body.Length.ToString():"0";
            return $"[{Success}] WR: {FullUri} {Verb.Method}  BL:{bl}";
        }
    }
}
