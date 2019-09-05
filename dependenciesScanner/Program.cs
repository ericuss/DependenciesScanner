namespace dependenciesScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using dependenciesScanner.Models;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Services;

    class Program
    {
        private static DirectoryBuildPropsFileService directoryBuildFileService => new DirectoryBuildPropsFileService();
        private static DependenciesPropsFileService dependenciesFileService => new DependenciesPropsFileService();
        private static CsProjFileService csprojsFileService => new CsProjFileService();
        private static AppSettings appSettings;

        static void Main(string[] args)
        {
            Initialize(args);
            Log.Information(" -- Starting...");
            Log.Information("        settings:{@settings}", appSettings);
            Log.Information($"        Parent directory: {Directory.GetParent(".")}, path: {appSettings.Path}");

            Log.Information(" -- Search and/or create dependency.props");
            var dependenciesFile = dependenciesFileService.SearchDependencyPropsAndCreateIfNotExist(appSettings.Path);

            Log.Information(" -- Create if not exist the DirectoryBuild.props");
            directoryBuildFileService.SearchDirectoryBuildPropsAndCreateIfNotExist(appSettings.Path);
            var packages = new Dictionary<string, List<string>>();

            Log.Information(" -- Scan packages in dependency.props");
            dependenciesFileService.ScanPackagesFromDependency(dependenciesFile, packages);

            Log.Information(" -- Search all CsProjs");
            var csProjFiles = csprojsFileService.GetAllCsProjs(appSettings.Path);

            Log.Information(" -- Scan packages in CsProjs");
            csProjFiles.ToList().ForEach(csprojPath => csprojsFileService.ScanPackagesFromCsProj(csprojPath, packages));

            Log.Information(" -- Packages and versions");
            packages.ToList().ForEach(x => Log.Information($"        {x.Key} {string.Join(",", x.Value)}"));

            Log.Information(" -- Update dependency.props");
            dependenciesFileService.WriteToFile(dependenciesFile, packages);
        }

        private static void Initialize(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                                .WriteTo
                                .Console(Serilog.Events.LogEventLevel.Information)
                                .CreateLogger();
            var switchMappings = new Dictionary<string, string>()
             {
                 { "--p", "Path" },
             };

            var builder = new ConfigurationBuilder()
                                .AddCommandLine(args, switchMappings)
                                .Build();
            appSettings = builder.Get<AppSettings>() ?? new AppSettings();

        }
    }
}
