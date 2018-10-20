namespace Plisky.Test {
    using Plisky.Plumbing;
    using Plisky.Test.Mocks;
    using System;
    using System.Diagnostics;
    using System.Text;
    
    using Xunit;

    public class HttpHelperTests {


        [Fact(DisplayName = nameof(Constructor_SetsBaseUri))]
        [Trait("age", "fresh")]
        public void Constructor_SetsBaseUri() {
            const string val = "http://notarealurl/somwhere/not";

            var sut = new HttpHelper(val);
            Assert.Equal(val, sut.BaseUri);
        }


        [Fact(DisplayName = nameof(Constructor_MustBeValidIfPassed))]
        [Trait("age", "fresh")]
        public void Constructor_MustBeValidIfPassed() {
            Assert.Throws<ArgumentNullException>( () => {
                var sut = new HttpHelper(null);
            });

            Assert.Throws<ArgumentOutOfRangeException>( () => {
                var sut = new HttpHelper(string.Empty);
            });
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void Exploratory_CreateHelper() {
            var hh = new MockHttpHelper();
            hh.SetResponse(404);
            //Assert.Throws<HttpException>(() => {
            //    hh.ActualCall();
            //});
        }



        [Fact][Trait("xunit","regression")]
        public void Exploratory_CreateHelper() {
            var hh = new MockHttpHelper();
            HttpHelper sut = hh;

            hh.SetResponse(404);
            sut.RetryCount = 5;

            Assert.Equal(0, hh.CallsMade);
            Assert.Equal(5, hh.CallsMade);
        }
    }

}