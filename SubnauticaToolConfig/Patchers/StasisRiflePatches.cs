using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class StasisRiflePatches
    {
        [HarmonyPatch(nameof(PlayerTool.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("StasisRifle Prefix on Draw.");
            if (__instance.GetType() == typeof(StasisRifle))
            {
                StasisRifle stasisRifle = __instance as StasisRifle;
                Plugin.Logger.LogInfo($"StasisRifle Energy Cost Before: {stasisRifle.energyCost}");
                stasisRifle.energyCost = ModConfig.Instance.StasisRifleEnergyCost;
                Plugin.Logger.LogInfo($"StasisRifle Energy Cost After: {stasisRifle.energyCost}");
                if (ModConfig.Instance.StasisRifleUnlimitedEnergy)
                {
                    Plugin.Logger.LogInfo("StasisRifle Unlimited Energy Enabled.");
                    stasisRifle.energyCost = 0.00001f;
                }
                else
                {
                    Plugin.Logger.LogInfo("StasisRifle Unlimited Energy Disabled.");
                }
            }
            return true;
        }
    }
}