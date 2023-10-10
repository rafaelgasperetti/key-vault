using key_vault.Models;
using System.Diagnostics;

namespace key_vault.Helpers
{
    public static class CmdHelper
    {
        public static CmdResult Execute(string program, string arguments, string directory = null)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                throw new Exception("Invalid command");
            }

            try
            {
                Process p = new();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = program;
                p.StartInfo.Arguments = arguments;

                if (!string.IsNullOrEmpty(directory))
                {
                    p.StartInfo.WorkingDirectory = directory;
                }

                var started = p.Start();

                if (!started)
                {
                    string command = $"{program} {arguments}";
                    throw new Exception(string.Format("Command not started: {0}", command));
                }

                p.WaitForExit();

                string output = null;
                bool success = !Convert.ToBoolean(p.ExitCode);

                if (success)
                {
                    output = p.StandardOutput.ReadToEnd();
                }
                else
                {
                    output = p.StandardError.ReadToEnd();
                }

                return new CmdResult()
                {
                    Success = success,
                    ExitCode = p.ExitCode,
                    ExitTime = p.ExitTime,
                    Message = string.IsNullOrEmpty(output) ? null : output.Trim()
                };
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("It was not possible to run {0}.", program), ex);
            }
        }
    }
}
