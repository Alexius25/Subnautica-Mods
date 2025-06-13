using HarmonyLib;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(PlayerTool))]

    internal class FlarePatches
    {
        [HarmonyPatch(nameof(Flare.OnDraw))]
        [HarmonyPrefix]
        public static bool OnDraw_Prefix(PlayerTool __instance)
        {
            Plugin.Logger.LogInfo("Flare Prefix on Draw.");
            if (__instance.GetType() == typeof(Flare))
            {
                Flare Flare = __instance as Flare;

                //Plugin.Logger.LogInfo($"Flare Energy Before: {Flare.energyLeft}");
                //Flare.energyLeft = ModConfig.Instance.FlareMaxEnergy;
                //Plugin.Logger.LogInfo($"Flare Energy After: {Flare.energyLeft}");
                
                Plugin.Logger.LogInfo($"Flare Throw Force Before: {Flare.throwForceAmount}");
                Flare.throwForceAmount = ModConfig.Instance.FlareThrowForce;
                Plugin.Logger.LogInfo($"Flare Throw Force After: {Flare.throwForceAmount}");
            }
            return true;
        }
    }
}