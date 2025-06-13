using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class LaserCutterPatches
    {
        [HarmonyPatch(nameof(LaserCutter.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("LaserCutter Prefix on Draw.");
            if (__instance.GetType() == typeof(LaserCutter))
            {
                LaserCutter LaserCutter = __instance as LaserCutter;
                Plugin.Logger.LogInfo($"LaserCutter Energy Cost Before: {LaserCutter.laserEnergyCost}");
                LaserCutter.laserEnergyCost = ModConfig.Instance.LaserCutterEnergyCost;
                Plugin.Logger.LogInfo($"LaserCutter Energy Cost After: {LaserCutter.laserEnergyCost}");

                if (ModConfig.Instance.LaserCutterUnlimitedEnergy)
                {
                    Plugin.Logger.LogInfo("LaserCutter Unlimited Energy Enabled.");
                    LaserCutter.laserEnergyCost = 0f;
                }
                else
                {
                    Plugin.Logger.LogInfo("LaserCutter Unlimited Energy Disabled.");
                }
            }
            return true;
        }
    }
}