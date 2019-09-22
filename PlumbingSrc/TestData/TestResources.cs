using System;

namespace TestData {



    public enum TestResourcesReferences {
        SingleTextFile
    }

    public static class TestResources {

        public static string GetIdentifiers(TestResourcesReferences refNo) {

            switch (refNo) {
                case TestResourcesReferences.SingleTextFile: return "SampleFileData.txt";
            }
            return null;
        }
    }
}
