Plisky.Plumbing - ConfigHub.
===========================================

ConfigHub uses the Hub context from Plisky Plumbing to provide application configuration anywhere wtihin the codebase irrespective of how the 
applicaiton configuration is to work for a given solution. It can therefore be used with app.config / web.config or any other form of configuration
to support the application configuration needs.

ConfigHub also has a built in DateTime function to get the current date, allowing for mocking and abstraction of the system DateTime.



Quick Start.
===============

To start simple retrieve existing settings from App.Config in the same way that you would do by default.

' // Use your own or the static Confighub.Current
' ConfigHub sut = new ConfigHub();
'
' // Use AppConfig <appSettings><add key="SettingName" value="value">
' sut.AddDefaultAppConfigFallback();
'
' string val = sut.GetSetting("SettingName");



Concepts.
=============

Config hub provides settings to your application code, using a series of fallback providers to find the settings from the environment that
the application is running on.  The Simplest is to add an app config fallback provider and config hub retrieves settings from application
configuration.  However often this is suboptimal, espeically where settings change through environments or where settings are sensitive.



TroubleShooting.
==================

To enable trace for ConfigHub turn on the trace switch "Plisky-ConfigHub".
This can be done using the standard configuration for Plisky.Diagnostics (see here)