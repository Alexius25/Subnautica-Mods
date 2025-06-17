using BepInEx;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CustomBedsSubnautica.Config
{
    public static class CustomBedsUtils
    {
        private static readonly string ModFolderPath = Path.Combine(Paths.PluginPath, "CustomBedsMod");
        private static readonly string CustomBedsPath = Path.Combine(ModFolderPath, "CustomBeds");

        public static void OpenCustomBedsFolder()
        {
            try
            {
                Process.Start("explorer.exe", CustomBedsPath);
                Main.Logger.LogInfo($"[CustomBedConfig] Custom beds folder opened: {CustomBedsPath}");
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"[CustomBedConfig] Failed to open folder: {ex.Message}");
                ShowMessage("Error", $"Failed to open folder: {ex.Message}");
            }
        }

        public static void PickAndExtractZip()
        {
            Directory.CreateDirectory(CustomBedsPath);

            Task.Run(() =>
            {
                try
                {
                    string downloadsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Downloads");

                    string[] selectedFiles = ShowFileDialogUsingPowerShell(downloadsPath);

                    if (selectedFiles == null || selectedFiles.Length == 0)
                    {
                        Main.Logger.LogInfo("[CustomBedConfig] No file selected");
                        return;
                    }

                    int successCount = 0;
                    int errorCount = 0;

                    foreach (string zipPath in selectedFiles)
                    {
                        try
                        {
                            ExtractZip(zipPath, CustomBedsPath);
                            successCount++;
                            Main.Logger.LogInfo($"[CustomBedConfig] Successfully extracted: {Path.GetFileName(zipPath)}");
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            Main.Logger.LogError($"[CustomBedConfig] Failed to extract {Path.GetFileName(zipPath)}: {ex.Message}");
                        }
                    }

                    ShowMessage(
                        "Installation Results",
                        $"Installation Complete!\n\nSuccessfully extracted: {successCount} file(s)\nFailed: {errorCount} file(s)\n\nFiles were extracted to:\n{CustomBedsPath}");
                }
                catch (Exception ex)
                {
                    Main.Logger.LogError($"[CustomBedConfig] Error during file selection or extraction: {ex.Message}");
                    ShowMessage("Error", $"An error occurred: {ex.Message}");
                }
            });
        }

        private static string[] ShowFileDialogUsingPowerShell(string initialDirectory)
        {
            try
            {
                string outputPath = Path.Combine(Path.GetTempPath(), "SelectedFiles.txt");

                string psScript = $@"
                Add-Type -AssemblyName System.Windows.Forms
                $openFileDialog = New-Object System.Windows.Forms.OpenFileDialog
                $openFileDialog.InitialDirectory = '{initialDirectory.Replace("'", "''")}'
                $openFileDialog.Filter = 'ZIP files (*.zip)|*.zip'
                $openFileDialog.Multiselect = $true
                $openFileDialog.Title = 'Select ZIP files to extract'

                if($openFileDialog.ShowDialog() -eq 'OK') {{
                $openFileDialog.FileNames | Out-File -FilePath '{outputPath.Replace("'", "''")}' -Encoding utf8
                }}
                ";

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript.Replace("\"", "\\\"")}\""
,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    };

                    StringBuilder errorOutput = new StringBuilder();
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                            errorOutput.AppendLine(args.Data);
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (!File.Exists(outputPath))
                    {
                        return Array.Empty<string>();
                    }

                    string[] selectedFiles = File.ReadAllLines(outputPath);

                    try
                    {
                        File.Delete(outputPath);
                    }
                    catch (Exception ex)
                    {
                        Main.Logger.LogWarning($"[CustomBedConfig] Failed to clean up temp files: {ex.Message}");
                    }

                    return selectedFiles;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"[CustomBedConfig] Error showing file dialog: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private static void ShowMessage(string title, string message)
        {
            try
            {
                // Ersetze Zeilenumbrüche durch echte Newlines für PowerShell
                string formattedMessage = message.Replace("'", "''").Replace("\n", Environment.NewLine);
                string psScript = $"[System.Windows.Forms.MessageBox]::Show('{formattedMessage}', '{title}')";

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"Add-Type -AssemblyName System.Windows.Forms; {psScript}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"[CustomBedConfig] Error showing message: {ex.Message}");
                Console.WriteLine($"{title}: {message}");
            }
        }

        private static void ExtractZip(string zipPath, string extractPath)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"& {{Expand-Archive -Path '{zipPath.Replace("'", "''")}' -DestinationPath '{extractPath.Replace("'", "''")}' -Force}}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    };

                    StringBuilder errorOutput = new StringBuilder();
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                            errorOutput.AppendLine(args.Data);
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"PowerShell extraction failed: {errorOutput}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ZIP extraction failed: {ex.Message}", ex);
            }
        }
    }
}