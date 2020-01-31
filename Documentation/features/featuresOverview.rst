Using Plisky Feature Support
=============================================

Features are switches that are designed to allow you to work with conditional code to enable and disable elements of your code throug configuration.  




Enabling Features
====================

Features are provided through a FeatureProvider which implements IResolveFeatures.  The simplest Feature Provider is a hard coded feature provider where 
all of the features to be resolved are hard coded at app startup.

' var f = new FeatureHardCodedProvider();
' f.AddFeature(new Feature("TEST", true));
'
' if (Feature.GetFeatureByName("TEST").Active) {
'    // This feature is active thanks to the hard coded feature resolver
' }





