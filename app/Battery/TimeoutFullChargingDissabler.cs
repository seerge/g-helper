using System.Diagnostics;
using Microsoft.VisualBasic.Logging;

namespace GHelper.Battery;

public class TimeoutFullChargingDisabler : IFullChargingDisabler
{
    private decimal lastChargingRate = 1;
    private DateTime lastChargingModeSwitch;
    
    public TimeoutFullChargingDisabler() {}
    
    public void TriggerChargingEvent(decimal? chargingRate, decimal chargingPercent)
    {
        if (chargingPercent >= 100)
        {
            SetBatteryLimitFullSafe();
        }

        if (lastChargingRate * (chargingRate ?? 0) <= 0)
        {
            lastChargingModeSwitch = DateTime.Now;
        }
        lastChargingRate = chargingRate ?? 0;
        
        if (chargingRate < 0 && DateTime.Now.Subtract(lastChargingModeSwitch).TotalMinutes > 1) {
            SetBatteryLimitFullSafe();
        }
    }

    private void SetBatteryLimitFullSafe()
    {
        if (BatteryControl.chargeFull) BatteryControl.UnSetBatteryLimitFull();
    }
}
