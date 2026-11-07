using System.Drawing;
using System.Reflection;

// Reflection lets the initial test run fail for the missing feature, not a compile error.
Type? renderer = Assembly.GetExecutingAssembly().GetType("GHelper.USB.AuraAnimation");
Check(renderer != null, "animation renderer exists");
Type effectType = Assembly.GetExecutingAssembly().GetType("GHelper.USB.AuraAnimationEffect")!;
Type playerType = Assembly.GetExecutingAssembly().GetType("GHelper.USB.AuraAnimationPlayer")!;
MethodInfo render = renderer!.GetMethod("Render")!;
Color[] Frame(string effect, double seconds, bool reverse = false, double cycle = 4) =>
    (Color[])render.Invoke(null, [Enum.Parse(effectType, effect), seconds, cycle, reverse, Color.Red, Color.Blue])!;
bool Equal(Color[] a, Color[] b) => a.Select(c => c.ToArgb()).SequenceEqual(b.Select(c => c.ToArgb()));

foreach (string effect in new[] { "Rainbow", "Wave", "Chase", "Breathe" })
{
    Color[] first = Frame(effect, 0);
    Check(first.Length == 8, $"{effect}: four keyboard and four lightbar colors");
    Check(first.Take(4).Select(c => c.ToArgb()).SequenceEqual(first.Skip(4).Select(c => c.ToArgb())), $"{effect}: keyboard and lightbar aligned");
    Check(!Equal(first, Frame(effect, 1)), $"{effect}: colors change with time");
    Check(Equal(first, Frame(effect, 4)), $"{effect}: seamless full cycle");
    Check(Equal(Frame(effect, 0.6, true), Frame(effect, -0.6)), $"{effect}: reverse runs the timeline backwards");
    Check(Equal(Frame(effect, 1, cycle: 4), Frame(effect, 0.5, cycle: 2)), $"{effect}: faster setting preserves motion path");
}
Check(Frame("Rainbow", 0).Take(4).Select(c => c.ToArgb()).Distinct().Count() == 4, "rainbow has spatial color variation");
Color[] chaseStart = Frame("Chase", 0), chaseNext = Frame("Chase", 1);
Check(chaseStart[0].R == 255 && chaseNext[1].R == 255 && chaseNext[0].R < chaseStart[0].R, "chase moves from zone one to zone two");
Check(Frame("Breathe", 0)[0].R > Frame("Breathe", 2)[0].R, "breathing modulates brightness");
Check(Frame("Wave", 0).All(c => c.G == 0), "two-color wave stays between the chosen red/blue colors");

// Worker tests exercise asynchronous lifecycle behavior without any hardware API.
object player = Activator.CreateInstance(playerType)!;
MethodInfo start = playerType.GetMethod("Start")!, stop = playerType.GetMethod("Stop")!;
int writes = 0, errors = 0, initialized = 0, activeWrites = 0, peakWrites = 0;
bool active = true;
var firstWrite = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var resumedWrite = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
Action<Color[], bool> send = (colors, init) =>
{
    int concurrent = Interlocked.Increment(ref activeWrites);
    peakWrites = Math.Max(peakWrites, concurrent);
    if (init && Interlocked.Increment(ref initialized) > 1) resumedWrite.TrySetResult();
    Interlocked.Increment(ref writes);
    firstWrite.TrySetResult();
    Thread.Sleep(30); // Slower than the test interval: no backlog or overlap is allowed.
    Interlocked.Decrement(ref activeWrites);
};
void Start(Action<Color[], bool> output) => start.Invoke(player, [
    Enum.Parse(effectType, "Rainbow"), 4d, false, Color.Red, Color.Blue,
    output, (Func<bool>)(() => Volatile.Read(ref active)), (Action<Exception>)(_ => Interlocked.Increment(ref errors)), 10]);
Start(send);
await firstWrite.Task.WaitAsync(TimeSpan.FromSeconds(3));
await Task.Delay(100);
Check(initialized == 1 && peakWrites == 1, "worker initializes once and never overlaps writes");
Volatile.Write(ref active, false);
await Task.Delay(80);
int pausedWrites = Volatile.Read(ref writes);
await Task.Delay(80);
Check(writes == pausedWrites, "inactive backlight pauses writes");
Volatile.Write(ref active, true);
await resumedWrite.Task.WaitAsync(TimeSpan.FromSeconds(3));
stop.Invoke(player, null);
int stoppedWrites = Volatile.Read(ref writes);
await Task.Delay(80);
Check(writes == stoppedWrites && activeWrites == 0, "stop drains in-flight write and prevents late frames");

var replacementWrite = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
Start((_, _) => replacementWrite.TrySetResult());
await replacementWrite.Task.WaitAsync(TimeSpan.FromSeconds(3));
stop.Invoke(player, null);
Check(writes == stoppedWrites, "restart does not revive old callback");
Start((_, _) => throw new IOException("simulated device disconnect"));
await Task.Delay(100);
Check(errors == 1, "transport failure stops once instead of retrying every frame");
stop.Invoke(player, null);
Console.WriteLine("All offline animation checks passed. Hardware was not accessed.");

static void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
}
