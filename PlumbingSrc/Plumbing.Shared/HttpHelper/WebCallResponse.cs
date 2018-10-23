namespace Plisky.Plumbing {
    using System;
    using System.Net;

    public class WebCallResponse {
        private HttpStatusCode hsc;
        private WebException ex;

        public bool Success { get; private set; }

        public HttpStatusCode Status {
            get {
                return hsc;
            }

            set {
                hsc = value;
                Success = (hsc == HttpStatusCode.OK);
            }

        }
        public string ResponseText { get; set; }
        public string ErrorText { get; set; }
        public Exception Error { get; set; }
        public WebException Exception {
            get {
                return ex;
            }
            set {
                ex = value;
                Success = (ex == null);
            }
        }
    }
}
