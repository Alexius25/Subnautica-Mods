using System.IO;
using BepInEx;
using UnityEngine;

namespace CustomBedsSubnautica.Config
{
    public static class CustomBedsUtils
    {
        public static void OpenCustomBedsFolder()
        {
            var folderPath = Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });

            Debug.Log($"[CustomBedConfig] Custom beds folder geöffnet: {folderPath}");
        }

        public static void CreateCustomBedsFolder()
        {
            var folderPath = Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");
            Directory.CreateDirectory(folderPath);
            Debug.Log($"[CustomBedConfig] Custom beds folder created (Did not exist or could not be found): {folderPath}");
        }
    }
}