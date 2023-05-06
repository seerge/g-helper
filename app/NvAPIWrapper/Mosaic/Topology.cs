using System;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Interfaces.Mosaic;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Mosaic
{
    /// <summary>
    ///     Represents a mosaic topology
    /// </summary>
    [Obsolete("Using Mosaic API Phase 1, please consider using TopologyGrid class on newer drivers", false)]
    public class Topology : IEquatable<Topology>
    {
        /// <summary>
        ///     Creates a new Topology
        /// </summary>
        /// <param name="resolution">Mosaic displays resolution</param>
        /// <param name="frequency">Mosaic displays frequency</param>
        /// <param name="topology">Topology arrangement</param>
        /// <param name="overlap">Mosaic overlap</param>
        public Topology(
            Resolution resolution,
            int frequency,
            Native.Mosaic.Topology topology,
            Overlap overlap)
        {
            Resolution = resolution;
            Frequency = frequency;
            FrequencyInMillihertz = (uint) (Frequency * 1000);
            TopologyMode = topology;
            Overlap = overlap;
        }

        /// <summary>
        ///     Creates a new Topology
        /// </summary>
        /// <param name="resolution">>Mosaic displays resolution</param>
        /// <param name="frequency">Mosaic frequency</param>
        /// <param name="frequencyInMillihertz">Mosaic frequency x 1000</param>
        /// <param name="topology">Topology arrangement</param>
        /// <param name="overlap">Mosaic overlap</param>
        public Topology(
            Resolution resolution,
            int frequency,
            uint frequencyInMillihertz,
            Native.Mosaic.Topology topology,
            Overlap overlap)
            : this(resolution, frequency, topology, overlap)
        {
            FrequencyInMillihertz = frequencyInMillihertz;
        }

        /// <summary>
        ///     Gets the mosaic displays frequency
        /// </summary>
        public int Frequency { get; }

        /// <summary>
        ///     Gets the mosaic displays frequency x 1000 (Millihertz)
        /// </summary>
        public uint FrequencyInMillihertz { get; }

        /// <summary>
        ///     Gets the topology overlap
        /// </summary>
        public Overlap Overlap { get; }

        /// <summary>
        ///     Gets the mosaic displays resolution
        /// </summary>
        public Resolution Resolution { get; }

        /// <summary>
        ///     Gets the topology arrangement
        /// </summary>
        public Native.Mosaic.Topology TopologyMode { get; }

        /// <inheritdoc />
        public bool Equals(Topology other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Resolution.Equals(other.Resolution) &&
                   Frequency == other.Frequency &&
                   FrequencyInMillihertz == other.FrequencyInMillihertz &&
                   TopologyMode == other.TopologyMode &&
                   Overlap.Equals(other.Overlap);
        }

        /// <summary>
        ///     Disables the current topology
        /// </summary>
        public static void DisableCurrent()
        {
            MosaicApi.EnableCurrentTopology(false);
        }

        /// <summary>
        ///     Enables the current topology
        /// </summary>
        public static void EnableCurrent()
        {
            MosaicApi.EnableCurrentTopology(true);
        }

        /// <summary>
        ///     Returns the current topology settings
        /// </summary>
        /// <returns>The current Topology object</returns>
        public static Topology GetCurrentTopology()
        {
            TopologyBrief brief;
            IDisplaySettings displaySettings;
            int overlapX;
            int overlapY;
            MosaicApi.GetCurrentTopology(out brief, out displaySettings, out overlapX, out overlapY);

            return
                new Topology(
                    new Resolution(displaySettings.Width, displaySettings.Height, displaySettings.BitsPerPixel),
                    displaySettings.Frequency, displaySettings.FrequencyInMillihertz, brief.Topology,
                    new Overlap(overlapX, overlapY));
        }

        /// <summary>
        ///     Retrieves all the supported topology modes that are now possible to apply
        /// </summary>
        /// <param name="type">The type of the topology mode to limit quary</param>
        /// <returns>An array of Topology modes</returns>
        public static Native.Mosaic.Topology[] GetSupportedTopologyModes(TopologyType type = TopologyType.All)
        {
            return
                MosaicApi.GetSupportedTopologiesInfo(type)
                    .TopologyBriefs.Where(topologyBrief => topologyBrief.IsPossible)
                    .Select(topologyBrief => topologyBrief.Topology)
                    .ToArray();
        }


        /// <summary>
        ///     Retrieves all the supported display settings
        /// </summary>
        /// <param name="type">The type of the topology mode to limit quary</param>
        /// <returns>An array of IDisplaySettings implamented objects</returns>
        public static IDisplaySettings[] GetSupportedTopologySettings(TopologyType type = TopologyType.All)
        {
            return MosaicApi.GetSupportedTopologiesInfo(type).DisplaySettings.ToArray();
        }

        /// <summary>
        ///     Indicates if the current topology is now active
        /// </summary>
        /// <returns>true if the current topology is now enable, otherwise false</returns>
        public static bool IsCurrentTopologyEnabled()
        {
            TopologyBrief brief;
            IDisplaySettings displaySettings;
            int overlapX;
            int overlapY;
            MosaicApi.GetCurrentTopology(out brief, out displaySettings, out overlapX, out overlapY);

            return brief.IsEnable;
        }

        /// <summary>
        ///     Indicates if the current topology is possible to apply
        /// </summary>
        /// <returns>true if the current topology is possible to apply, otherwise false</returns>
        public static bool IsCurrentTopologyPossible()
        {
            TopologyBrief brief;
            IDisplaySettings displaySettings;
            int overlapX;
            int overlapY;
            MosaicApi.GetCurrentTopology(out brief, out displaySettings, out overlapX, out overlapY);

            return brief.IsPossible;
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(Topology left, Topology right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(Topology left, Topology right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Topology) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Resolution.GetHashCode();
                hashCode = (hashCode * 397) ^ Frequency;
                hashCode = (hashCode * 397) ^ (int) FrequencyInMillihertz;
                hashCode = (hashCode * 397) ^ (int) TopologyMode;
                hashCode = (hashCode * 397) ^ Overlap.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        ///     Retrieves topology details
        /// </summary>
        /// <returns>An array of TopologyDetails</returns>
        public TopologyDetails[] GetDetails()
        {
            var brief = GetTopologyBrief();

            return
                MosaicApi.GetTopologyGroup(brief)
                    .TopologyDetails.Select(detail => new TopologyDetails(detail))
                    .ToArray();
        }

        /// <summary>
        ///     Creates and fills a DisplaySettingsV1 object
        /// </summary>
        /// <returns>The newly created DisplaySettingsV1 object</returns>
        public DisplaySettingsV1 GetDisplaySettingsV1()
        {
            return new DisplaySettingsV1(Resolution.Width, Resolution.Height, Resolution.ColorDepth, Frequency);
        }

        /// <summary>
        ///     Creates and fills a DisplaySettingsV2 object
        /// </summary>
        /// <returns>The newly created DisplaySettingsV2 object</returns>
        public DisplaySettingsV2 GetDisplaySettingsV2()
        {
            return new DisplaySettingsV2(Resolution.Width, Resolution.Height, Resolution.ColorDepth, Frequency,
                FrequencyInMillihertz);
        }

        /// <summary>
        ///     Retrieve the topology overlap limits
        /// </summary>
        /// <returns></returns>
        public OverlapLimit GetOverlapLimits()
        {
            int minX;
            int maxX;
            int minY;
            int maxY;
            var brief = GetTopologyBrief();
            var displaySettingsV2 = GetDisplaySettingsV2();

            try
            {
                MosaicApi.GetOverlapLimits(brief, displaySettingsV2, out minX, out maxX, out minY, out maxY);

                return new OverlapLimit(minX, maxX, minY, maxY);
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var displaySettingsV1 = GetDisplaySettingsV1();
            MosaicApi.GetOverlapLimits(brief, displaySettingsV1, out minX, out maxX, out minY, out maxY);

            return new OverlapLimit(minX, maxX, minY, maxY);
        }

        /// <summary>
        ///     Creates and fills a TopologyBrief object
        /// </summary>
        /// <returns>The newly created TopologyBrief object</returns>
        public TopologyBrief GetTopologyBrief()
        {
            return new TopologyBrief(TopologyMode);
        }

        /// <summary>
        ///     Sets this topology as the current topology
        /// </summary>
        /// <param name="apply">if true, will apply the topology right now</param>
        public void SetAsCurrentTopology(bool apply = false)
        {
            var brief = GetTopologyBrief();
            var displaySettingsV2 = GetDisplaySettingsV2();

            try
            {
                MosaicApi.SetCurrentTopology(brief, displaySettingsV2, Overlap.HorizontalOverlap,
                    Overlap.VerticalOverlap, apply);
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            var displaySettingsV1 = GetDisplaySettingsV1();
            MosaicApi.SetCurrentTopology(brief, displaySettingsV1, Overlap.HorizontalOverlap, Overlap.VerticalOverlap,
                apply);
        }
    }
}