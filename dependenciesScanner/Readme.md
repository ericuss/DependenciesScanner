dotnet build
dotnet pack
dotnet tool uninstall -g dependenciesscanner
dotnet tool install  -g --add-source ./nupkg/ dependenciesscanner
/c/Users/etorre/.dotnet/tools/dotnet-depscanner.exe 
