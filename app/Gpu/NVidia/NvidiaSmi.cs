using PawnIO;
using System.Diagnostics;
using System.Text.RegularExpressions;

public static class NvidiaSmi
{

    public static int GetDefaultMaxGPUPower()
    {
        if (AppConfig.ContainsModel("GU605") || AppConfig.ContainsModel("GA605")) return 125;
        if (AppConfig.ContainsModel("GA403")) return 90;
        if (AppConfig.ContainsModel("FA607")) return 140;
        else return 175;
    }

    public static int GetMaxGPUPower()
    {
        string output = RunNvidiaSmiCommand("--query-gpu=power.max_limit --format csv,noheader,nounits");
        output = output.Trim().Trim('\n', '\r').Replace(".00","").Replace(",00", "");

        if (float.TryParse(output, out float floatValue))
        {
            int intValue = (int)floatValue;
            if (intValue >= 50 && intValue <= 175) return intValue;
        }

        return GetDefaultMaxGPUPower();
    }

    private static string RunNvidiaSmiCommand(string arguments = "-i 0 -q")
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "nvidia-smi",
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = new Process { StartInfo = startInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        catch (Exception ex)
        {
            //return File.ReadAllText(@"smi.txt");
            Debug.WriteLine(ex.Message);
        }

        return "";

    }
}
