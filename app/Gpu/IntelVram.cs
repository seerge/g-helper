using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GHelper.Gpu;

public static class IntelVram
{
    private const string DisplayClass = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
    private const string ValueName = "DedicatedSegmentSize";

    [DllImport("kernel32.dll")]
    private static extern bool GetPhysicallyInstalledSystemMemory(out long totalKb);

    private static string? key;

    public static bool Supported => key is not null;

    public static int[] Sizes { get; private set; } = [];

    public static int Index { get; private set; }

    public static bool Init()
    {
        key = FindKey();
        if (key is null) return false;

        int max = GetPhysicallyInstalledSystemMemory(out long memory) ? (int)(memory >> 21) : 4;
        Sizes = [.. Enumerable.Range(0, 16).Select(i => 1 << i).TakeWhile(size => size <= max)];

        using var adapter = Registry.LocalMachine.OpenSubKey(key);
        Index = Array.IndexOf(Sizes, (adapter?.GetValue(ValueName) as int? ?? 0) / 1024) + 1;

        return true;
    }

    public static void SetIndex(int index)
    {
        try
        {
            if (index < 1)
            {
                using var existing = Registry.LocalMachine.OpenSubKey(key, true);
                existing?.DeleteValue(ValueName, false);
                return;
            }

            using var adapter = Registry.LocalMachine.CreateSubKey(key);
            adapter?.SetValue(ValueName, Sizes[index - 1] * 1024, RegistryValueKind.DWord);
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Failed to set Intel VRAM size: {ex.Message}");
        }
    }

    private static string? FindKey()
    {
        using var display = Registry.LocalMachine.OpenSubKey(DisplayClass);
        if (display is null) return null;

        foreach (string name in display.GetSubKeyNames())
        {
            using var adapter = display.OpenSubKey(name);
            if (adapter?.GetValue("DriverDesc") is string desc && desc.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                return $@"{DisplayClass}\{name}\GMM";
        }

        return null;
    }
}
