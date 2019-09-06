namespace dependenciesScanner.Commands
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using dependenciesScanner.Models;
    using dependenciesScanner.Services;
    using Serilog;

    public class AddNugetToDependencyCommand : CommandCore
    {
        private readonly Parameters parameters;
        private readonly string[] args;

        public AddNugetToDependencyCommand(Parameters parameters, string[] args)
        {
            this.args = args;
            this.parameters = parameters;
        }

        protected override bool CanExecute()
        {
            if (!args.Contains("--add")) return false;
            var existProjectFilePath = File.Exists(this.parameters.ProjectFilePath);
            if (!existProjectFilePath) Log.Error("Project file path not exist");
            var existRootPath = File.Exists(this.parameters.Path);
            if (!existRootPath) Log.Error("Project file path not exist");
            return !string.IsNullOrWhiteSpace(this.parameters.NugetPackage)
                    && existProjectFilePath
                    && existRootPath
                    ;
        }

        protected override void Action()
        {

        }
    }
}