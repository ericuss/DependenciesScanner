namespace dependenciesScanner.Commands
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class ShowHelpCommand : CommandCore
    {
        private readonly string[] args;

        public ShowHelpCommand(string[] args)
        {
            this.args = args;
        }

        protected override bool CanExecute()
        {
            return !args.Any() || args.Contains("--h") || args.Contains("--help");
        }

        protected override void Action()
        {
            var versionString = Assembly.GetEntryAssembly()
                                     .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                     .InformationalVersion
                                     .ToString();
                                     
            Console.WriteLine($"\ndepScanner v{versionString}");
            Console.WriteLine(@"
......(\_/)
......( '_')
..../""""""""""""\======░ ▒▓▓█D
/'""""""""""""""""""""""""""""""""\
\__@__@__@__@__@__/   
");
            Console.WriteLine("\nDependencies Scanner.");
            Console.WriteLine("     For scan dependencies and generate packages in dependencies.props");
            Console.WriteLine("     Usage: dotnet-depscanner [--options]");
            Console.WriteLine("     Usage: dotnet-depscanner --p ./src");
            Console.WriteLine("\n     Options:");
            Console.WriteLine("     --p              Set root path");
            Console.WriteLine("     --h/--help       Show help");

            Console.WriteLine("\n\nAdd Dependency.");
            Console.WriteLine("     For add dependency to project and dependencies.props");
            Console.WriteLine("     Usage: dotnet-depscanner --add {NUGET.PACKAGE} [--options]");
            Console.WriteLine("     Usage: dotnet-depscanner --add Serilog --root ./src --project ./src/clients/clients.csproj");
            Console.WriteLine("\n     Options:");
            Console.WriteLine("     --p              Set root path");
            Console.WriteLine("     --h/--help       Show help");
            Console.WriteLine("\n");
        }
    }
}