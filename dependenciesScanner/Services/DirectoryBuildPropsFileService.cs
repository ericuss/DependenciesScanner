namespace dependenciesScanner.Services
{
    using System.IO;
    using System.Linq;

    public class DirectoryBuildPropsFileService : FileServiceCore
    {
        private const string DirectoryBuildFileName = "Directory.Build.props";

        public string SearchDirectoryBuildPropsAndCreateIfNotExist(string sourcePath)
        {
            return this.SearchAndCreateIfNotExist(sourcePath, DirectoryBuildFileName, this.CreateDirectoryBuildPropsFile)
                    .FirstOrDefault();
        }

        public string CreateDirectoryBuildPropsFile(string sourcePath, string fileName)
        {
            var content = ResourcesServices.GetDirectoryBuildPropsTemplate();
            var filePath = sourcePath + fileName;
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}
