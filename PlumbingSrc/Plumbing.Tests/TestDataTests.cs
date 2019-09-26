using System.IO;
using Plisky.Diagnostics.Listeners;
using TestData;

namespace Plisky.Test {
    using System;
    using System.Net;
    using Xunit;
    using Plisky.Plumbing;
    using Plisky.Diagnostics;

    public class TestDataTests {
        private Bilge b;
        private UnitTestHelper uth;

        public TestDataTests() {
            b = new Bilge(tl: System.Diagnostics.TraceLevel.Verbose);
            uth = new UnitTestHelper(b);
            uth.AddHandlerOnce(new TCPHandler("127.0.0.1", 5060, true));
        }

        [Fact(DisplayName = nameof(TestDataFile_CleanUpWorks))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TestDataFile_CleanUpWorks() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            var ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

            string fname = sut.GetTestDataFile(ident);
            bool fileExistsBeforeCleanup = File.Exists(fname);

            sut.ClearUpTestFiles();

            Assert.True(fileExistsBeforeCleanup);
            Assert.False(File.Exists(fname));

        }

        [Fact(DisplayName = nameof(File_GetTestData_Works))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void File_GetTestData_Works() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {
                var ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

                string fname = sut.GetTestDataFile(ident);

                Assert.NotNull(fname);
                Assert.True(File.Exists(fname));
            } finally {
                sut.ClearUpTestFiles();
            }

        }

        [Fact(DisplayName = nameof(Blows_IfResourceName_Invalid))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Blows_IfResourceName_Invalid() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {

                Assert.Throws<InvalidOperationException>(() => {
                    _ = sut.GetTestDataFile("monkey-butt");
                });

            } finally {
                sut.ClearUpTestFiles();
            }

        }

        [Fact(DisplayName = nameof(File_GetTestData_HasRightContent))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void File_GetTestData_HasRightContent() {
            const string TEXT_FROM_FILE = "arfle barfle gloop";
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {
                var ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

                string fname = sut.GetTestDataFile(ident);

                string str = File.ReadAllText(fname);

                Assert.Equal(TEXT_FROM_FILE, str);

            } finally {
                sut.ClearUpTestFiles();
            }

        }


        [Fact(DisplayName = nameof(GetTestDatFile_BlowsOnNull))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GetTestDatFile_BlowsOnNull() {
            b.Info.Flow();

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var sut = new UnitTestHelper();
                sut.GetTestDataFile(null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var sut = new UnitTestHelper();
                sut.GetTestDataFile("");
            });
        }


        [Fact(DisplayName = nameof(FileResource_InvalidResourceNameBlows))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void FileResource_InvalidResourceNameBlows() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {
                var ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

                string fname = sut.GetTestDataFile(ident);

                Assert.NotNull(fname);
                Assert.True(File.Exists(fname));
            } finally {
                sut.ClearUpTestFiles();
            }

        }


        [Fact(DisplayName = nameof(TestData_GenericString_Valid))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TestData_GenericString_Valid() {
            var res1 = SampleTestData.GenericString1;
            var res2 = SampleTestData.GenericString2;
            var res3 = SampleTestData.GenericString3;

            Assert.False(string.IsNullOrWhiteSpace(res1));
            Assert.False(string.IsNullOrWhiteSpace(res2));
            Assert.False(string.IsNullOrWhiteSpace(res3));
        }


        [Fact(DisplayName = nameof(TestData_URL_GetsCorrectNumber))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TestData_URL_GetsCorrectNumber() {
            b.Info.Flow();

            SampleTestData td = new SampleTestData();
            var urls = td.GetTestURLs(10);
            int count = 0;
            foreach (var u in urls) {
                count++;
                Assert.False(string.IsNullOrWhiteSpace(u));
                var f = u.ToLowerInvariant();
                Assert.StartsWith("http", f);
            }

            Assert.Equal(10, count);
        }

    }
}