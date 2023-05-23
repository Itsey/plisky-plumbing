namespace Plisky.Test {

    using System;
    using System.IO;
    using Plisky.Diagnostics;
    using TestData;
    using Xunit;

    public class TestDataTests {
        private Bilge b;
        private UnitTestHelper uth;

        public TestDataTests() {
            b = new Bilge();
            uth = new UnitTestHelper();
        }

        [Fact(DisplayName = nameof(Blows_IfResourceName_Invalid))]
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
        public void File_GetTestData_HasRightContent() {
            const string TEXT_FROM_FILE = "arfle barfle gloop";
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {
                string ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

                string fname = sut.GetTestDataFile(ident);

                string str = File.ReadAllText(fname);

                Assert.Equal(TEXT_FROM_FILE, str);
            } finally {
                sut.ClearUpTestFiles();
            }
        }

        [Fact(DisplayName = nameof(File_GetTestData_Works))]
        public void File_GetTestData_Works() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            try {
                string ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

                string fname = sut.GetTestDataFile(ident);

                Assert.NotNull(fname);
                Assert.True(File.Exists(fname));
            } finally {
                sut.ClearUpTestFiles();
            }
        }

        [Fact(DisplayName = nameof(FileResource_InvalidResourceNameBlows))]
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

        [Fact(DisplayName = nameof(GetTestDatFile_BlowsOnNull))]
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

        [Fact(DisplayName = nameof(TestData_GenericString_Valid))]
        public void TestData_GenericString_Valid() {
            string res1 = SampleTestData.GENERIC_STRING1;
            string res2 = SampleTestData.GENERNIC_STRING2;
            string res3 = SampleTestData.GENERIC_STRING3;

            Assert.False(string.IsNullOrWhiteSpace(res1));
            Assert.False(string.IsNullOrWhiteSpace(res2));
            Assert.False(string.IsNullOrWhiteSpace(res3));
        }

        [Fact(DisplayName = nameof(TestData_URL_GetsCorrectNumber))]
        public void TestData_URL_GetsCorrectNumber() {
            b.Info.Flow();

            SampleTestData td = new SampleTestData();
            var urls = td.GetTestURLs(10);
            int count = 0;
            foreach (string u in urls) {
                count++;
                Assert.False(string.IsNullOrWhiteSpace(u));
                var f = u.ToLowerInvariant();
                Assert.StartsWith("http", f);
            }

            Assert.Equal(10, count);
        }

        [Fact(DisplayName = nameof(TestDataFile_CleanUpWorks))]
        public void TestDataFile_CleanUpWorks() {
            b.Info.Flow();

            var sut = new UnitTestHelper();
            string ident = TestResources.GetIdentifiers(TestResourcesReferences.SingleTextFile);

            string fname = sut.GetTestDataFile(ident);
            bool fileExistsBeforeCleanup = File.Exists(fname);

            sut.ClearUpTestFiles();

            Assert.True(fileExistsBeforeCleanup);
            Assert.False(File.Exists(fname));
        }
    }
}