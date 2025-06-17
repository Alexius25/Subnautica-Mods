using BepInEx;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CraftData;

public static class BedPrefabLoader
{
    // --- Config Classes ---
    public class BedConfig
    {
        public string techType; // Optional: Custom TechType string
        public string displayName;
        public string name;
        public string description;
        public string unlockTechType; // Optional: TechType to unlock this bed
        public List<IngredientConfig> ingredients;
        public string texture;
        public string bedType; // "Bed1", "Bed2" oder "NarrowBed"
    }

    public class IngredientConfig
    {
        public string item;
        public int amount;
    }

    // --- Public Bed Registration Method ---
    public static void RegisterCustomBeds(string imageFolder)
    {
        Debug.Log($"[BedPrefabLoader] RegisterCustomBeds called, scanning: {imageFolder}");

        // Validate folder
        if (!Directory.Exists(imageFolder))
        {
            Debug.LogWarning($"[BedPrefabLoader] Beds folder does not exist: {imageFolder}");
            return;
        }

        // Discover PNG files
        var pngFiles = Directory.GetFiles(imageFolder, "*.png", SearchOption.AllDirectories);
        Debug.Log($"[BedPrefabLoader] Found {pngFiles.Length} PNG files in {imageFolder} (and subfolders)");

        int index = 1;
        foreach (var pngPath in pngFiles)
        {
            string baseName = Path.GetFileNameWithoutExtension(pngPath);
            string folder = Path.GetDirectoryName(pngPath);
            string configPath = Path.Combine(folder, "Config.json");

            // --- Load Bed Config ---
            BedConfig config = null;
            if (File.Exists(configPath))
            {
                Debug.Log($"[BedPrefabLoader] Found config: {configPath}");
                config = JsonConvert.DeserializeObject<BedConfig>(File.ReadAllText(configPath));
            }
            else
            {
                Debug.LogWarning($"[BedPrefabLoader] No config found in {folder}, skipping {pngPath}");
                continue; // Ohne Config keine Registrierung
            }

            Debug.Log($"[BedPrefabLoader] Processing PNG: {pngPath}");

            // --- Bed Basic Info ---
            string bedName = config.techType ?? $"CustomBed_{baseName}_{index}";
            string displayName = config.displayName ?? config.name ?? baseName;
            string desc = config.description ?? "A bed with a custom design.";

            // --- Bed Type bestimmen ---
            TechType baseBedType = TechType.Bed1; // Default
            if (!string.IsNullOrEmpty(config.bedType))
            {
                if (System.Enum.TryParse<TechType>(config.bedType, out var parsedType))
                    baseBedType = parsedType;
                else
                    Debug.LogWarning($"[BedPrefabLoader] Unknown bedType '{config.bedType}', fallback auf Bed1.");
            }
            else
            {
                Debug.Log($"[BedPrefabLoader] Kein bedType in Config, verwende Bed1.");
            }

            Debug.Log($"[BedPrefabLoader] Registering prefab: {bedName} (Display: {displayName}, Base: {baseBedType})");

            // --- Nautilus Prefab Setup ---
            var prefabInfo = PrefabInfo.WithTechType(bedName, displayName, desc);
            prefabInfo.WithIcon(SpriteManager.Get(baseBedType));
            var prefab = new CustomPrefab(prefabInfo);
            var bedClone = new CloneTemplate(prefabInfo, baseBedType);

            // --- Set Appearance/Textures ---
            bedClone.ModifyPrefab += gameObject =>
            {
                Debug.Log($"[BedPrefabLoader] Customizing cloned vanilla bed prefab for: {bedName}");

                // Load texture from config
                Texture2D matressTex = null;

                if (!string.IsNullOrEmpty(config.texture))
                {
                    string matressPath = Path.Combine(folder, config.texture);
                    if (File.Exists(matressPath))
                    {
                        Debug.Log($"[BedPrefabLoader] Loading matress texture: {matressPath}");
                        matressTex = LoadTextureFromFile(matressPath);
                    }
                    else
                    {
                        Debug.LogWarning($"[BedPrefabLoader] Matress texture specified in config but not found: {matressPath}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[BedPrefabLoader] No texture specified in config for {bedName}");
                }

                if (matressTex == null)
                {
                    Debug.LogWarning($"[BedPrefabLoader] No valid texture loaded for {bedName}, skipping texture assignment.");
                    return;
                }

                // --- Apply texture to relevant renderers ---
                var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                Debug.Log($"[BedPrefabLoader] Found {renderers.Length} renderers for prefab: {bedName}");

                foreach (var renderer in renderers)
                {
                    string rname = renderer.name.ToLowerInvariant();
                    Debug.Log($"[BedPrefabLoader] Renderer: {renderer.name}");

                    bool apply = false;
                    switch (baseBedType)
                    {
                        case TechType.Bed1:
                            apply = rname.Contains("matress") || rname.Contains("pillow_01") || rname.Contains("pillow_02");
                            break;
                        case TechType.Bed2:
                            apply = rname.Contains("matress") || rname.Contains("pillow_01") || rname.Contains("pillow_02") || rname.Contains("blanket");
                            break;
                        case TechType.NarrowBed:
                            apply = rname.Contains("matress_narrow") || rname.Contains("pillow_01") || rname.Contains("blanket_narrow");
                            break;
                    }

                    if (apply)
                    {
                        var mats = renderer.materials;
                        for (int i = 0; i < mats.Length; i++)
                        {
                            mats[i] = new Material(mats[i]);
                            mats[i].SetTexture("_MainTex", matressTex);
                            Debug.Log($"[BedPrefabLoader] Applied shared texture to material {i} of {renderer.name}");
                        }
                        renderer.materials = mats;
                    }
                }
            };

            prefab.SetGameObject(bedClone);

            // --- Recipe Parsing ---
            var recipe = new List<Ingredient>();
            if (config.ingredients != null)
            {
                Debug.Log($"[BedPrefabLoader] Parsing ingredients for {bedName}");
                foreach (var ing in config.ingredients)
                {
                    Debug.Log($"[BedPrefabLoader] Ingredient: {ing.item} x{ing.amount}");
                    if (System.Enum.TryParse<TechType>(ing.item, out var techType))
                        recipe.Add(new Ingredient(techType, ing.amount));
                    else
                        Debug.LogWarning($"[BedPrefabLoader] Unknown TechType: {ing.item}");
                }
            }
            if (recipe.Count == 0)
            {
                Debug.Log($"[BedPrefabLoader] No ingredients specified in config, using default recipe for {bedName}");
                recipe.Add(new Ingredient(TechType.FiberMesh, 2));
                recipe.Add(new Ingredient(TechType.Titanium, 1));
            }

            // --- Register Prefab ---
            TechType unlockTechType = TechType.Bed1;
            if (!string.IsNullOrEmpty(config.unlockTechType) && System.Enum.TryParse<TechType>(config.unlockTechType, out var parsedUnlock))
            {
                unlockTechType = parsedUnlock;
            }
            prefab.SetRecipe(new RecipeData(recipe.ToArray()));
            prefab.SetUnlock(unlockTechType);
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            prefab.Register();
            LanguageHandler.SetLanguageLine(bedName, displayName);
            LanguageHandler.SetLanguageLine($"{bedName}_Description", desc);
            LanguageHandler.SetLanguageLine($"Tooltip_{bedName}", desc);
            Debug.Log($"[BedPrefabLoader] Registered bed prefab: {bedName}");

            index++;
        }
    }

    // --- Helper: Load texture From File ---
    private static Texture2D LoadTextureFromFile(string path)
    {
        Debug.Log($"[BedPrefabLoader] Loading texture from file: {path}");
        byte[] fileData = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(fileData);
        Debug.Log($"[BedPrefabLoader] Loaded {tex.width}x{tex.height} texture from {path}");
        return tex;
    }
}