// G-Helper glue for GK6X-family keyboards (Matrix Clix 60%).
//
// Behaviour:
//  * Disabled unless config.json has "gk6x_keyboard": 1. When disabled nothing is opened or sent.
//  * Follows G-Helper's own controls: laptop Aura colour ("aura_color") and laptop keyboard backlight
//    level (0..3). Level 0 = off (all slots black). Animated laptop modes are mirrored as their primary
//    colour only; nothing is ever streamed.
//  * The keyboard stores the colour in its own flash, so after a write nothing keeps running.
//    G-Helper does not need to stay open for the colour to persist.
//  * Requests are coalesced (debounce) and deduplicated against the last value written, so moving a
//    slider or resuming from sleep does not cause repeated flash writes.
//  * All USB work happens on one background task; the UI thread never blocks on the keyboard.

using GHelper.Input;
using System;
using System.Drawing;

namespace GHelper.Peripherals.Keyboard.Gk6x
{
    public static class Gk6xKeyboards
    {
        public const string EnableKey = "gk6x_keyboard";
        private const string LastWrittenKey = "gk6x_last_written";
        public const int DebounceMs = 750;
        public const int MinWriteIntervalMs = 2000;

        private static readonly Gk6xCoalescer coalescer =
            new(SyncOnce, DebounceMs, MinWriteIntervalMs, Logger.WriteLine);

        public static bool Enabled => AppConfig.Is(EnableKey);

        /// <summary>Called from Aura.ApplyAura, user brightness changes and HID device-change events.</summary>
        public static void RequestSync()
        {
            if (!Enabled) return;
            coalescer.Request();
        }

        public static (byte r, byte g, byte b) DesiredColour()
        {
            Color c = Color.FromArgb(AppConfig.Get("aura_color"));
            int level = InputDispatcher.GetBacklight(); // 0..3, power-source aware
            int percent = level <= 0 ? 0 : level * 100 / 3;
            return Gk6xProtocol.Scale(c.R, c.G, c.B, percent);
        }

        /// <summary>Returns true only if a lighting write was made.</summary>
        private static bool SyncOnce()
        {
            var candidates = Gk6xHidTransport.FindCandidates();
            if (candidates.Count == 0) return false;
            if (candidates.Count > 1)
            {
                Logger.WriteLine($"GK6X: {candidates.Count} matching collections found, refusing to guess which one to use");
                return false;
            }

            var transport = Gk6xHidTransport.TryOpen(candidates[0]);
            if (transport is null)
            {
                Logger.WriteLine("GK6X: could not open vendor collection");
                return false;
            }

            using var kb = new Gk6xKeyboard(transport, Logger.WriteLine);
            Gk6xIdentity id = kb.Identify();
            if (!kb.IsSupported) return false;

            var (r, g, b) = DesiredColour();
            string stamp = $"{id.ModelId}:{id.FirmwareId:X8}:{r},{g},{b}";
            if (AppConfig.GetString(LastWrittenKey) == stamp) return false;

            // Forget the old stamp first: if this write fails part-way the next request must rewrite.
            AppConfig.Set(LastWrittenKey, "");
            kb.SetStaticColour(r, g, b);
            AppConfig.Set(LastWrittenKey, stamp);
            Logger.WriteLine($"GK6X: static colour {r},{g},{b} written");
            return true;
        }
    }
}
