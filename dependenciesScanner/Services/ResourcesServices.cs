namespace dependenciesScanner.Services
{
    using System.IO;
    using System.Reflection;

    public static class ResourcesServices
    {
        const string directoryBuildPropsConst = "dependenciesScanner.Directory.Build.sample.props";
        const string dependenciesPropsConst = "dependenciesScanner.dependencies.sample.props";

        public static string GetDependenciesPropsTemplate() => GetTemplateText(dependenciesPropsConst);

        public static string GetDirectoryBuildPropsTemplate() => GetTemplateText(directoryBuildPropsConst);

        private static string GetTemplateText(string fileName)
        {
            var template = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    template = reader.ReadToEnd();
                }
            }

            return template;
        }
    }
}
