using HarmonyLib;
using UnityEngine;
using SubnauticaToolConfig.Settings;

namespace SubnauticaToolConfig.Patches
{
    [HarmonyPatch(typeof(Seaglide))]
    internal static class SeaglidePatch
    {
        [HarmonyPatch(nameof(Seaglide.Start))]
        [HarmonyPostfix]
        public static void Postfix(Seaglide __instance)
        {
            Debug.Log($"[SeaglidePatch] Original spinUpSpeed: {__instance.GetPrivateField<float>("spinUpSpeed")}");
            Debug.Log($"[SeaglidePatch] Original spinDownSpeed: {__instance.GetPrivateField<float>("spinDownSpeed")}");
            Debug.Log($"[SeaglidePatch] Original maxSpinSpeed: {__instance.GetPrivateField<float>("maxSpinSpeed")}");

            float newSpinUpSpeed = ModConfig.Instance.SeaglideSpinUpSpeed;
            float newSpinDownSpeed = ModConfig.Instance.SeaglideSpinDownSpeed;
            float newMaxSpinSpeed = ModConfig.Instance.SeaglideMaxSpinSpeed;

            __instance.SetPrivateField("spinUpSpeed", newSpinUpSpeed);
            __instance.SetPrivateField("spinDownSpeed", newSpinDownSpeed);
            __instance.SetPrivateField("maxSpinSpeed", newMaxSpinSpeed);

            Debug.Log($"[SeaglidePatch] Updated spinUpSpeed: {__instance.GetPrivateField<float>("spinUpSpeed")}");
            Debug.Log($"[SeaglidePatch] Updated spinDownSpeed: {__instance.GetPrivateField<float>("spinDownSpeed")}");
            Debug.Log($"[SeaglidePatch] Updated maxSpinSpeed: {__instance.GetPrivateField<float>("maxSpinSpeed")}");
        }
    }

    public static class ReflectionUtils
    {
        public static T GetPrivateField<T>(this object instance, string fieldName)
        {
            return (T)instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(instance);
        }

        public static void SetPrivateField<T>(this object instance, string fieldName, T value)
        {
            instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(instance, value);
        }
    }
}
