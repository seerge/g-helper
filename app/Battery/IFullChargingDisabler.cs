namespace GHelper.Battery;

public interface IFullChargingDisabler
{
    public void TriggerChargingEvent(decimal? chargingRate, decimal chargingPercent);
}
