using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
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

            Debug.Log($"[CustomBedConfig] Custom beds folder opened: {folderPath}");
        }

        public static void CreateCustomBedsFolder()
        {
            var folderPath = Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");
            Directory.CreateDirectory(folderPath);
            Debug.Log($"[CustomBedConfig] Custom beds folder created (Did not exist or could not be found): {folderPath}");
        }

        public static void PickAndExtractZip()
        {
            string downloadsPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            string[] zipPaths = null;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "ZIP files (*.zip)|*.zip";
                dialog.Title = "Select one or more ZIP files";
                dialog.Multiselect = true; // Allow multiple selection
                dialog.InitialDirectory = downloadsPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    zipPaths = dialog.FileNames;
                }
            }

            if (zipPaths == null || zipPaths.Length == 0)
            {
                Debug.Log("[CustomBedConfig] No ZIP selected");
                return;
            }

            var extractPath = Path.Combine(Paths.PluginPath, "CustomBedsMod", "CustomBeds");
            Directory.CreateDirectory(extractPath);

            foreach (var zipPath in zipPaths)
            {
                try
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Debug.Log($"[CustomBedConfig] ZIP extracted: {zipPath} -> {extractPath}");
                }
                catch (IOException ex)
                {
                    Debug.LogError($"[CustomBedConfig] Error while extracting {zipPath}: {ex.Message}");
                }
            }
        }
    }
}