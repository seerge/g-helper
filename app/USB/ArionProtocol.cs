// Protocol reference: OpenRGB, by Adam Honse (CalcProgrammer1), GPL-2.0-or-later.
// https://github.com/CalcProgrammer1/OpenRGB/tree/326d2138e04503df602f685470abd64dc7e4ef8e/Controllers/ENESMBusController
// Adapted for G-Helper under GPL-3.0.
namespace GHelper.USB;

internal readonly record struct ArionRegisterWrite(ushort Register, byte[] Data);

internal static class ArionProtocol
{
    public const int MaxBlockLength = 24;

    public static string? GetDevicePath(string dosDeviceName)
    {
        string[] parts = dosDeviceName.Split('#');
        if (parts.Length < 3 || parts[2].Length == 0 || !parts[0].Equals("SCSI", StringComparison.OrdinalIgnoreCase)) return null;

        string[] identity = parts[1].Split('&');
        if (identity.Length < 3 || identity.Length > 4 || !identity[0].Equals("Disk", StringComparison.OrdinalIgnoreCase)) return null;
        if (identity.Length == 4 && !identity[3].StartsWith("Rev_", StringComparison.OrdinalIgnoreCase)) return null;
        if (!identity[1].TrimEnd('_', ' ').Equals("Ven_ROG", StringComparison.OrdinalIgnoreCase) ||
            !identity[2].TrimEnd('_', ' ').Equals("Prod_ESD-S1C", StringComparison.OrdinalIgnoreCase)) return null;

        return @"\\?\" + dosDeviceName;
    }

    public static byte[] BuildCdb(ushort register, int length)
    {
        if (length < 1 || length > MaxBlockLength) throw new ArgumentOutOfRangeException(nameof(length));

        byte[] cdb = new byte[16];
        cdb[0] = 0xEC;
        cdb[1] = 0x41;
        cdb[2] = 0x53;
        cdb[3] = (byte)(register >> 8);
        cdb[4] = (byte)register;
        cdb[13] = (byte)length;
        return cdb;
    }

    // ENE modes: off=0, static=1, breathing=2, flashing=3, cycle=4, rainbow=5.
    // ENE speeds run in the opposite direction to Aura: 4=slowest, 0=fastest.
    public static ArionRegisterWrite[] BuildEffect(byte mode, byte speed, byte red, byte green, byte blue)
    {
        if (mode > 5) throw new ArgumentOutOfRangeException(nameof(mode));
        if (speed > 4) throw new ArgumentOutOfRangeException(nameof(speed));

        byte[] colors = new byte[12];
        for (int led = 0; led < 4; led++)
        {
            colors[led * 3] = red;
            colors[led * 3 + 1] = blue;
            colors[led * 3 + 2] = green;
        }

        return
        [
            new(0x8160, colors),    // Second-generation ENE effect colors, four LEDs in R-B-G order.
            new(0x8020, [0x00]),    // Leave direct mode.
            new(0x8021, [mode]),
            new(0x8022, [speed]),
            new(0x8023, [0x00]),    // Forward direction.
            new(0x80A0, [0x01]),    // Apply in RAM only; do not save to flash (0xAA).
        ];
    }
}
