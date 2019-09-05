namespace dependenciesScanner.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class CsProjFileService:FileServiceCore
    {
        private const string patternOfCsProjs = "*.csproj";
        private const string keyToSearchPackages = "<PackageReference ";
        private readonly Func<string, bool> CsProjFilter = line => line.Contains(keyToSearchPackages)
                                                                        && line.ToLower().Contains("version")
                                                                        && !line.Contains("$(");
        public IEnumerable<string> GetAllCsProjs(string sourcePath)
        {
            var csprojs = SearchAndCreateIfNotExist(sourcePath, patternOfCsProjs, null);
            return csprojs.ToList();
        }

        public void ScanPackagesFromCsProj(string filePath, Dictionary<string, List<string>> packages)
        {
            var lines = File.ReadLines(filePath);
            ScanPackagesFromFile(filePath, packages, CsProjFilter, null);

            var linesModified = lines
                                .ToList()
                                .Select(line =>
                                {
                                    if (line.Contains(keyToSearchPackages) && line.ToLower().Contains("version"))
                                    {
                                        var parameters = GetParametersFromLine(line);
                                        var lineReplaced = line.Replace(parameters[1], "\"$(" + parameters[0].Replace(".", "_").Replace("\"", "") + "_Package_Version)\"");
                                        return lineReplaced;
                                    }
                                    else
                                    {
                                        return line;
                                    }
                                });

            File.WriteAllLines(filePath, linesModified);
        }
    }
}
