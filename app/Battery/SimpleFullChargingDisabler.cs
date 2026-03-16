namespace GHelper.Battery;

public class SimpleFullChargingDisabler : IFullChargingDisabler
{
    public void TriggerChargingEvent(decimal? chargingRate, decimal chargingPercent)
    {
        if (chargingPercent > 99 && BatteryControl.chargeFull)
        {
            BatteryControl.UnSetBatteryLimitFull();
        }
    }
}
