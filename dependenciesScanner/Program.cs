namespace dependenciesScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Services;

    class Program
    {
        private static DirectoryBuildPropsFileService directoryBuildFileService => new DirectoryBuildPropsFileService();
        private static DependenciesPropsFileService dependenciesFileService => new DependenciesPropsFileService();
        private static CsProjFileService csprojsFileService => new CsProjFileService();

        static void Main(string[] args)
        {
            Console.WriteLine(" -- Starting...");

            args.ToList().ForEach(x => Console.WriteLine("        arg:" + x));
            var path = GetPath(args);
            Console.WriteLine($"        Parent directory: {Directory.GetParent(".")}, path: {path}");

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

        private static string GetPath(string[] args)
        {
            var path = string.Empty;
            if (args.Any())
            {
                path = args[0];
            }

            if (path.ToCharArray()[path.Length - 1] != '/') path += "/";
            return path;
        }
    }
}
