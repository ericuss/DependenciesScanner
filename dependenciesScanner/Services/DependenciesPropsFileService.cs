﻿namespace dependenciesScanner.Services
{
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DependenciesPropsFileService : FileServiceCore
    {
        private const string dependenciesFileName = "dependencies.props";
        private const string nestedDirectoryOfdependenciesFile = "_build";
        private Func<string, bool> DependenciesFilter = line => line.Contains("_Package_Version");
        private Func<string, string> MapLineDependency = line => line
                                                                    .Replace("_Package_Version", string.Empty)
                                                                    .Replace("_", ".");
        public IEnumerable<string> ScanPackagesFromDependency(string filePath, Dictionary<string, List<string>> packages)
        {

            return ScanPackagesFromFile(filePath, packages, DependenciesFilter, MapLineDependency);
        }
        public string SearchDependencyPropsAndCreateIfNotExist(string sourcePath)
        {
            var directoryOfFile = $"./{nestedDirectoryOfdependenciesFile}";
            if (!Directory.Exists(directoryOfFile))
            {
                Directory.CreateDirectory(directoryOfFile);
            }

            return SearchAndCreateIfNotExist(sourcePath, dependenciesFileName, this.CreateDependencyFile)
                    .FirstOrDefault();
        }

        public string CreateDependencyFile(string sourcePath, string fileName)
        {
            var content = ResourcesServices.GetDependenciesPropsTemplate();
            if (!Directory.Exists($"{sourcePath}/{nestedDirectoryOfdependenciesFile}/"))
            {
                Directory.CreateDirectory($"{sourcePath}/{nestedDirectoryOfdependenciesFile}/");
            }
            var filePath = $"{sourcePath}/{nestedDirectoryOfdependenciesFile}/{fileName}";
            File.WriteAllText(filePath, content);
            return filePath;
        }
        public void WriteToFile(string dependenciesFilePath, Dictionary<string, List<string>> packages)
        {
            var lines = packages
                        .Select(package =>
                        {
                            var name = package.Key.Replace(".", "_") + "_Package_Version";
                            var version = package.Value.OrderByDescending(x => x).First();
                            return $"    <{name}>{version}</{name}>";
                        })
                        .ToList();

            var linesOfDependencies = File.ReadLines(dependenciesFilePath).ToList();
            var contentOfDependencies = linesOfDependencies
                                        .Where(line => !line.Contains("_Package_Version"))
                                        .ToList();

            var packageVersionLine = linesOfDependencies.FirstOrDefault(line => line.Contains("PropertyGroup Label=\"Package Versions\""));
            Log.Information(" # Result in dependency.props");
            if (packageVersionLine != null)
            {
                var projectTagIndex = linesOfDependencies.IndexOf(packageVersionLine);
                contentOfDependencies.InsertRange(projectTagIndex + 1, lines);
            }
            else
            {
                lines.Insert(0, "  <PropertyGroup Label=\"Package Versions\">");
                lines.Add("  </PropertyGroup>");
                contentOfDependencies.InsertRange(contentOfDependencies.Count - 1, lines);
            }

            contentOfDependencies.ForEach(x => Log.Information(x));
            File.WriteAllLines(dependenciesFilePath, contentOfDependencies);
        }


        protected override List<string> GetParametersFromLine(string line)
        {
            line = line.Trim();
            line = line.Substring(1, line.Length - 1);
            var list = (new List<string>() { line.Split('>')[0] }).Concat(line.Split('>')[1].Split("</")).ToList();
            return list;
        }
    }
}
