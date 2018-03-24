@echo off
::if "%1"=="" goto :error
echo Run this with a version Last used Version 2

echo creating Diagnostics Library and sending to Nuget
nuget pack PliskyPlumbing.nuspec  -properties configuration=Debug


::echo disabled push
::goto eof

::nuget push Plisky.Listeners.2.0.7.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Plisky.Plumbing.1.0.0.nupkg -apiKey oy2guijbuix4bqf6i6mayp3vipctybpunopwgq3icvtlqy -Source https://www.nuget.org/api/v2/package

goto eof

:error 
echo put version as single digit parameter
:eof


:error 
echo put version as single digit parameter
:eof
