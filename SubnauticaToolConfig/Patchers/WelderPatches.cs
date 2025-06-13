using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(Welder), nameof(Welder.Weld))]
    internal class WelderPatches
    {
        [HarmonyPrefix]
        public static void Weld_Prefix(Welder __instance)
        {
            if (ModConfig.Instance.WelderUnlimitedEnergy)
                __instance.weldEnergyCost = 0f;
            else
                __instance.weldEnergyCost = ModConfig.Instance.WelderEnergyCost;

            if (ModConfig.Instance.WelderUnlimitedHealthPerWeld)
                __instance.healthPerWeld = 999999f;
            else
                __instance.healthPerWeld = ModConfig.Instance.WelderHealthPerWeld;

            Plugin.Logger.LogInfo($"Welder energy cost set to {__instance.weldEnergyCost}");
            Plugin.Logger.LogInfo($"Welder health per weld set to {__instance.healthPerWeld}");
        }
    }
}
