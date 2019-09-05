To run need to install from nuget:

dotnet tool install  -g dependenciesscanner

After you can run like:
dotnet-depscanner --p={ROOT_PATH}


// old
dotnet build
dotnet pack
dotnet tool uninstall -g dependenciesscanner
dotnet tool install  -g --add-source ./nupkg/ dependenciesscanner
/c/Users/etorre/.dotnet/tools/dotnet-depscanner.exe 
