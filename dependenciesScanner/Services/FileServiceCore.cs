namespace dependenciesScanner.Services
{
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class FileServiceCore
    {
        protected IEnumerable<string> SearchAndCreateIfNotExist(string sourcePath, string fileName, Func<string, string, string> createFile)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(sourcePath, fileName, SearchOption.AllDirectories);

            if (files.Any())
            {
                files.ToList().ForEach(x => Log.Information($"        {fileName} found: {x}"));
                result = files.ToList();
            }
            else if (createFile != null)
            {
                Log.Information($"        Creating {fileName}" );
                result.Add(createFile(sourcePath, fileName));
            }

            return result;
        }

        protected virtual List<string> GetParametersFromLine(string line)
        {
            string pattern = "\"[\\s\\S]*?\"";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            var parameters = r.Matches(line)
                                    .Cast<Match>()
                                    .Select(m => m.Value)
                                    .ToList();
            return parameters;
        }

        protected void AppendToPackages(string line, Dictionary<string, List<string>> packages)
        {
            var package = new Models.PackageScannModel(this.GetParametersFromLine(line));

            Log.Information($"        Package founded => {package?.Name} {package?.Version}");
            if (!packages.ContainsKey(package.Name)) packages.Add(package.Name, new List<string> { package.Version });
            else if (!packages[package.Name].Contains(package.Version)) packages[package.Name].Add(package.Version);
        }

        protected IEnumerable<string> ScanPackagesFromFile(string filePath,
                                                Dictionary<string, List<string>> packages,
                                                Func<string, bool> filterInFile,
                                                Func<string, string> mapLines)
        {
            var lines = File.ReadLines(filePath)
                            .Where(filterInFile)
                            .Select(line => mapLines != null ? mapLines(line) : line)
                            .ToList()
                            ;

            lines.ForEach(line => AppendToPackages(line, packages));

            return lines;
        }


    }
}
