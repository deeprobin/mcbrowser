using System.Diagnostics;

namespace MinecraftServerlist;

internal sealed class StripeHandle : IDisposable
{
    private readonly Process _process;

    public StripeHandle(string webHook)
    {
        var processStartInfo = new ProcessStartInfo("stripe")
        {
            UseShellExecute = true,
            Arguments = $"listen --forward-to {webHook}",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            RedirectStandardInput = false
        };
        _process = Process.Start(processStartInfo)!;
    }

    public void Dispose()
    {
        _process.Kill();
        _process.Dispose();
    }

    public static bool IsStripeInstalled()
    {
        var processStartInfo = new ProcessStartInfo("stripe")
        {
            UseShellExecute = true,
            Arguments = "--version",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            RedirectStandardInput = false
        };
        try
        {
            var process = Process.Start(processStartInfo);
            var line = process?.StandardOutput.ReadToEnd();
            if (line is null)
            {
                return false;
            }

            var split = line.Split(' ');
            if (split.Length == 3 && split[0] == "stripe" && split[1] == "version")
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}