using System;
using System.Diagnostics;
using System.IO;
using System.Net;

class Program
{
    static void Main()
    {
        // Check if running with administrator privileges
        if (!IsAdmin())
        {
            Console.WriteLine("This application requires administrator privileges.");
            Console.WriteLine("Please run the application as an administrator.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Running with administrator privileges.");

        // Replace 'owner' and 'repo' with the GitHub owner and repository name
        string owner = "seerge";
        string repo = "g-helper";

        // Fetch the latest release information
        string downloadUrl = GetDownloadUrl(owner, repo);
        Console.WriteLine($"Download URL: {downloadUrl}");

        // Specify the process name
        string processName = "GHelper.exe";

        // Kill the process
        KillProcess(processName);

        // Get the full path of the application's directory
        string destinationFolder = AppDomain.CurrentDomain.BaseDirectory;

        // Loop through files in the directory
        foreach (var file in Directory.GetFiles(destinationFolder))
        {
            // Check if it's a batch file
            if (Path.GetFileName(file).Equals("Updater.exe", StringComparison.OrdinalIgnoreCase))
            {
                // Delete the file
                File.Delete(file);
                Console.WriteLine($"Deleted: {file}");
            }
        }

        Console.WriteLine("Files deleted, excluding batch files.");

        // Download the file
        DownloadFile(downloadUrl, Path.Combine(destinationFolder, "GHelper.zip"));

        // Change the current directory to the destination folder
        Environment.CurrentDirectory = destinationFolder;

        // Unzip the file
        Process.Start("powershell", $"-command \"Expand-Archive -Path '.\\GHelper.zip' -DestinationPath '.\\'\"").WaitForExit();

        // Delete the zip file
        File.Delete(Path.Combine(destinationFolder, "GHelper.zip"));
        Console.WriteLine("Zip file deleted.");

        Console.WriteLine("File downloaded and unzipped successfully.");
        Console.ReadLine();
    }

    static bool IsAdmin()
    {
        using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
        {
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }

    static string GetDownloadUrl(string owner, string repo)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = httpClient.GetStringAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest").Result;

            return response.Split("browser_download_url\":\"")[1].Split("\"")[0];
        }
    }


    static void KillProcess(string processName)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "taskkill",
            Arguments = $"/F /IM \"{processName}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processStartInfo })
        {
            process.Start();
            process.WaitForExit();
        }
    }

    static void DownloadFile(string url, string destinationPath)
    {
        using (var httpClient = new HttpClient())
        {
            var responseBytes = httpClient.GetByteArrayAsync(url).Result;
            File.WriteAllBytes(destinationPath, responseBytes);
        }
    }

}
