using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CraftData;

public static class BedPrefabLoader
{
    public class BedConfig
    {
        public string displayName;
        public string name;
        public string description;
        public List<IngredientConfig> ingredients;
        public string blanketTexture;
        public string pillowTexture;
        public string frameTexture;
        public string iconTexture;
    }

    public class IngredientConfig
    {
        public string item;
        public int amount;
    }

    public static void RegisterCustomBeds(string imageFolder)
    {
        Debug.Log($"[BedPrefabLoader] RegisterCustomBeds called, scanning: {imageFolder}");

        if (!Directory.Exists(imageFolder))
        {
            Debug.LogWarning($"[BedPrefabLoader] Beds folder does not exist: {imageFolder}");
            return;
        }

        var pngFiles = Directory.GetFiles(imageFolder, "*.png", SearchOption.AllDirectories);
        Debug.Log($"[BedPrefabLoader] Found {pngFiles.Length} PNG files in {imageFolder} (and subfolders)");

        int index = 1;
        foreach (var pngPath in pngFiles)
        {
            string baseName = Path.GetFileNameWithoutExtension(pngPath);
            string folder = Path.GetDirectoryName(pngPath);

            string configPath = Path.Combine(folder, baseName + ".json");

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

            if (config != null)
            {
                bool isBasePng =
                    (config.blanketTexture == null || Path.GetFileName(config.blanketTexture) == Path.GetFileName(pngPath)) &&
                    (config.pillowTexture == null || Path.GetFileName(config.pillowTexture) != Path.GetFileName(pngPath)) &&
                    (config.frameTexture == null || Path.GetFileName(config.frameTexture) != Path.GetFileName(pngPath));
                if (!isBasePng)
                {
                    Debug.Log($"[BedPrefabLoader] Skipping PNG {pngPath} since it's used as a part texture.");
                    continue;
                }
            }

            Debug.Log($"[BedPrefabLoader] Processing PNG: {pngPath}");

            string displayName = config?.displayName ?? config?.name ?? baseName;
            string bedName = $"CustomBed_{baseName}_{index}";
            string desc = config?.description ?? "A bed with a custom design.";

            string iconPath = config?.iconTexture != null
                ? Path.Combine(folder, config.iconTexture)
                : pngPath;

            Sprite iconSprite = null;
            if (File.Exists(iconPath))
            {
                iconSprite = LoadSpriteFromFile(iconPath);
            }
            if (iconSprite == null && File.Exists(pngPath))
            {
                iconSprite = LoadSpriteFromFile(pngPath);
            }
            if (iconSprite == null)
            {
                Debug.LogWarning($"[BedPrefabLoader] Could not load icon sprite for {bedName}, prefab may not show correctly.");
            }
            else
            {
                Debug.Log($"[BedPrefabLoader] Using icon: {(File.Exists(iconPath) ? iconPath : pngPath)}");
            }

            Debug.Log($"[BedPrefabLoader] Registering prefab: {bedName} (Display: {displayName})");

            var prefab = new CustomPrefab(bedName, displayName, desc, iconSprite);

            prefab.SetGameObject(() =>
            {
                Debug.Log($"[BedPrefabLoader] Instantiating vanilla bed prefab for: {bedName}");
                var vanillaBed = Resources.Load<GameObject>("WorldEntities/Tools/Bed1");
                var bedInstance = GameObject.Instantiate(vanillaBed);

                Texture2D blanketTex = null, pillowTex = null, frameTex = null;

                if (config?.blanketTexture != null)
                {
                    string blanketPath = Path.Combine(folder, config.blanketTexture);
                    if (File.Exists(blanketPath))
                    {
                        Debug.Log($"[BedPrefabLoader] Loading blanket texture: {blanketPath}");
                        blanketTex = LoadTextureFromFile(blanketPath);
                    }
                    else
                    {
                        Debug.Log($"[BedPrefabLoader] Blanket texture specified in config but not found: {blanketPath}");
                    }
                }
                if (blanketTex == null)
                {
                    Debug.Log($"[BedPrefabLoader] Using main PNG as blanket texture: {pngPath}");
                    blanketTex = LoadTextureFromFile(pngPath);
                }

                if (config?.pillowTexture != null)
                {
                    string pillowPath = Path.Combine(folder, config.pillowTexture);
                    if (File.Exists(pillowPath))
                    {
                        Debug.Log($"[BedPrefabLoader] Loading pillow texture: {pillowPath}");
                        pillowTex = LoadTextureFromFile(pillowPath);
                    }
                    else
                    {
                        Debug.Log($"[BedPrefabLoader] Pillow texture specified in config but not found: {pillowPath}");
                    }
                }

                if (config?.frameTexture != null)
                {
                    string framePath = Path.Combine(folder, config.frameTexture);
                    if (File.Exists(framePath))
                    {
                        Debug.Log($"[BedPrefabLoader] Loading frame texture: {framePath}");
                        frameTex = LoadTextureFromFile(framePath);
                    }
                    else
                    {
                        Debug.Log($"[BedPrefabLoader] Frame texture specified in config but not found: {framePath}");
                    }
                }

                var renderers = bedInstance.GetComponentsInChildren<Renderer>(true);
                Debug.Log($"[BedPrefabLoader] Found {renderers.Length} renderers for prefab: {bedName}");
                foreach (var renderer in renderers)
                {
                    string rname = renderer.name.ToLowerInvariant();
                    Debug.Log($"[BedPrefabLoader] Renderer: {renderer.name}");

                    if ((rname.Contains("blanket") || rname.Contains("cube")) && blanketTex != null)
                    {
                        Debug.Log($"[BedPrefabLoader] Applying blanket texture to renderer: {renderer.name}");
                        foreach (var mat in renderer.materials)
                            mat.mainTexture = blanketTex;
                    }
                    else if (rname.Contains("pillow") && pillowTex != null)
                    {
                        Debug.Log($"[BedPrefabLoader] Applying pillow texture to renderer: {renderer.name}");
                        foreach (var mat in renderer.materials)
                            mat.mainTexture = pillowTex;
                    }
                    else if ((rname.Contains("frame") || rname.Contains("base")) && frameTex != null)
                    {
                        Debug.Log($"[BedPrefabLoader] Applying frame texture to renderer: {renderer.name}");
                        foreach (var mat in renderer.materials)
                            mat.mainTexture = frameTex;
                    }
                }

                return bedInstance;
            });

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
            prefab.SetRecipe(new RecipeData(recipe.ToArray()));
            prefab.SetUnlock(TechType.Bed1); // vanilla bed unlock

            prefab.Register();
            Debug.Log($"[BedPrefabLoader] Registered bed prefab: {bedName}");
            index++;
        }
    }

    private static Texture2D LoadTextureFromFile(string path)
    {
        Debug.Log($"[BedPrefabLoader] Loading texture from file: {path}");
        byte[] fileData = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(fileData);
        return tex;
    }

    private static Sprite LoadSpriteFromFile(string path)
    {
        Debug.Log($"[BedPrefabLoader] Loading sprite from file: {path}");
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(File.ReadAllBytes(path));
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
