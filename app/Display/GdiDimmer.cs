using GHelper.Helpers;

namespace GHelper.Display
{
    /// <summary>
    /// Flicker-free dimmer using Win32 SetDeviceGammaRamp on the laptop
    /// panel only, with a pure-linear ramp matching what AsusSplendid's
    /// NV CSC matrix produced pre-591.
    ///
    /// Workaround for the NVIDIA driver 591.44+ regression where
    /// NvAPI_GPU_SetColorSpaceConversion (the call AsusSplendid uses for
    /// flicker-free dimming on the internal panel) returns
    /// NVAPI_NOT_SUPPORTED (-104) when the dGPU is driving the panel.
    /// See investigation_results.md in the project root.
    ///
    /// Caller is VisualControl.ApplyDimmerForDGpu which remaps slider
    /// 0..100 -> dimLevel 52..100 in dGPU mode, so the linear ramp always
    /// stays within the Windows ICM clamp's naturally-accepted window
    /// (gain >= ~0.50) and the slider responds at every position.
    ///
    /// The GDI ramp is applied at the GPU's LUT stage (downstream of DWM
    /// composition and the hardware cursor), so it dims the cursor, and
    /// only the laptop panel (other monitors keep their identity ramp).
    ///
    /// Known limitations:
    ///   * Windows ignores SetDeviceGammaRamp on displays in HDR mode.
    ///   * Linear-gain floor of ~0.52 (see slider remap in VisualControl).
    ///   * True fullscreen-exclusive D3D games call
    ///     IDXGIOutput::SetGammaControl(identity) on swapchain creation,
    ///     wiping our ramp.  Modern Win11 games use Fullscreen
    ///     Optimisations (DWM-composited borderless) and don't do this.
    ///     If you ever hit a true-FSE game and the dim disappears,
    ///     nudge the brightness slider once to re-apply.
    /// </summary>
    public static class GdiDimmer
    {
        /// <summary>
        /// AppConfig key (in %APPDATA%\GHelper\config.json) that enables this
        /// dGPU dimming workaround.  Default off so upstream behaviour is
        /// unchanged for users on NVIDIA drivers older than 591.44 where
        /// AsusSplendid's CSC path still works correctly.  Users on the
        /// broken driver branch (591.44+) enable by setting it to 1.
        /// </summary>
        public const string CFG_ENABLED = "oled_dgpu_dimmer_workaround";

        /// <summary>
        /// True when the user has opted in to the dGPU dimming workaround.
        /// When false, ApplyDimmerForDGpu is a no-op (and clears any
        /// previously-active dim).
        /// </summary>
        public static bool Enabled => AppConfig.Get(CFG_ENABLED, 0) != 0;

        private static readonly object stateLock = new();
        private static DisplayNative.GAMMA_RAMP currentRamp;
        private static string? targetDisplay;
        private static bool   active;

        public static bool IsActive
        {
            get { lock (stateLock) return active; }
        }

        /// <summary>
        /// Apply a dim level (0..100 where 100 = no dim, identity ramp).
        /// Caller is expected to feed a value in [52, 100] (VisualControl
        /// does this remap for dGPU mode); values below ~52 will be
        /// silently rejected by the Windows ICM clamp.
        /// </summary>
        public static bool Apply(int dimLevel)
        {
            int brightness = Math.Clamp(dimLevel, 0, 100);
            if (brightness >= 100)
            {
                Reset();
                return true;
            }

            lock (stateLock)
            {
                BuildRamp(ref currentRamp, brightness / 100.0);

                targetDisplay = ScreenNative.FindLaptopScreen();
                if (targetDisplay == null)
                {
                    Logger.WriteLine("GdiDimmer: no internal panel resolved, skip");
                    return false;
                }

                bool ok = ApplyToDisplay(targetDisplay, ref currentRamp);
                if (!ok)
                {
                    Logger.WriteLine($"GdiDimmer: SetDeviceGammaRamp failed on {targetDisplay} (HDR on?)");
                    return false;
                }
                active = true;
                Logger.WriteLine($"GdiDimmer: applied {brightness}% on {targetDisplay}");
                return true;
            }
        }

        /// <summary>
        /// Unconditionally restore identity gamma.  Always sends the
        /// identity ramp to the OS (doesn't short-circuit on the 'active'
        /// flag) so a stuck dim from a state-mismatch can still be cleared.
        /// </summary>
        public static void Reset()
        {
            lock (stateLock)
            {
                bool wasActive = active;
                active = false;

                BuildRamp(ref currentRamp, 1.0);
                var disp = targetDisplay ?? ScreenNative.FindLaptopScreen();
                if (disp != null)
                {
                    ApplyToDisplay(disp, ref currentRamp);
                    if (wasActive) Logger.WriteLine($"GdiDimmer: reset on {disp}");
                }
            }
        }

        /// <summary>Clear the ramp.  Call from Program.OnExit.</summary>
        public static void Shutdown()
        {
            try { Reset(); } catch { /* best effort */ }
        }

        private static bool ApplyToDisplay(string device, ref DisplayNative.GAMMA_RAMP r)
        {
            var hdc = DisplayNative.CreateDC("DISPLAY", device, null!, IntPtr.Zero);
            if (hdc == IntPtr.Zero) return false;
            try
            {
                return DisplayNative.SetDeviceGammaRamp(hdc, ref r);
            }
            finally
            {
                DisplayNative.DeleteDC(hdc);
            }
        }

        /// <summary>
        /// Builds a pure-linear per-channel ramp:
        ///     out[i] = i * brightness * 257
        /// (257 = 65535/255, so identity at brightness=1.0).  Same scale
        /// AsusSplendid's NV CSC matrix used pre-591; whites dim, no
        /// colour twist.  Cursor and fullscreen-exclusive games are
        /// reached because the LUT is at the GPU output stage.
        /// </summary>
        private static unsafe void BuildRamp(ref DisplayNative.GAMMA_RAMP r, double brightness)
        {
            for (int i = 0; i < 256; i++)
            {
                double y = i * brightness * 257.0;
                ushort w = (ushort)Math.Clamp(y, 0, 65535);
                r.table[      i] = w;
                r.table[256 + i] = w;
                r.table[512 + i] = w;
            }
        }
    }
}
