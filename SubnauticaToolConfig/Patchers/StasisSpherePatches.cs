using HarmonyLib;
using SubnauticaToolConfig.Settings;

//namespace SubnauticaToolConfig.Patchers
//{
//    [HarmonyPatch(typeof(PlayerTool))]
//    internal class StasisSpherePatches
//    {
//        [HarmonyPatch(nameof(Bullet.Shoot))]
//        [HarmonyPrefix]
//        public static bool OnShoot_Prefix(Bullet __instance)
//        {
//            Plugin.Logger.LogInfo("StasisSphere Prefix on Draw.");
//            if (__instance.GetType() == typeof(StasisSphere))
//            {
//                StasisSphere stasisSphere = __instance as StasisSphere;
//                Plugin.Logger.LogInfo($"StasisSphere Max Radius Before: {stasisSphere.maxRadius}");
//                stasisSphere.maxRadius = ModConfig.Instance.StasisSphereMaxRadius;
//                Plugin.Logger.LogInfo($"StasisSphere Max Radius After: {stasisSphere.maxRadius}");
//
//                Plugin.Logger.LogInfo($"StasisSphere Min Radius Before: {stasisSphere.minRadius}");
//                stasisSphere.minRadius = ModConfig.Instance.StasisSphereMinRadius;
//                Plugin.Logger.LogInfo($"StasisSphere Min Radius After: {stasisSphere.minRadius}");
//
//                Plugin.Logger.LogInfo($"StasisSphere Max Time Before: {stasisSphere.maxTime}");
//                stasisSphere.maxTime = ModConfig.Instance.StasisSphereMaxTime;
//                Plugin.Logger.LogInfo($"StasisSphere Max Time After: {stasisSphere.maxTime}");
//
//                Plugin.Logger.LogInfo($"StasisSphere Min Time Before: {stasisSphere.minTime}");
//                stasisSphere.minTime = ModConfig.Instance.StasisSphereMinTime;
//                Plugin.Logger.LogInfo($"StasisSphere Min Time After: {stasisSphere.minTime}");
//            }
//            return true;
//        }
//    }
//}