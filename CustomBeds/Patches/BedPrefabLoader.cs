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
        public string matressTexture;
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
            string configPath = Path.Combine(folder, baseName + ".json");

            // --- Load Bed Config ---
            BedConfig config = null;
            if (File.Exists(configPath))
            {
                Debug.Log($"[BedPrefabLoader] Found config for {pngPath}: {configPath}");
                config = JsonConvert.DeserializeObject<BedConfig>(File.ReadAllText(configPath));
            }
            else
            {
                Debug.Log($"[BedPrefabLoader] No config found for {pngPath}");
            }

            Debug.Log($"[BedPrefabLoader] Processing PNG: {pngPath}");

            // --- Bed Basic Info ---
            string bedName = config?.techType ?? $"CustomBed_{baseName}_{index}";
            string displayName = config?.displayName ?? config?.name ?? baseName;
            string desc = config?.description ?? "A bed with a custom design.";

            Debug.Log($"[BedPrefabLoader] Registering prefab: {bedName} (Display: {displayName})");

            // --- Nautilus Prefab Setup ---
            var prefabInfo = PrefabInfo.WithTechType(bedName, displayName, desc);
            prefabInfo.WithIcon(SpriteManager.Get(TechType.Bed1));
            var prefab = new CustomPrefab(prefabInfo);
            var bedClone = new CloneTemplate(prefabInfo, TechType.Bed1);

            // --- Set Appearance/Textures ---
            bedClone.ModifyPrefab += gameObject =>
            {
                Debug.Log($"[BedPrefabLoader] Customizing cloned vanilla bed prefab for: {bedName}");

                // Load texture (matressTex is used for all)
                Texture2D matressTex = null;

                if (config?.matressTexture != null)
                {
                    string matressPath = Path.Combine(folder, config.matressTexture);
                    if (File.Exists(matressPath))
                    {
                        Debug.Log($"[BedPrefabLoader] Loading matress texture: {matressPath}");
                        matressTex = LoadTextureFromFile(matressPath);
                    }
                    else
                    {
                        Debug.Log($"[BedPrefabLoader] Matress texture specified in config but not found: {matressPath}");
                    }
                }
                if (matressTex == null)
                {
                    Debug.Log($"[BedPrefabLoader] Using main PNG as matress texture: {pngPath}");
                    matressTex = LoadTextureFromFile(pngPath);
                }

                // --- Apply Texture to Matress and Pillows ---
                var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                Debug.Log($"[BedPrefabLoader] Found {renderers.Length} renderers for prefab: {bedName}");

                foreach (var renderer in renderers)
                {
                    string rname = renderer.name.ToLowerInvariant();
                    Debug.Log($"[BedPrefabLoader] Renderer: {renderer.name}");

                    // Apply matress texture to all relevant renderers
                    if (rname.Contains("matress") 
                        || rname.Contains("pillow01") || rname.Contains("pillow_01")
                        || rname.Contains("pillow02") || rname.Contains("pillow_02"))
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
            if (config?.ingredients != null)
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
            // Unlock-TechType aus der Config lesen, sonst Bed1 als Fallback
            TechType unlockTechType = TechType.Bed1;
            if (!string.IsNullOrEmpty(config?.unlockTechType) && System.Enum.TryParse<TechType>(config.unlockTechType, out var parsedUnlock))
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

    // --- Helper: Load Texture From File ---
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