using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GHelper.Helpers
{
    internal static class SingleInstance
    {
        private const string ExitEventName = "Global\\GHelperApp-Exit";
        private const int WaitForOthersMs = 3000;

        private static EventWaitHandle? _exitEvent;
        private static RegisteredWaitHandle? _waitHandle;

        public static void Acquire()
        {
            var evt = OpenOrCreateExitEvent();

            using var current = Process.GetCurrentProcess();
            var others = OtherInstances(current);

            if (others.Length > 0)
            {
                Logger.WriteLine($"Signaling {others.Length} other GHelper instance(s) to exit");
                try { evt?.Set(); } catch { }

                var deadline = Environment.TickCount + WaitForOthersMs;
                while (Environment.TickCount < deadline)
                {
                    foreach (var p in others) p.Dispose();
                    others = OtherInstances(current);
                    if (others.Length == 0) break;
                    Thread.Sleep(100);
                }

                foreach (var p in others)
                {
                    try
                    {
                        p.Kill();
                        Logger.WriteLine($"Force-killed GHelper PID {p.Id} (session {SafeSession(p)})");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine($"Can't kill PID {p.Id}: {ex.Message}");
                    }
                    finally { p.Dispose(); }
                }

                try { evt?.Reset(); } catch { }
            }

            if (evt != null)
            {
                _exitEvent = evt;
                _waitHandle = ThreadPool.RegisterWaitForSingleObject(
                    evt, OnExitSignaled, null, Timeout.Infinite, executeOnlyOnce: true);
            }
        }

        public static void TriggerExit(string reason)
        {
            Logger.WriteLine($"SingleInstance exit: {reason}");
            try
            {
                Form? form = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                if (form != null && form.InvokeRequired)
                    form.BeginInvoke((Action)Application.Exit);
                else
                    Application.Exit();
            }
            catch
            {
                Environment.Exit(0);
            }
        }

        private static void OnExitSignaled(object? state, bool timedOut)
        {
            TriggerExit("global exit event signaled");
        }

        private static Process[] OtherInstances(Process current)
        {
            return Process.GetProcessesByName(current.ProcessName)
                          .Where(p => p.Id != current.Id)
                          .ToArray();
        }

        private static int SafeSession(Process p)
        {
            try { return p.SessionId; } catch { return -1; }
        }

        private static EventWaitHandle? OpenOrCreateExitEvent()
        {
            try
            {
                var sec = new EventWaitHandleSecurity();
                sec.AddAccessRule(new EventWaitHandleAccessRule(
                    new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                    EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify,
                    AccessControlType.Allow));

                return EventWaitHandleAcl.Create(
                    initialState: false,
                    mode: EventResetMode.ManualReset,
                    name: ExitEventName,
                    createdNew: out _,
                    eventSecurity: sec);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Can't create global exit event: " + ex.Message);
                try { return EventWaitHandle.OpenExisting(ExitEventName); }
                catch { return null; }
            }
        }
    }
}
