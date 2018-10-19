namespace Plisky.Plumbing {
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class HttpHelper {
        public string BaseUri { get; set; }
        

        public HttpHelper(string val) {
            if (val==null) { throw new ArgumentNullException(nameof(val)); }
            if (string.IsNullOrWhiteSpace(val)) { throw new ArgumentOutOfRangeException(nameof(val)); }
            BaseUri = val;
        }

        public HttpHelper() {

        }
        

        /*
         

        var uri = @"https://www.hackerrank.com/x/api/v1/";
var qstem = "questions/";
string method = "GET";
string secret = "9674c8fdc7a43b8ef9662879c20063d89a9fc507484e23afc0ac90359c027e8e";
string body = null;
string qparam = null;
List<Tuple<string, string>> headers = new List<System.Tuple<string, string>>();
//

int whatToDo = 3;

if (whatToDo == 1) {
    // add question
    method = "GET";
    body = null;
    qparam = "232646";
} else if (whatToDo == 2) {
    method = "POST";
    body = null;
    qparam = "232646";
    //string strversionofq = File.ReadAllText(@"C:\temp\q.json");
   //var f = JObject.Parse(strversionofq);

} else if (whatToDo== 3) {
    method="GET";
    qstem = "tests/291241/questions/232646/testcases/";
    qparam = "0";
    body = "";
}

string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(secret));

headers.Add(new Tuple<string, string>("Authorization", "Basic " + svcCredentials));

 

//body = strversionofq;

try {
	var url = uri+qstem+qparam;
    Console.WriteLine(url);
	var request = (HttpWebRequest)WebRequest.Create(url);
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


	var req = request.GetResponseAsync();
	try {

		var response = await req.ConfigureAwait(false);

		var responseStream = response.GetResponseStream();
		try {

			using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8)) {


				responseStream = null;
				var s = await reader.ReadToEndAsync().ConfigureAwait(false);
				Console.WriteLine(s);
				return;
			}

		} catch (WebException wx) {
			if (wx.Status == WebExceptionStatus.ProtocolError) {
				var r = wx.Response as HttpWebResponse;
				if (r != null) {

					string log = $"{r.StatusCode} L[{url.Length}]@ {url}";

					if (r.StatusCode == HttpStatusCode.NotFound) {
						Console.WriteLine("404");
					} else {
						Console.WriteLine("Fail");
					}
				}
			}
			throw;
		} finally {
			if (responseStream != null) {
				responseStream.Dispose();
			}
		}
	} finally {
		if (req != null) {
			req.Dispose();
		}
	}

} catch (Exception ex) {
   Console.WriteLine(ex.Message);
	throw;
}

    */
    }
}
