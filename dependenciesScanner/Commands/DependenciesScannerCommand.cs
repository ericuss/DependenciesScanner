namespace dependenciesScanner.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using dependenciesScanner.Models;
    using dependenciesScanner.Services;
    using Serilog;

    public class DependenciesScannerCommand : CommandCore
    {
        private readonly Parameters parameters;
        private readonly string[] args;

        public DependenciesScannerCommand(Parameters parameters, string[] args)
        {
            this.args = args;
            this.parameters = parameters;
        }

        protected override bool CanExecute()
        {
            return args.Contains("--p");
        }

        protected override void Action()
        {
            var directoryBuildFileService = new DirectoryBuildPropsFileService();
            var dependenciesFileService = new DependenciesPropsFileService();
            var csprojsFileService = new CsProjFileService();
            var packages = new Dictionary<string, List<string>>();

            Log.Information(" -- Search and/or create dependency.props");
            var dependenciesFile = dependenciesFileService.SearchDependencyPropsAndCreateIfNotExist(parameters.Path);

            Log.Information(" -- Create if not exist the DirectoryBuild.props");
            directoryBuildFileService.SearchDirectoryBuildPropsAndCreateIfNotExist(parameters.Path);

            Log.Information(" -- Scan packages in dependency.props");
            dependenciesFileService.ScanPackagesFromDependency(dependenciesFile, packages);

            Log.Information(" -- Search all CsProjs");
            var csProjFiles = csprojsFileService.GetAllCsProjs(parameters.Path);

            Log.Information(" -- Scan packages in CsProjs");
            csProjFiles.ToList().ForEach(csprojPath => csprojsFileService.ScanPackagesFromCsProj(csprojPath, packages));

            Log.Information(" -- Packages and versions");
            packages.ToList().ForEach(x => Log.Information($"        {x.Key} {string.Join(",", x.Value)}"));

            Log.Information(" -- Update dependency.props");
            dependenciesFileService.WriteToFile(dependenciesFile, packages);
        }
    }
}