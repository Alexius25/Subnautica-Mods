using HarmonyLib;
using SubnauticaToolConfig;
using SubnauticaToolConfig.Settings;

[HarmonyPatch(typeof(PlayerController))]
public class PlayerControllerPatches
{
    [HarmonyPatch(nameof(PlayerController.SetMotorMode))]
    [HarmonyPrefix]
    public static void SetMotorMode_Prefix(PlayerController __instance, Player.MotorMode newMotorMode)
    {
        if (newMotorMode != Player.MotorMode.Seaglide) return;

        float speedMultiplier = ModConfig.Instance.SeaglideSpeedMultiplier;

        if (speedMultiplier <= 0f) return;

        __instance.seaglideForwardMaxSpeed *= speedMultiplier;
        __instance.seaglideBackwardMaxSpeed *= speedMultiplier;
        __instance.seaglideStrafeMaxSpeed *= speedMultiplier;
        __instance.seaglideVerticalMaxSpeed *= speedMultiplier;

        Plugin.Logger.LogInfo($"seaglideForwardMaxSpeed: {__instance.seaglideForwardMaxSpeed}");
        Plugin.Logger.LogInfo($"seaglideBackwardMaxSpeed: {__instance.seaglideBackwardMaxSpeed}");
        Plugin.Logger.LogInfo($"seaglideStrafeMaxSpeed: {__instance.seaglideStrafeMaxSpeed}");
        Plugin.Logger.LogInfo($"seaglideVerticalMaxSpeed: {__instance.seaglideVerticalMaxSpeed}");

        Plugin.Logger.LogInfo($"seaglideWaterAcceleration Before: {__instance.seaglideWaterAcceleration}");
        __instance.seaglideWaterAcceleration = 36.56f * speedMultiplier;
        Plugin.Logger.LogInfo($"seaglideWaterAcceleration After: {__instance.seaglideWaterAcceleration}");
    }
}
