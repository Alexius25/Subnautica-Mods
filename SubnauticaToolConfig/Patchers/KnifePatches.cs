using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class KnifePatches
    {
        [HarmonyPatch(nameof(Knife.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("Knife Prefix on Draw.");
            if (__instance.GetType() == typeof(Knife))
            {
                Knife knife = __instance as Knife;

                Plugin.Logger.LogInfo($"Knife Damage Before: {knife.damage}");
                
                if (ModConfig.Instance.KnifeOneHit == true)
                {
                    knife.damage = 999999;
                    Plugin.Logger.LogInfo($"Knife One Hit Mode Enabled. Damage After: {knife.damage}");
                }
                else 
                {
                    knife.damage = ModConfig.Instance.KnifeDamge;
                    Plugin.Logger.LogInfo($"Knife One Hit Mode Disabled. Damage After: {knife.damage}");
                }

                Plugin.Logger.LogInfo($"Knife Range Before: {knife.attackDist}");
                knife.attackDist = ModConfig.Instance.KnifeAttackDist;
                Plugin.Logger.LogInfo($"Knife Range After: {knife.attackDist}");
            }
            return true;
        }
    }
}