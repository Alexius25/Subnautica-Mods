using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class HeatBladePatches
    {
        [HarmonyPatch(nameof(HeatBlade.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("HeatKnife Prefix on Draw.");
            if (__instance.GetType() == typeof(HeatBlade))
            {
                HeatBlade HeatBlade = __instance as HeatBlade;

                Plugin.Logger.LogInfo($"HeatBlade Damage Before: {HeatBlade.damage}");

                // Check if One Hit Mode is enabled
                if (ModConfig.Instance.HeatBladeOneHit == true)
                {
                    HeatBlade.damage = 999999;
                    Plugin.Logger.LogInfo($"Heat Blade One Hit Mode Enabled. Damage After: {HeatBlade.damage}");
                }
                else
                {
                    HeatBlade.damage = ModConfig.Instance.HeatBladeDamge;
                    Plugin.Logger.LogInfo($"Heat Blade One Hit Mode Disabled. Damage After: {HeatBlade.damage}");
                }

                Plugin.Logger.LogInfo($"HeatBlade Range Before: {HeatBlade.attackDist}");
                HeatBlade.attackDist = ModConfig.Instance.HeatBladeAttackDist;
                Plugin.Logger.LogInfo($"HeatBlade Range After: {HeatBlade.attackDist}");
            }
            return true;
        }
    }
}