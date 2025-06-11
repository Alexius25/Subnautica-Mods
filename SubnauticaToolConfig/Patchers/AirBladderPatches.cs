using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class AirBladderPatches
    {
        [HarmonyPatch(nameof(PlayerTool.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("AirBladder Prefix on Draw.");
            if (__instance.GetType() == typeof(AirBladder))
            {
                AirBladder AirBladder = __instance as AirBladder;

                Plugin.Logger.LogInfo($"AirBladder Oxygen Capacity Before: {AirBladder.oxygenCapacity}");
                AirBladder.oxygenCapacity = ModConfig.Instance.AirBladderOxygenCapacity;
                Plugin.Logger.LogInfo($"AirBladder Oxygen Capacity After: {AirBladder.oxygenCapacity}");

                Plugin.Logger.LogInfo($"AirBladder Buoyancy Force Before: {AirBladder.buoyancyForce}");
                AirBladder.buoyancyForce = ModConfig.Instance.AirBladderBuoyancyForce;
                Plugin.Logger.LogInfo($"AirBladder Buoyancy Force After: {AirBladder.buoyancyForce}");
            }
            return true;
        }
    }
}