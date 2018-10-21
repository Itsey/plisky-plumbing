namespace Plisky.Test {
    using System;
    using System.Net;
    using Xunit;
    using Plisky.Plumbing;
    using Plisky.Test.Mocks;
    using System.Net.Http;

    public class HttpHelperTests {

        [Fact(DisplayName = nameof(Constructor_SetsBaseUri))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void Constructor_SetsBaseUri() {
            const string val = "http://notarealurl/somwhere/not";

            var sut = new HttpHelper(val);
            Assert.Equal(val, sut.BaseUri);
        }

        [Fact(DisplayName = nameof(DefaultContentType_Json))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void DefaultContentType_Json() {
            var sut = new HttpHelper();
            Assert.Equal("application/json", sut.AcceptContentType);
        }

        [Fact(DisplayName = nameof(Constructor_MustBeValidIfPassed))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void Constructor_MustBeValidIfPassed() {
            Assert.Throws<ArgumentNullException>(() => {
                var sut = new HttpHelper(val: null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var sut = new HttpHelper(string.Empty);
            });
        }

        [Fact(DisplayName = nameof(MockHelper_SetResponse_ReturnsResponse))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public async void MockHelper_SetResponse_ReturnsResponse() {
            var hh = new MockHttpHelper("dummy");
            hh.SetResponse(HttpStatusCode.InternalServerError);

            var wcr = new WebCallRequest {  
                // Normally done in the Helper but this test exercises the mock directly.
                Verb = HttpMethod.Get
            };

            var r = await hh.ActualCall_Test(wcr);
            Assert.Equal(HttpStatusCode.InternalServerError, r.Status);
        }

        [Fact(DisplayName = nameof(Response_Non200_SetsSuccessFalse))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void Response_Non200_SetsSuccessFalse() {
            var sut = new WebCallResponse();

            foreach (var f in Enum.GetValues(typeof(HttpStatusCode))) {
                var nextCode = (HttpStatusCode)f;
                sut.Status = nextCode;

                if (nextCode == HttpStatusCode.OK) {
                    Assert.True(sut.Success);
                } else {
                    Assert.False(sut.Success);
                }
            }

            Assert.False(sut.Success);
        }


        [Theory(DisplayName = nameof(GetUri_ReturnsUri))]
        [Trait("age", "fresh")]
        [InlineData("https://test","test","test", "https://test/test/test")]
        [InlineData("https://www.google.co.uk/", "monkey", "fish", "https://www.google.co.uk/monkey/fish")]
        [InlineData("http://1", "/2", "3?4&5&6", "http://1/2/3?4&5&6")]
        [InlineData("http://www.test.com", "", null, "http://www.test.com/")]
        [InlineData("http://www.test.com", null, null, "http://www.test.com/")]
        [InlineData("http://www.test.com", null, "", "http://www.test.com/")]
        public void GetUri_ReturnsUri(string baseval, string stem, string qparam, string expected) {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;

            var result = sut.GetUri(baseval, stem, qparam);
            Assert.Equal(expected, result);
        }


        [Fact(DisplayName = nameof(GetUri_BaseCanNotBeNull))]
        [Trait("age", "fresh")]
        public void GetUri_BaseCanNotBeNull() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;

            Assert.Throws<ArgumentNullException>(() => {
                var result = sut.GetUri(null, "dummy", "dummy");
            });

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var result = sut.GetUri("", "dummy", "dummy");
            });
        }


        [Fact(DisplayName = nameof(Verb_ExecuteSet_Arrives))]
        [Trait("age", "fresh")]
        public void Verb_ExecuteSet_Arrives() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;

            _ = sut.Execute("p", HttpMethod.Get);

            Assert.Equal(hh.LastUsedVerb, "GET");
        }


        [Fact(DisplayName = nameof(Verb_DefaultIs_Get))]
        [Trait("age", "fresh")]
        public void Verb_DefaultIs_Get() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            _ = sut.Execute("p");

            Assert.Equal(hh.LastUsedVerb, "GET");
        }


        [Fact(DisplayName = nameof(Verb_PropSet_Persists))]
        [Trait("age", "fresh")]
        public void Verb_PropSet_Persists() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            sut.Verb = HttpMethod.Post;

            _ = sut.Execute("p");

            Assert.Equal(hh.LastUsedVerb, "POST");
        }


        [Fact(DisplayName = nameof(CustomVerb_Works))]
        [Trait("age", "fresh")]
        public void CustomVerb_Works() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;

            _ = sut.Execute("p", new HttpMethod("customMethodThatDoesNotExist"));
            Assert.Equal("customMethodThatDoesNotExist", hh.LastUsedVerb);
        }



        [Fact(DisplayName = nameof(Verb_ArivesUpperCase))]
        [Trait("age", "fresh")]
        public void Verb_ArivesUpperCase() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            
            _ = sut.Execute("p", HttpMethod.Post);
            Assert.Equal("POST", hh.LastUsedVerb );

            _ = sut.Execute("p", HttpMethod.Get);
            Assert.Equal("GET", hh.LastUsedVerb);

            sut.Verb = HttpMethod.Delete;
            _ = sut.Execute("p");
            Assert.Equal("DELETE", hh.LastUsedVerb);
        }


        [Fact(DisplayName = nameof(Auth_Basic_AddsB64Creds))]
        [Trait("age", "fresh")]
        public void Auth_Basic_AddsB64Creds() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            sut.AddBasicAuthHeader("secret");

            var s = hh.GetHeaderValue("Authorization");

            Assert.True(s.Contains("Basic"));
            Assert.True(!s.Contains("secret"));  // cant contain it in plain text.
        }

        [Fact(DisplayName = nameof(Auth_Bearer_AddsCreds))]
        [Trait("age", "fresh")]
        public void Auth_Bearer_AddsCreds() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            sut.AddBearerAuthHeader("secret");

            var s = hh.GetHeaderValue("Authorization");

            Assert.True(s.Contains("Bearer"));
            Assert.True(s.Contains("secret"));  // cant contain it in plain text.
        }

        [Fact(DisplayName = nameof(MockHelper_StartsZeroCallsMade))]
        [Trait("age", "fresh")]
        [Trait("type", "exploratory")]
        public void MockHelper_StartsZeroCallsMade() {
            var hh = new MockHttpHelper("dummy");
            Assert.Equal(0, hh.CallsMade);
        }

        [Fact(DisplayName = nameof(Retry_500_DoesRetry))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void Retry_500_DoesRetry() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;
            hh.SetResponse(HttpStatusCode.InternalServerError);

            sut.RetryCount = 5;
            sut.Execute(null);
            Assert.Equal(5, hh.CallsMade);
        }

        [Fact(DisplayName = nameof(Retry_404_DoesNotRetry))]
        [Trait("age", "current")]
        [Trait("type", "regression")]
        public void Retry_404_DoesNotRetry() {
            var hh = new MockHttpHelper("dummyuri");
            HttpHelper sut = hh;

            hh.SetResponse(HttpStatusCode.NotFound);
            sut.RetryCount = 5;
            sut.Execute(null);

            Assert.Equal(1, hh.CallsMade);
        }
    }
}