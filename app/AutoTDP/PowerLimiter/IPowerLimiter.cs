namespace GHelper.AutoTDP.PowerLimiter
{
    internal interface IPowerLimiter : IDisposable
    {
        public void SetCPUPowerLimit(int watts);

        public int GetCPUPowerLimit();

        public void ResetPowerLimits();

    }
}
