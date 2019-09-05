namespace dependenciesScanner.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class PackageScannModel
    {
        public PackageScannModel(List<string> parameters)
        {
            if (parameters.Count() >= 2)
            {
                this.Name = parameters.First().Replace("\"", "");
                this.Version = parameters[1]?.Replace("\"", "");
            }

            if (parameters.Count() >= 3)
            {
                this.PrivateAsset = parameters[2]?.Replace("\"", "");
            }
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string PrivateAsset { get; set; }
    }
}