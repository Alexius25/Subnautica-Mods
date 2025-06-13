using HarmonyLib;
using SubnauticaToolConfig.Settings;
using UnityEngine;

namespace SubnauticaToolConfig.Patchers
{
    [HarmonyPatch(typeof(StasisSphere), nameof(StasisSphere.Shoot))]
    [HarmonyPatch(new[] {typeof(Vector3), typeof(Quaternion), typeof(float), typeof(float), typeof(float)})]
    internal class StasisSpherePatches
    {
        [HarmonyPrefix]
        public static void OnShoot_Prefix(StasisSphere __instance)
        {
            Plugin.Logger.LogInfo("StasisSphere Prefix on Shoot called!");

            Plugin.Logger.LogInfo($"StasisSphere Max Radius Before: {__instance.maxRadius}");
            __instance.maxRadius = ModConfig.Instance.StasisSphereMaxRadius;
            Plugin.Logger.LogInfo($"StasisSphere Max Radius After: {__instance.maxRadius}");

            Plugin.Logger.LogInfo($"StasisSphere Min Radius Before: {__instance.minRadius}");
            __instance.minRadius = ModConfig.Instance.StasisSphereMinRadius;
            Plugin.Logger.LogInfo($"StasisSphere Min Radius After: {__instance.minRadius}");

            Plugin.Logger.LogInfo($"StasisSphere Max Time Before: {__instance.maxTime}");
            __instance.maxTime = ModConfig.Instance.StasisSphereMaxTime;
            Plugin.Logger.LogInfo($"StasisSphere Max Time After: {__instance.maxTime}");

            Plugin.Logger.LogInfo($"StasisSphere Min Time Before: {__instance.minTime}");
            __instance.minTime = ModConfig.Instance.StasisSphereMinTime;
            Plugin.Logger.LogInfo($"StasisSphere Min Time After: {__instance.minTime}");
        }
    }
}
