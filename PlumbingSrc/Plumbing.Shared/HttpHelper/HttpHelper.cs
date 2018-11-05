﻿namespace Plisky.Plumbing {
    using Plisky.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpHelper {
        protected Bilge b;
        protected NameValueCollection headers = new NameValueCollection();
        public string BaseUri { get; set; }
        public string Stem { get; set; }

        /// <summary>
        /// This is the preferred way of setting the httpMethod that the call uses and will persist across execute requests, 
        /// however execute takes a secondary parameter that will allow you to use alternative verbs that are not present 
        /// in the enum or to override an individual call.
        /// </summary>
        public HttpMethod Verb { get; set; }

        public int RetryCount { get; set; }
        public string AcceptContentType { get; set; }

        protected virtual async Task<WebCallResponse> ActualCall(WebCallRequest wcr) {
            var result = new WebCallResponse();
            var request = (HttpWebRequest)WebRequest.Create(wcr.FullUri);
            request.Method = wcr.Verb.Method;

            request.Headers.Add(wcr.AllHeaders);

            try {
                b.Verbose.Log(wcr.DiagnosticSummary());


                if (!string.IsNullOrEmpty(wcr.Body)) {
                    var encoding = new UTF8Encoding();
                    var byteArray = encoding.GetBytes(wcr.Body);

                    request.ContentLength = byteArray.Length;
                    request.ContentType = @"application/json";

                    using (Stream dataStream = request.GetRequestStream()) {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                } else {
                    request.ContentLength = 0;
                }



                var req = request.GetResponseAsync();
                Stream responseStream = null;
                try {
                    try {
                        var response = await req.ConfigureAwait(false);

                        responseStream = response.GetResponseStream();

                        

                        using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8)) {
                            responseStream = null;
                            result.ResponseText = await reader.ReadToEndAsync().ConfigureAwait(false);
                            // TODO : Dont log what could be a massive string out  b.Log.Summarise();
                            b.Verbose.Log(result.ResponseText);
                        }

                        result.Status = HttpStatusCode.OK;
                        result.Exception = null;

                    } catch (WebException wx) {
                        result.Exception = wx;
                        if (wx.Status == WebExceptionStatus.ProtocolError) {
                            var r = wx.Response as HttpWebResponse;
                            if (r != null) {
                                result.Status = r.StatusCode;
                            }
                        }
                    } finally {
                        responseStream?.Dispose();
                    }
                } finally {
                    req?.Dispose();
                }

            } catch (Exception ex) {
                b.Error.Dump(ex, $"httpActualCAll Failed {wcr.FullUri}");
                throw;
            }

            return result;
        }

        public HttpHelper(string val) : this() {
            if (val == null) { throw new ArgumentNullException(nameof(val)); }
            if (string.IsNullOrWhiteSpace(val)) { throw new ArgumentOutOfRangeException(nameof(val)); }
            BaseUri = val;
        }

        public HttpHelper(Bilge useThisBilge = null) {
            AcceptContentType = "application/json";
            Verb = HttpMethod.Get;
            b = useThisBilge ?? new Bilge(tl: TraceLevel.Off);
        }


        public async Task<WebCallResponse> Execute(string qString, string exBody=null, HttpMethod exVerb = null) {
            int retries = 0;

            var wcr = new WebCallRequest {
                FullUri = GetUri(BaseUri, Stem, qString),
                Verb = exVerb ?? Verb,
                Body = exBody,
                AllHeaders = new NameValueCollection(headers)
            };

            var f = await ActualCall(wcr);
            while ((f.Status != HttpStatusCode.OK) && (f.Status != HttpStatusCode.NotFound)) {
                retries++;
                if (retries >= RetryCount) {
                    break;
                }
                f = await ActualCall(wcr);
            }

            return f;
        }

        public string GetUri(string actualBase, string actualStem, string actualParam) {
            if (actualBase == null) { throw new ArgumentNullException(nameof(actualBase)); }
            if (string.IsNullOrWhiteSpace(actualBase)) { throw new ArgumentOutOfRangeException(nameof(actualBase)); }

            if (!actualBase.EndsWith("/")) {
                actualBase += "/";
            }

            if (!string.IsNullOrWhiteSpace(actualStem)) {
                if (actualStem.StartsWith("/")) {
                    actualStem = actualStem.Substring(1);
                }
            } else {
                actualStem = "";
            }

            if (!string.IsNullOrWhiteSpace(actualParam)) {
                if (!actualStem.EndsWith("/")) {
                    actualStem += "/";
                }
            }
            return $"{actualBase}{actualStem}{actualParam}";
        }

        private const string AUTH_HEADER_NAME = "Authorization";
        private const string AUTH_BEARER_VALUE = "Bearer";
        private const string AUTH_BASIC_VALUE = "Basic";

        public void AddBearerAuthHeader(string secret) {

             
            headers.Add(AUTH_HEADER_NAME, $"{AUTH_BEARER_VALUE} {secret}");
        }

        public void AddBasicAuthHeader(string secret) {
            secret = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(secret));
            headers.Add(AUTH_HEADER_NAME, $"{AUTH_BASIC_VALUE} {secret}");
        }

        public void AddHeader(string v1, string v2) {
            headers.Add(v1, v2);
        }


        /*
         

   



 

//body = strversionofq;

try {
	var url = uri+qstem+qparam;
    Console.WriteLine(url);
	

        request.Method = method.ToUpper();
	request.Timeout = 200000;

	if (headers.Count > 0) {
		foreach (var h in headers) {
			request.Headers.Add(h.Item1, h.Item2);
		}

	}

	if (!string.IsNullOrEmpty(body)) {
		var encoding = new UTF8Encoding();
		var byteArray = encoding.GetBytes(body);

		request.ContentLength = byteArray.Length;
		request.ContentType = @"application/json";

		using (Stream dataStream = request.GetRequestStream()) {
			dataStream.Write(byteArray, 0, byteArray.Length);
		}
	} else {
		request.ContentLength = 0;
	}


	
    */
    }
}