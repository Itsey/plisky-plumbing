using System;

namespace TestData {



    public enum TestResourcesReferences {
        SingleTextFile,
        XMLUseCaseFile,
        ConfigHubTestData
    }

    public static class TestResources {

        public static string GetIdentifiers(TestResourcesReferences refNo) {

            switch (refNo) {
                case TestResourcesReferences.SingleTextFile: return "SampleFileData.txt";
                case TestResourcesReferences.XMLUseCaseFile: return "xmlusecase.xml";
                case TestResourcesReferences.ConfigHubTestData: return "chubtestdata.xml";
            }
            return null;
        }




    }
}
