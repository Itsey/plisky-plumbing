namespace Plisky.Test {
    using System;
    using System.Net;
    using Xunit;
    using Plisky.Plumbing;
    using Plisky.Diagnostics;

    public class TestDataTests {
        private Bilge b = new Bilge();

        [Fact(DisplayName = nameof(TestData_GenericString_Valid))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TestData_GenericString_Valid() {
            var res1 = TestData.GenericString1;
            var res2 = TestData.GenericString2;
            var res3 = TestData.GenericString3;
            
            Assert.False(string.IsNullOrWhiteSpace(res1));
            Assert.False(string.IsNullOrWhiteSpace(res2));
            Assert.False(string.IsNullOrWhiteSpace(res3));
        }


        [Fact(DisplayName = nameof(TestData_URL_GetsCorrectNumber))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TestData_URL_GetsCorrectNumber() {
            b.Info.Flow();

            TestData td = new TestData();
            var urls = td.GetTestURLs(10);
            int count = 0;
            foreach(var u in urls) {
                count++;
                Assert.False(string.IsNullOrWhiteSpace(u));
                var f = u.ToLowerInvariant();
                Assert.StartsWith("http", f);
            }

            Assert.Equal(10, count);
        }

    }
}