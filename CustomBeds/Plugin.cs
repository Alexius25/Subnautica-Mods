using BepInEx;
using BepInEx.Logging;
using CustomBedsSubnautica;
using CustomBedsSubnautica.Config;
using HarmonyLib;
using Nautilus.Handlers;
using System.IO;
using static BedPrefabLoader;
using static OVRHaptics;

[BepInPlugin("com.Alexius25.CustomBeds", "Custom Beds", "1.0.0")]
[BepInDependency("com.snmodding.nautilus")]
public class Main : BaseUnityPlugin
{
    public static string BedImageFolder => Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");

    internal static new ManualLogSource Logger { get; private set; }
    internal static new CustomBedConfig Config { get; } = OptionsPanelHandler.RegisterModOptions<CustomBedConfig>();

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("[CustomBeds] Plugin Awake, starting initialization...");
        Logger.LogInfo($"[CustomBeds] Bed image folder is: {BedImageFolder}");
        Harmony.CreateAndPatchAll(typeof(Main));
        Directory.CreateDirectory(BedImageFolder);

        // Nautilus-Konfiguration registrieren und Instanz erhalten

        if (Config.EnableCustomBeds)
        {
            Logger.LogInfo("[CustomBeds] Registering custom beds...");
            BedPrefabLoader.RegisterCustomBeds(BedImageFolder);
        }
        else
        {
            Logger.LogInfo("[CustomBeds] Custom beds sind deaktiviert. Änderungen werden erst nach Neustart wirksam.");
        }

        Logger.LogInfo("[CustomBeds] Initialization complete.");
    }
}