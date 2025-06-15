using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace CustomBedsSubnautica.Config;

[Menu("CustomBed Config")]
public class CustomBedConfig : ConfigFile
{
    [Toggle("Enable Custom Beds", Tooltip = "Enable or disable custom beds feature. WARNING: Every placed bed will be destroyed on Save loadup")]
    public bool EnableCustomBeds { get; set; } = true;

    [Button("Open Custom Beds Folder", Tooltip = "Opens the Folder where all your BedPacks Should be located"), OnGameObjectCreated(nameof(OnOpenCustomBedsFolderCreated))]
    public void OpenCustomBedsFolder(ButtonClickedEventArgs e)
    {
        Debug.Log($"[CustomBedConfig] Button clicked: {e.Id}");
        CustomBedsUtils.OpenCustomBedsFolder();
    }

    [Button("Restore CustomBeds Folder", Tooltip = "Restores CustomBeds Folder for BedPacks if it does not exist")]
    public void CreateCustomBedsFolder(ButtonClickedEventArgs e)
    {
        Debug.Log($"[CustomBedConfig] Button clicked: {e.Id}");
        CustomBedsUtils.CreateCustomBedsFolder();
    }

    [Button("Automatic BedPack Install", Tooltip = "Pick A Zip file to install as an BedPack. Only Works under Windows")]
    public void PickAndExtractZip(ButtonClickedEventArgs e)
    {
        Debug.Log($"[CustomBedConfig] Button clicked: {e.Id}");
        CustomBedsUtils.PickAndExtractZip();
    }

    public void OnOpenCustomBedsFolderCreated()
    {
        Debug.Log("[CustomBedConfig] Open Custom Beds Folder button created.");
    }
}