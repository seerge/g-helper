namespace GHelper.Battery;

public class FullChargingDisablerFactory
{
    public static IFullChargingDisabler CreateUsingConfig()
    {
        return AppConfig.Is("full_charging_timeout_disabling_strategy")
            ? new TimeoutFullChargingDisabler()
            : new SimpleFullChargingDisabler();
    }
}
