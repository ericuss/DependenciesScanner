namespace dependenciesScanner.Services
{
    using System.Diagnostics;
    using Serilog;

    public static class ExecuteExternalAppService
    {
        public static string Execute(string commandName, string arguments)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = commandName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            proc.Start();

            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                return "error: " + error;

            string output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            Log.Information(output);
            return output;
        }
    }
}
