namespace GHelper.AutoTDP.PowerLimiter
{
    internal interface IPowerLimiter : IDisposable
    {
        public void SetCPUPowerLimit(double watts);

        public int GetCPUPowerLimit();

        public void ResetPowerLimits();

    }
}
