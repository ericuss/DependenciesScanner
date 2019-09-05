namespace dependenciesScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Services;

    class Program
    {
        const string path = ".";
        // const string path = "../../../../src/";
        // const string path = "../src/";
        private static DirectoryBuildPropsFileService directoryBuildFileService => new DirectoryBuildPropsFileService();
        private static DependenciesPropsFileService dependenciesFileService => new DependenciesPropsFileService();
        private static CsProjFileService csprojsFileService => new CsProjFileService();

        static void Main(string[] args)
        {
            Console.WriteLine(" -- Starting...");
            Console.WriteLine("        Parent directory:" + Directory.GetParent("."));

            Console.WriteLine(" -- Search and/or create dependency.props");
            var dependenciesFile = dependenciesFileService.SearchDependencyPropsAndCreateIfNotExist(path);
            Console.WriteLine(" -- Create if not exist the DirectoryBuild.props");
            directoryBuildFileService.SearchDirectoryBuildPropsAndCreateIfNotExist(path);
            var packages = new Dictionary<string, List<string>>();

            Console.WriteLine(" -- Scan packages in dependency.props");
            dependenciesFileService.ScanPackagesFromDependency(dependenciesFile, packages);

            Console.WriteLine(" -- Search all CsProjs");
            var csProjFiles = csprojsFileService.GetAllCsProjs(path);
            Console.WriteLine(" -- Scan packages in CsProjs");
            csProjFiles.ToList().ForEach(csprojPath => csprojsFileService.ScanPackagesFromCsProj(csprojPath, packages));

            Console.WriteLine(" -- Packages and versions");
            packages.ToList().ForEach(x => Console.WriteLine($"        {x.Key} {string.Join(",", x.Value)}"));

            Console.WriteLine(" -- Update dependency.props");
            dependenciesFileService.WriteToFile(dependenciesFile, packages);
        }
    }
}
