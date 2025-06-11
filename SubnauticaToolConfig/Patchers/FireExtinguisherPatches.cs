using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class FireExtinguisherPatches
    {
        [HarmonyPatch(nameof(PlayerTool.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("FireExtinguisher Prefix on Draw.");
            if (__instance.GetType() == typeof(FireExtinguisher))
            {
                FireExtinguisher FireExtinguisher = __instance as FireExtinguisher;

                Plugin.Logger.LogInfo($"FireExtinguisher Oxygen Capacity Before: {FireExtinguisher.maxFuel}");
                FireExtinguisher.maxFuel = ModConfig.Instance.FireExtinguisherMaxFuel;
                Plugin.Logger.LogInfo($"FireExtinguisher Oxygen Capacity After: {FireExtinguisher.maxFuel}");
            
                if (ModConfig.Instance.FireExtinguisherUnlimitedFuel)
                {
                    Plugin.Logger.LogInfo("FireExtinguisher Unlimited Fuel Enabled.");
                    FireExtinguisher.fuel = FireExtinguisher.maxFuel;
                }
                else
                {
                    Plugin.Logger.LogInfo("FireExtinguisher Unlimited Fuel Disabled.");
                }

            }
            return true;
        }
    }
}