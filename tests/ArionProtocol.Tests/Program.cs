using GHelper.USB;

int passed = 0;
void Check(string name, Action test)
{
    test();
    Console.WriteLine($"PASS {name}");
    passed++;
}
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"Expected {expected}, got {actual}");
}
void Bytes(byte[] expected, byte[] actual) => Equal(Convert.ToHexString(expected), Convert.ToHexString(actual));
void OutOfRange(Action action)
{
    try { action(); }
    catch (ArgumentOutOfRangeException) { return; }
    throw new Exception("Expected ArgumentOutOfRangeException");
}

Check("OpenRGB Arion CDB vector, second-generation effect colors", () =>
    Bytes([0xEC, 0x41, 0x53, 0x81, 0x60, 0, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0], ArionProtocol.BuildCdb(0x8160, 12)));
Check("OpenRGB apply CDB vector", () =>
    Bytes([0xEC, 0x41, 0x53, 0x80, 0xA0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0], ArionProtocol.BuildCdb(0x80A0, 1)));
Check("CDB block length boundaries", () =>
{
    Equal((byte)24, ArionProtocol.BuildCdb(0x8160, 24)[13]);
    foreach (int length in new[] { int.MinValue, -1, 0, 25, 256, int.MaxValue })
        OutOfRange(() => ArionProtocol.BuildCdb(0x8160, length));
});
Check("Four LED mixed color vector uses R-B-G, not RGB", () =>
    Bytes([0x12, 0x56, 0x34, 0x12, 0x56, 0x34, 0x12, 0x56, 0x34, 0x12, 0x56, 0x34],
        ArionProtocol.BuildEffect(1, 2, 0x12, 0x34, 0x56)[0].Data));
Check("Complete volatile-only transaction and mode/speed range", () =>
{
    for (byte mode = 0; mode <= 5; mode++)
    for (byte speed = 0; speed <= 4; speed++)
    {
        var writes = ArionProtocol.BuildEffect(mode, speed, 1, 2, 3);
        Equal(6, writes.Length);
        Equal("8160,8020,8021,8022,8023,80A0", string.Join(',', writes.Select(w => w.Register.ToString("X4"))));
        Bytes([0], writes[1].Data);
        Bytes([mode], writes[2].Data);
        Bytes([speed], writes[3].Data);
        Bytes([0], writes[4].Data);
        Bytes([1], writes[5].Data);
        foreach (var write in writes) ArionProtocol.BuildCdb(write.Register, write.Data.Length);
    }
    foreach (byte mode in new byte[] { 6, 14, 255 })
        OutOfRange(() => ArionProtocol.BuildEffect(mode, 2, 0, 0, 0));
    OutOfRange(() => ArionProtocol.BuildEffect(1, 5, 0, 0, 0));
});
Check("Device identity allows padding and case differences", () =>
{
    const string name = "SCSI#Disk&Ven_ROG_____&Prod_ESD-S1C_________#123#{GUID}";
    Equal(@"\\?\" + name, ArionProtocol.GetDevicePath(name));
    Equal(@"\\?\" + name.ToLowerInvariant(), ArionProtocol.GetDevicePath(name.ToLowerInvariant()));
    const string revision = "SCSI#Disk&Ven_ROG&Prod_ESD-S1C&Rev_1.00#123#{GUID}";
    Equal(@"\\?\" + revision, ArionProtocol.GetDevicePath(revision));
});
Check("Unrelated disks, variants and malformed paths are rejected", () =>
{
    foreach (string name in new[]
    {
        "", "PhysicalDrive0", "USB#Disk&Ven_ROG&Prod_ESD-S1C#123",
        "SCSI#Disk&Ven_OTHER&Prod_ESD-S1C#123", "SCSI#Disk&Ven_ROGUE&Prod_ESD-S1C#123",
        "SCSI#Disk&Ven_ROG&Prod_ESD-S1CL#123", "SCSI#Disk&Ven_ROG&Prod_OTHER#123",
        "SCSI#CdRom&Ven_ROG&Prod_ESD-S1C#123", "SCSI#Disk&Ven_ROG&Prod_ESD-S1C",
        "SCSI#Disk&Ven_ROG&Prod_ESD-S1C#", "SCSI#Disk&Ven_ROG&Prod_ESD-S1C&Other#123",
    }) Equal<string?>(null, ArionProtocol.GetDevicePath(name));
});

Console.WriteLine($"{passed} protocol checks passed. No Windows I/O or hardware was exercised.");
