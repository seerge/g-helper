using GHelper.Fan;
using GHelper.Mode;
using System.Diagnostics;
using System.Management;
using GHelper.USB;
using System.ComponentModel;
using GHelper.Display;
using System.Text;

namespace GHelper.CompanionApp
{
    abstract class CompanionService
    {

        protected const int GPU_MODE_OPTIMIZED = 3;

        private PerformanceCounter cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        // private PerformanceCounter gpuUsageCounter = new PerformanceCounter("GPU Engine", "Utilization Percentage", "*");
        // private PerformanceCounter ramUsageCounter = new PerformanceCounter("Memory", "Available MBytes");

        private ManagementObjectSearcher wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

        // Match with Mobile TYPE_.. mode
        protected enum Type
        {
            INFO, // = 0
            MODES,// = 1
            SENSOR,// = 2
            CMD, // = 3
        }
        protected enum Cmd
        {
            Sensors, // 0
            Mode, // Silent, Balanced, Turbo
            GpuMode,
            AuraKeyboardMode,
            ScreenMode,
        }

        // Seconds
        /*   public enum UpdateTick
           {
               Normal = 2000, 
               Fast = 1000
           }*/

        //  public UpdateTick Tick { get;  set; } = UpdateTick.Normal;
        /// <summary>
        ///   Status Event handlers. Any UI form can get this if needs update
        /// </summary>
        public enum EStatus
        {
            Started,
            Stopped
        }
        public event EventHandler<StatusEventArgs>? StatusChanged = null;
        public class StatusEventArgs : EventArgs
        {
            public EStatus Status { get; }
            public Exception? Exception { get; }
            public StatusEventArgs(EStatus status)
            {
                Status = status;
            }
            public StatusEventArgs(EStatus status, Exception? exception)
            {
                Status = status;
                Exception = exception;
            }
        }

        private AsyncOperation MainThreadOperation;

        public EStatus Status { get; private set; } = EStatus.Stopped;
        protected void SetStatusChanged(EStatus status, Exception? ex = null)
        {
            // Make sure state is posted in main thread
            MainThreadOperation.Post((state) =>
            {
                Status = status;
                // For any UI form that registers Server.StatusChanged += ...
                StatusChanged?.Invoke(this, new StatusEventArgs(status, ex));
            }, null);
        }


        public CompanionService()
        {
            MainThreadOperation = AsyncOperationManager.CreateOperation(this);
        }
        public abstract void Start();

        public virtual void Stop()
        {
            Status = EStatus.Stopped;
            SetStatusChanged(Status);
        }





        protected void OnRead(byte[] data)
        {
            Type Type = (Type)data[0];
            if (Type == Type.CMD) // TYPE_CMD
            {
                Cmd _Cmd = (Cmd)data[1];
                switch (_Cmd)
                {
                    case Cmd.Mode:
                        {
                            int mode = (int)data[2];
                            // check if mode between 0..2
                            if (mode >= AsusACPI.PerformanceBalanced && mode <= AsusACPI.PerformanceSilent)
                            {
                                // TODO: this should be decoupled from WinForm
                                Program.modeControl.SetPerformanceMode(mode);
                            }
                            break;
                        }
                    case Cmd.GpuMode:
                        {
                            int mode = (int)data[2];//GetIntFromByte(data, 8);

                            // TODO: this should be decoupled from WinForm
                            Program.settingsForm.Invoke(() =>
                            {
                                switch (mode)
                                {
                                    case AsusACPI.GPUModeUltimate:
                                        Program.settingsForm.ButtonUltimate_Click(null, EventArgs.Empty);
                                        break;

                                    case AsusACPI.GPUModeEco:
                                        Program.settingsForm.ButtonEco_Click(null, EventArgs.Empty);
                                        break;

                                    case AsusACPI.GPUModeStandard:
                                        Program.settingsForm.ButtonStandard_Click(null, EventArgs.Empty);
                                        break;

                                    case GPU_MODE_OPTIMIZED:
                                        Program.settingsForm.ButtonOptimized_Click(null, EventArgs.Empty);
                                        break;

                                }
                            });
                            break;
                        }
                    //case Cmd.AuraKeyboardMode:
                    //    {
                    //        int mode = data[2];
                    //        Program.settingsForm.BeginInvoke(() =>
                    //        {
                    //            AppConfig.Set("aura_mode", mode);
                    //            Program.settingsForm.SetAura();
                    //        });
                    //    }
                    //    break;

                    //case Cmd.ScreenMode:
                    //    {
                    //        int mode = data[2];
                    //        Program.settingsForm.BeginInvoke(() =>
                    //        {
                    //            switch (mode)
                    //            {
                    //                case 0:
                    //                    Program.settingsForm.Button60Hz_Click(null, EventArgs.Empty);
                    //                    break;

                    //                case 1:
                    //                    Program.settingsForm.Button120Hz_Click(null, EventArgs.Empty);
                    //                    break;

                    //                case 2:
                    //                    Program.settingsForm.ButtonMiniled_Click(null, EventArgs.Empty);
                    //                    break;

                    //                case 3:
                    //                    Program.settingsForm.ButtonScreenAuto_Click(null, EventArgs.Empty);
                    //                    break;
                    //            }

                    //        });
                    //    }
                    //    break;


                    default: { break; }
                }
            }

        }

        /// <summary>
        /// Helper Functions
        /// </summary>
        /// <returns></returns>

        protected byte[] PrepareInfoBuffer()
        {

            string model = AppConfig.GetModelShort();
            byte[] asciiBytes = Encoding.ASCII.GetBytes(model);

            byte[] buffer = new byte[1 + asciiBytes.Length];
            buffer[0] = (byte)model.Length;
            Array.Copy(asciiBytes, 0, buffer, 1, asciiBytes.Length);

            return buffer;

        }

        protected byte[] PrepareModesBuffer()
        {

            byte performanceMode = (byte)Modes.GetCurrent();
            byte gpuMode = (byte)(AppConfig.Is("gpu_auto") ? GPU_MODE_OPTIMIZED : AppConfig.Get("gpu_mode"));

            //byte kbLightMode = (byte)AppConfig.Get("aura_mode");


            //var kbLightModes = Aura.GetModes().Keys.ToList();

            //// cannot reach 249 so write as byte
            //// tell length of modes (is dynamic)

            //int minRate = ScreenControl.MIN_RATE;
            //int maxRate = ScreenControl.MAX_RATE;

            //string? laptopScreen = null;
            //int frequency = 0;
            //int maxFrequency = 0;

            //bool screenAuto = false;
            //bool overdriveSetting = false;
            //int overdrive = 0;
            //int miniled1 = 0;
            //int miniled2 = 0;

            //int miniled = 0;
            //bool hdr = false;


            //bool screenEnabled = false;
            //int fhd = -1;
            //int hdrControl = -1;

            //
            // ScreenControl.InitScreen variables have been moved into a new func .InitScreenVariables 
            // in order to access them here
            //
            //ScreenControl.InitScreenVariables(
            //    ref laptopScreen,
            //    ref frequency,
            //    ref maxFrequency,
            //    ref screenAuto,
            //    ref overdriveSetting,
            //    ref overdrive,
            //    ref miniled1,
            //    ref miniled2,
            //    ref miniled,
            //    ref hdr,
            //    ref screenEnabled,
            //    ref fhd,
            //    ref hdrControl);

            byte[] buffer = [performanceMode, gpuMode];
            return buffer;

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (var writer = new BinaryWriter(ms))
            //    {
            //writer.Write((byte)(screenEnabled ? 1 : 0));
            //writer.Write((byte)(screenAuto ? 1 : 0));
            //writer.Write((byte)(overdriveSetting ? 1 : 0));
            //writer.Write((byte)(hdr ? 1 : 0));
            //writer.Write((byte)(hdrControl < 0 ? 0 : hdrControl + 1));
            //writer.Write((byte)(fhd < 0 ? 0 : fhd + 1)); // in case -1, set 0 (ubyte)
            //writer.Write((byte)(miniled1 < 0 ? 0 : miniled1 + 1));
            //writer.Write((byte)(miniled2 < 0 ? 0 : miniled2 + 1));
            //writer.Write(minRate);
            //writer.Write(maxRate);
            //writer.Write(frequency);
            //writer.Write(maxFrequency);

            //writer.Write(kbLightMode);
            //// cannot reach 249 so write as byte
            //// tell length of modes (is dynamic)
            //writer.Write((byte)kbLightModes.Count);

            //foreach (var mode in kbLightModes)
            //    writer.Write((byte)mode);


            //         return ms.GetBuffer();
            //     }
            // }
        }

        protected byte[] PrepareSensorBuffer()
        {
            // 
            // Values do not exceed 254. we use single byte for each of them. 
            // we can pack to 10 bytes in total
            // 


            byte cpuUsage = (byte)GetUsageCounterFor(ref cpuUsageCounter);


            //Debug.WriteLine($"GPU USAGE: {gpuUsage}");

            byte gpuUsage = (byte)HardwareControl.GetGpuUse();

            //float gpuUsage = -1f;// GetUsageCounterFor(ref gpuUsageCounter);
            //float ramUsage = GetUsageCounterFor(ref ramUsageCounter);

            var stats = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
            {
                FreeMemory = int.Parse(mo["FreePhysicalMemory"].ToString()),
                Memory = int.Parse(mo["TotalVisibleMemorySize"].ToString()),

            }).FirstOrDefault(new { FreeMemory = -1, Memory = -1 });


            byte cpuFan = (byte)Program.acpi.GetFan(AsusFan.CPU);
            byte gpuFan = (byte)Program.acpi.GetFan(AsusFan.GPU);
            byte midFan = (byte)Program.acpi.GetFan(AsusFan.Mid);

            byte maxCpuFan = (byte)FanSensorControl.GetFanMax(AsusFan.CPU);
            byte maxGpuFan = (byte)FanSensorControl.GetFanMax(AsusFan.GPU);
            byte maxMidFan = (byte)FanSensorControl.GetFanMax(AsusFan.Mid);

            byte cpuTemp = (byte)((int?)HardwareControl.GetCPUTemp() ?? 0);
            byte gpuTemp = (byte)((int?)HardwareControl.GetGPUTemp() ?? 0);


            byte[] buffer = new byte[18];
            buffer[0] = cpuFan;
            buffer[1] = gpuFan;
            buffer[2] = midFan;
            buffer[3] = maxCpuFan;
            buffer[4] = maxGpuFan;
            buffer[5] = maxMidFan;
            buffer[6] = cpuTemp;
            buffer[7] = gpuTemp;
            buffer[8] = cpuUsage;
            buffer[9] = gpuUsage;
            Array.Copy(BitConverter.GetBytes(stats.Memory), 0, buffer, 10, 4);
            Array.Copy(BitConverter.GetBytes(stats.FreeMemory), 0, buffer, 14, 4);

            return buffer;

        }

        private float GetUsageCounterFor(ref PerformanceCounter counter)
        {
            try
            {
                return counter.NextValue();
            }
            // Admin privileges??
            catch (UnauthorizedAccessException ex) { }
            //An error occurred when accessing a system API.
            catch (System.ComponentModel.Win32Exception ex) { }
            // The instance is not correctly associated with a performance counter.
            catch (System.InvalidOperationException ex) { }

            return -1f;
        }

    }


    class EmptyService : CompanionService
    {
        public override void Start()
        {
        }
    }
}
