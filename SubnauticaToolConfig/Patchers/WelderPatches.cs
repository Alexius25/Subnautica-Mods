using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class WelderPatches
    {
        [HarmonyPatch(nameof(PlayerTool.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("Welder Prefix on Draw.");
            if (__instance.GetType() == typeof(Welder))
            {
                Welder Welder = __instance as Welder;

                if (ModConfig.Instance.WelderUnlimitedEnergy == true)
                    {
                    Welder.weldEnergyCost = 0f;
                    Plugin.Logger.LogInfo($"Welder Unlimited Energy Enabled. Energy Cost After: {Welder.weldEnergyCost}");
                }
                else
                {
                    Plugin.Logger.LogInfo($"Welder Unlimited Energy Disabled. Energy Cost Before: {Welder.weldEnergyCost}");
                    Welder.weldEnergyCost = ModConfig.Instance.WelderEnergyCost;
                    Plugin.Logger.LogInfo($"Welder Energy Cost After: {Welder.weldEnergyCost}");
                }

                if (ModConfig.Instance.WelderUnlimitedHealthPerWeld == true)
                    {
                    Welder.healthPerWeld = 999999f;
                    Plugin.Logger.LogInfo($"Welder Unlimited Health per Weld Enabled. Health per Weld After: {Welder.healthPerWeld}");
                }
                else
                {
                    Plugin.Logger.LogInfo($"Welder Unlimited Health per Weld Disabled. Health per Weld Before: {Welder.healthPerWeld}");
                    Welder.healthPerWeld = ModConfig.Instance.WelderHealthPerWeld;
                    Plugin.Logger.LogInfo($"Welder Health per Weld After: {Welder.healthPerWeld}");
                }
            }
            return true;
        }
    }
}