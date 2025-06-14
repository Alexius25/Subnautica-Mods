using BepInEx;
using HarmonyLib;
using System.IO;

[BepInPlugin("com.yourname.CustomBeds", "Custom Beds", "1.0.0")]
public class Mod : BaseUnityPlugin
{
    public static string BedImageFolder => Path.Combine(Paths.PluginPath, "CustomBeds", "CustomBeds");

    private void Awake()
    {
        Logger.LogInfo("[CustomBeds] Plugin Awake, starting initialization...");
        Logger.LogInfo($"[CustomBeds] Bed image folder is: {BedImageFolder}");
        Harmony.CreateAndPatchAll(typeof(Mod));
        Directory.CreateDirectory(BedImageFolder);
        Logger.LogInfo("[CustomBeds] Registering custom beds...");
        BedPrefabLoader.RegisterCustomBeds(BedImageFolder);
        Logger.LogInfo("[CustomBeds] Initialization complete.");
    }
}