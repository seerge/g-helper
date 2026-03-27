using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GHelper
{
    internal static class UserPath
    {
        #region P/Invoke

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            int sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out int pBytesReturned);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);

        [DllImport("kernel32.dll")]
        private static extern int WTSGetActiveConsoleSessionId();

        private enum WTS_INFO_CLASS { WTSUserName = 5 }

        #endregion

        private const string ProfileListKey =
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";

        private const string LogonUIKey =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI";

        /// <summary>
        /// Returns the AppData\Roaming path for the target user.
        /// Resolution order:
        ///   1. Active console session (user is logged in)
        ///   2. Last logged-on user via LogonUI SID (survives cold boot)
        ///   3. Most recently used non-system profile (by ProfileLoadTime)
        ///   4. Current process AppData (fallback)
        /// </summary>
        public static string GetAppDataPath()
        {
            return TryGetActiveSessionAppData()
                ?? TryGetLastLoggedOnAppData()
                ?? TryGetMostRecentProfileAppData()
                ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        // ── Priority 1 ───────────────────────────────────────────────────────────

        private static string TryGetActiveSessionAppData()
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                int sessionId = WTSGetActiveConsoleSessionId();
                if (sessionId <= 0) return null;

                if (!WTSQuerySessionInformation(IntPtr.Zero, sessionId,
                    WTS_INFO_CLASS.WTSUserName, out buffer, out _))
                    return null;

                string userName = Marshal.PtrToStringUni(buffer);
                if (string.IsNullOrWhiteSpace(userName)) return null;

                return ProfilePathFromUsername(userName);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"[UserPath] Active session lookup failed: {ex.Message}");
                return null;
            }
            finally
            {
                if (buffer != IntPtr.Zero) WTSFreeMemory(buffer);
            }
        }

        // ── Priority 2 ───────────────────────────────────────────────────────────

        private static string TryGetLastLoggedOnAppData()
        {
            try
            {
                using var logonKey = Registry.LocalMachine.OpenSubKey(LogonUIKey);
                string sid = logonKey?.GetValue("LastLoggedOnUserSID") as string;

                if (string.IsNullOrWhiteSpace(sid)) return null;

                using var profileKey = Registry.LocalMachine
                    .OpenSubKey($@"{ProfileListKey}\{sid}");

                string profilePath = profileKey?.GetValue("ProfileImagePath") as string;
                if (profilePath == null) return null;

                return RoamingFromProfile(profilePath);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"[UserPath] LastLoggedOn lookup failed: {ex.Message}");
                return null;
            }
        }

        // ── Priority 3 ───────────────────────────────────────────────────────────

        private static string TryGetMostRecentProfileAppData()
        {
            try
            {
                using var profileList = Registry.LocalMachine.OpenSubKey(ProfileListKey);
                if (profileList == null) return null;

                string bestPath = null;
                long bestTime = 0;

                foreach (string sid in profileList.GetSubKeyNames())
                {
                    // Skip built-in accounts: SYSTEM (S-1-5-18), LocalService,
                    // NetworkService, etc. Only real user SIDs start with S-1-5-21-
                    if (!sid.StartsWith("S-1-5-21-")) continue;

                    using var key = profileList.OpenSubKey(sid);
                    if (key == null) continue;

                    int high = (int)(key.GetValue("ProfileLoadTimeHigh") ?? 0);
                    int low = (int)(key.GetValue("ProfileLoadTimeLow") ?? 0);
                    long fileTime = ((long)(uint)high << 32) | (uint)low;

                    if (fileTime > bestTime &&
                        key.GetValue("ProfileImagePath") is string path)
                    {
                        bestTime = fileTime;
                        bestPath = path;
                    }
                }

                return bestPath != null ? RoamingFromProfile(bestPath) : null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"[UserPath] MostRecentProfile lookup failed: {ex.Message}");
                return null;
            }
        }

        // ── Shared helpers ────────────────────────────────────────────────────────

        /// <summary>
        /// Resolves a username (plain or DOMAIN\user) to the profile's AppData\Roaming
        /// path by scanning the ProfileList registry hive.
        /// </summary>
        private static string ProfilePathFromUsername(string userName)
        {
            try
            {
                string shortName = userName.Contains('\\')
                    ? userName.Split('\\')[1]
                    : userName;

                using var profileList = Registry.LocalMachine.OpenSubKey(ProfileListKey);
                if (profileList == null) return null;

                foreach (string sid in profileList.GetSubKeyNames())
                {
                    using var key = profileList.OpenSubKey(sid);
                    if (key?.GetValue("ProfileImagePath") is string profilePath &&
                        Path.GetFileName(Environment.ExpandEnvironmentVariables(profilePath))
                            .Equals(shortName, StringComparison.OrdinalIgnoreCase))
                    {
                        return RoamingFromProfile(profilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"[UserPath] ProfileFromUsername failed: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Expands environment variables and appends AppData\Roaming to a profile root path.
        /// Only returns the path if the directory actually exists on disk.
        /// </summary>
        private static string RoamingFromProfile(string profilePath)
        {
            string expanded = Environment.ExpandEnvironmentVariables(profilePath);
            string roaming = Path.Combine(expanded, "AppData", "Roaming");
            return Directory.Exists(roaming) ? roaming : null;
        }
    }
}
