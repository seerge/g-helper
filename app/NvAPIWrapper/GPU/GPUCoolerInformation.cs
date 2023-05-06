using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the GPU coolers and current fan speed
    /// </summary>
    public class GPUCoolerInformation
    {
        internal GPUCoolerInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;

            // TODO: Add Support For Pascal Only Policy Table Method
            // TODO: GPUApi.GetCoolerPolicyTable & GPUApi.SetCoolerPolicyTable & GPUApi.RestoreCoolerPolicyTable
            // TODO: Better support of ClientFanCoolers set of APIs
        }

        /// <summary>
        ///     Gets a list of all available coolers along with their current settings and status
        /// </summary>
        public IEnumerable<GPUCooler> Coolers
        {
            get
            {
                PrivateCoolerSettingsV1? settings = null;

                try
                {
                    settings = GPUApi.GetCoolerSettings(PhysicalGPU.Handle);
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status != Status.NotSupported)
                    {
                        throw;
                    }
                }

                if (settings != null)
                {
                    for (var i = 0; i < settings.Value.CoolerSettings.Length; i++)
                    {
                        if (i == 0)
                        {
                            var currentRPM = -1;
                            try
                            {
                                currentRPM = (int)GPUApi.GetTachReading(PhysicalGPU.Handle);
                            }
                            catch (NVIDIAApiException)
                            {
                                // ignored
                            }

                            if (currentRPM >= 0)
                            {
                                yield return new GPUCooler(
                                    i,
                                    settings.Value.CoolerSettings[i],
                                    currentRPM
                                );
                                continue;
                            }
                        }

                        yield return new GPUCooler(
                            i,
                            settings.Value.CoolerSettings[i]
                        );
                    }

                    yield break;
                }

                PrivateFanCoolersStatusV1? status = null;
                PrivateFanCoolersInfoV1? info = null;
                PrivateFanCoolersControlV1? control = null;

                try
                {
                    status = GPUApi.GetClientFanCoolersStatus(PhysicalGPU.Handle);
                    info = GPUApi.GetClientFanCoolersInfo(PhysicalGPU.Handle);
                    control = GPUApi.GetClientFanCoolersControl(PhysicalGPU.Handle);
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status != Status.NotSupported)
                    {
                        throw;
                    }
                }

                if (status != null && info != null && control != null)
                {
                    for (var i = 0; i < status.Value.FanCoolersStatusEntries.Length; i++)
                    {
                        if (info.Value.FanCoolersInfoEntries.Length > i &&
                            control.Value.FanCoolersControlEntries.Length > i)
                        {
                            yield return new GPUCooler(
                                info.Value.FanCoolersInfoEntries[i],
                                status.Value.FanCoolersStatusEntries[i],
                                control.Value.FanCoolersControlEntries[i]
                            );
                        }
                    }

                    yield break;
                }

                throw new NVIDIAApiException(Status.NotSupported);
            }
        }

        /// <summary>
        ///     Gets the GPU fan speed in revolutions per minute
        /// </summary>
        public int CurrentFanSpeedInRPM
        {
            get
            {
                try
                {
                    return (int) GPUApi.GetTachReading(PhysicalGPU.Handle);
                }
                catch
                {
                    return Coolers.FirstOrDefault(cooler => cooler.Target == CoolerTarget.All)?.CurrentFanSpeedInRPM ??
                           0;
                }
            }
        }

        /// <summary>
        ///     Gets the current fan speed in percentage if available
        /// </summary>
        public int CurrentFanSpeedLevel
        {
            get
            {
                try
                {
                    return (int) GPUApi.GetCurrentFanSpeedLevel(PhysicalGPU.Handle);
                }
                catch
                {
                    return Coolers.FirstOrDefault(cooler => cooler.Target == CoolerTarget.All)?.CurrentLevel ?? 0;
                }
            }
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{CurrentFanSpeedInRPM} RPM ({CurrentFanSpeedLevel}%)";
        }

        /// <summary>
        ///     Resets all cooler settings to default.
        /// </summary>
        public void RestoreCoolerSettingsToDefault()
        {
            RestoreCoolerSettingsToDefault(Coolers.Select(cooler => cooler.CoolerId).ToArray());
        }

        /// <summary>
        ///     Resets one or more cooler settings to default.
        /// </summary>
        /// <param name="coolerIds">The cooler identification numbers (indexes) to reset their settings to default.</param>
        public void RestoreCoolerSettingsToDefault(params int[] coolerIds)
        {
            var availableCoolerIds = Coolers.Select(cooler => cooler.CoolerId).ToArray();

            if (coolerIds.Any(i => !availableCoolerIds.Contains(i)))
            {
                throw new ArgumentException("Invalid cooler identification number provided.", nameof(coolerIds));
            }

            try
            {
                GPUApi.RestoreCoolerSettings(PhysicalGPU.Handle, coolerIds.Select(i => (uint) i).ToArray());

                return;
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status != Status.NotSupported)
                {
                    throw;
                }
            }

            var currentControl = GPUApi.GetClientFanCoolersControl(PhysicalGPU.Handle);
            var newControl = new PrivateFanCoolersControlV1(
                currentControl.FanCoolersControlEntries.Select(
                        entry => coolerIds.Contains((int) entry.CoolerId)
                            ? new PrivateFanCoolersControlV1.FanCoolersControlEntry(
                                entry.CoolerId,
                                FanCoolersControlMode.Auto
                            )
                            : entry
                    )
                    .ToArray(),
                currentControl.UnknownUInt
            );
            GPUApi.SetClientFanCoolersControl(PhysicalGPU.Handle, newControl);
        }

        /// <summary>
        ///     Changes a cooler settings by modifying the policy and the current level
        /// </summary>
        /// <param name="coolerId">The cooler identification number (index) to change the settings.</param>
        /// <param name="policy">The new cooler policy.</param>
        /// <param name="newLevel">The new cooler level. Valid only if policy is set to manual.</param>
        // ReSharper disable once TooManyDeclarations
        public void SetCoolerSettings(int coolerId, CoolerPolicy policy, int newLevel)
        {
            if (Coolers.All(cooler => cooler.CoolerId != coolerId))
            {
                throw new ArgumentException("Invalid cooler identification number provided.", nameof(coolerId));
            }

            try
            {
                GPUApi.SetCoolerLevels(
                    PhysicalGPU.Handle,
                    (uint) coolerId,
                    new PrivateCoolerLevelsV1(new[]
                        {
                            new PrivateCoolerLevelsV1.CoolerLevel(policy, (uint) newLevel)
                        }
                    ),
                    1
                );

                return;
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status != Status.NotSupported)
                {
                    throw;
                }
            }

            var currentControl = GPUApi.GetClientFanCoolersControl(PhysicalGPU.Handle);
            var newControl = new PrivateFanCoolersControlV1(
                currentControl.FanCoolersControlEntries.Select(
                        entry => entry.CoolerId == coolerId
                            ? new PrivateFanCoolersControlV1.FanCoolersControlEntry(
                                entry.CoolerId,
                                policy == CoolerPolicy.Manual
                                    ? FanCoolersControlMode.Manual
                                    : FanCoolersControlMode.Auto,
                                policy == CoolerPolicy.Manual ? (uint)newLevel : 0u)
                            : entry
                    )
                    .ToArray(),
                currentControl.UnknownUInt
            );
            GPUApi.SetClientFanCoolersControl(PhysicalGPU.Handle, newControl);
        }

        /// <summary>
        ///     Changes a cooler setting by modifying the policy
        /// </summary>
        /// <param name="coolerId">The cooler identification number (index) to change the settings.</param>
        /// <param name="policy">The new cooler policy.</param>
        // ReSharper disable once TooManyDeclarations
        public void SetCoolerSettings(int coolerId, CoolerPolicy policy)
        {
            if (Coolers.All(cooler => cooler.CoolerId != coolerId))
            {
                throw new ArgumentException("Invalid cooler identification number provided.", nameof(coolerId));
            }

            try
            {
                GPUApi.SetCoolerLevels(
                    PhysicalGPU.Handle,
                    (uint) coolerId,
                    new PrivateCoolerLevelsV1(new[]
                        {
                            new PrivateCoolerLevelsV1.CoolerLevel(policy)
                        }
                    ),
                    1
                );

                return;
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status != Status.NotSupported)
                {
                    throw;
                }
            }

            var currentControl = GPUApi.GetClientFanCoolersControl(PhysicalGPU.Handle);
            var newControl = new PrivateFanCoolersControlV1(
                currentControl.FanCoolersControlEntries.Select(
                        entry => entry.CoolerId == coolerId
                            ? new PrivateFanCoolersControlV1.FanCoolersControlEntry(
                                entry.CoolerId,
                                policy == CoolerPolicy.Manual
                                    ? FanCoolersControlMode.Manual
                                    : FanCoolersControlMode.Auto)
                            : entry
                    )
                    .ToArray(),
                currentControl.UnknownUInt
            );
            GPUApi.SetClientFanCoolersControl(PhysicalGPU.Handle, newControl);
        }

        /// <summary>
        ///     Changes a cooler settings by modifying the policy to manual and sets a new level
        /// </summary>
        /// <param name="coolerId">The cooler identification number (index) to change the settings.</param>
        /// <param name="newLevel">The new cooler level.</param>
        public void SetCoolerSettings(int coolerId, int newLevel)
        {
            SetCoolerSettings(coolerId, CoolerPolicy.Manual, newLevel);
        }
    }
}