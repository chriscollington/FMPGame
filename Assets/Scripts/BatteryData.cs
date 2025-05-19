public static class BatteryData
{
    public static float currentBatteryHealth = -1f; // -1 means not initialized

    public static void ResetBattery()
    {
        currentBatteryHealth = -1f; // This will cause FlashLight.cs to reinitialize to max
    }
}