using BepInEx;
using CustomBedsSubnautica;
using CustomBedsSubnautica.Config;
using HarmonyLib;
using Nautilus.Handlers;
using System.IO;
using static BedPrefabLoader;

[BepInPlugin("com.Alexius25.CustomBeds", "Custom Beds", "1.0.0")]
public class Mod : BaseUnityPlugin
{
    public static string BedImageFolder => Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");

    private void Awake()
    {
        Logger.LogInfo("[CustomBeds] Plugin Awake, starting initialization...");
        Logger.LogInfo($"[CustomBeds] Bed image folder is: {BedImageFolder}");
        Harmony.CreateAndPatchAll(typeof(Mod));
        Directory.CreateDirectory(BedImageFolder);

        // Nautilus-Konfiguration registrieren und Instanz erhalten
        var config = OptionsPanelHandler.RegisterModOptions<CustomBedConfig>();

        if (config.EnableCustomBeds)
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