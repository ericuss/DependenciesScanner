namespace dependenciesScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using dependenciesScanner.Commands;
    using dependenciesScanner.Models;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Services;

    class Program
    {
        private static Parameters parameters;

        static void Main(string[] args)
        {
            Initialize(args);
            new ShowHelpCommand(args).Execute();
            Log.Verbose(" -- Starting...");
            Log.Verbose("        settings:{@settings}", parameters);
            new DependenciesScannerCommand(parameters, args).Execute();
            new AddNugetToDependencyCommand(parameters, args).Execute();
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
                 { "--root", "Path" },
                 { "--add", "NugetPackage" },
                 { "--project", "ProjectFilePath" },
                 { "--pr", "ProjectFilePath" },
             };

            var builder = new ConfigurationBuilder()
                                .AddCommandLine(args, switchMappings)
                                .Build();
            parameters = builder.Get<Parameters>() ?? new Parameters();
            
            if(!string.IsNullOrWhiteSpace(parameters.Path) && parameters.Path[parameters.Path.Length -1] != '/'){
                parameters.Path += "/";
            }
        }

        private static void ShowHelp(string[] args)
        {

            var versionString = Assembly.GetEntryAssembly()
                                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                   .InformationalVersion
                                   .ToString();

            Console.WriteLine($"depScanner v{versionString}");
            Console.WriteLine("\nUsage: dotnet-depscanner [--options]");
            Console.WriteLine("Usage: dotnet-depscanner --p ./src");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("--p              Set root path");
            Console.WriteLine("--h/--help       Show help");
        }
    }
}
