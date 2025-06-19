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

/// <summary>
/// Handles loading, configuration, and creation of custom bed prefabs,
/// including compositing multiple textures (mattress, blanket, pillows) into a single texture.
/// </summary>
public static class BedPrefabLoader
{
    // --- Static Rects for Texture Parts ---
    // These rectangles define the area (in pixels) on the final bed texture
    // where each part (pillow, blanket, mattress) will be drawn.
    // Adjust these positions and sizes as needed to match your texture layout!
    private static readonly Rect PillowUpperRect = new Rect(0, 252, 335, 535);
    private static readonly Rect PillowLowerRect = new Rect(0, 752, 335, 272);
    private static readonly Rect BlanketRect = new Rect(335, 769, 688, 255);
    private static readonly Rect MattressRect = new Rect(375, 1, 648, 793);

    /// <summary>
    /// Represents the configuration for a custom bed, as loaded from Config.json.
    /// </summary>
    public class BedConfig
    {
        public string techType;                 // Custom TechType for this bed (optional)
        public string displayName;              // Name to show in the build menu
        public string name;                     // Internal name (optional)
        public string description;              // Item description
        public string unlockTechType;           // TechType required to unlock this bed (optional)
        public List<IngredientConfig> ingredients; // Crafting recipe ingredients
        public string pillowUpperTexture;       // Filename for upper pillow texture (optional)
        public string pillowLowerTexture;       // Filename for lower pillow texture (optional)
        public string blanketTexture;           // Filename for blanket texture (optional)
        public string mattressTexture;          // Filename for mattress texture (optional)
        public string bedType;                  // Which vanilla bed type to clone ("Bed1", "Bed2", "NarrowBed")
    }

    /// <summary>
    /// Describes a single ingredient required to craft a bed.
    /// </summary>
    public class IngredientConfig
    {
        public string item;     // TechType string (e.g. "Titanium")
        public int amount;      // Amount required
    }

    /// <summary>
    /// Combines provided part textures (pillow, blanket, mattress) into a single texture,
    /// blending them over the original vanilla texture. Each part is "aspect-filled" (scaled to cover the assigned rectangle, preserving aspect ratio, and cropping excess).
    /// </summary>
    /// <param name="baseTex">Original vanilla texture (for alpha and base RGB).</param>
    /// <param name="pillowUpperTex">Texture for upper pillow (optional).</param>
    /// <param name="pillowLowerTex">Texture for lower pillow (optional).</param>
    /// <param name="blanketTex">Texture for blanket (optional).</param>
    /// <param name="mattressTex">Texture for mattress (optional).</param>
    /// <returns>A new Texture2D with all provided parts composited in their regions.</returns>
    public static Texture2D CombineBedParts(
        Texture2D baseTex,
        Texture2D pillowUpperTex,
        Texture2D pillowLowerTex,
        Texture2D blanketTex,
        Texture2D mattressTex
    )
    {
        // Dimensions of the final (vanilla) texture
        int width = baseTex.width, height = baseTex.height;
        // We'll build up the result in this array
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Get pixel data from the vanilla texture (for reference alpha and RGB)
        Color[] basePixels = baseTex.GetPixels();
        Color[] resultPixels = new Color[basePixels.Length];
        System.Array.Copy(basePixels, resultPixels, basePixels.Length); // Start as a copy of vanilla

        /// <summary>
        /// Draws srcTex onto the region destRect on the result texture,
        /// scaling the source with aspect fill (so the rectangle is fully covered, and any overflow is cropped),
        /// and preserving the original alpha channel.
        /// </summary>
        void PasteRGBAspectFill(Texture2D srcTex, Rect destRect)
        {
            // Calculate aspect ratios
            float srcAspect = (float)srcTex.width / srcTex.height;
            float destAspect = destRect.width / destRect.height;

            // Figure out how to scale and crop the source so it fills destRect
            float scale;
            float offsetX = 0, offsetY = 0;
            if (srcAspect > destAspect)
            {
                // Source is wider than destination: scale by height, crop width
                scale = destRect.height / srcTex.height;
                float cropW = srcTex.width - (destRect.width / scale);
                offsetX = cropW / 2f;
            }
            else
            {
                // Source is taller than destination: scale by width, crop height
                scale = destRect.width / srcTex.width;
                float cropH = srcTex.height - (destRect.height / scale);
                offsetY = cropH / 2f;
            }

            // For every pixel in the destination rectangle...
            for (int y = 0; y < destRect.height; y++)
            {
                for (int x = 0; x < destRect.width; x++)
                {
                    // Map destination (x,y) to the right place in the (possibly cropped) source
                    float srcX = (x / scale) + offsetX;
                    float srcY = (y / scale) + offsetY;

                    // Clamp coordinates to source texture boundaries
                    int srcXi = Mathf.Clamp(Mathf.RoundToInt(srcX), 0, srcTex.width - 1);
                    int srcYi = Mathf.Clamp(Mathf.RoundToInt(srcY), 0, srcTex.height - 1);

                    // Compute the absolute pixel in the full output texture
                    int targetX = (int)destRect.x + x;
                    int targetY = (int)destRect.y + y;
                    if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
                        continue; // Safety check

                    int pixelIdx = targetY * width + targetX;
                    Color src = srcTex.GetPixel(srcXi, srcYi);

                    // Use the original alpha from the vanilla/base texture for masking
                    float alpha = basePixels[pixelIdx].a;
                    resultPixels[pixelIdx] = new Color(src.r, src.g, src.b, alpha);
                }
            }
        }

        // Draw each part, if a texture was provided, in its assigned rectangle
        if (pillowUpperTex != null) PasteRGBAspectFill(pillowUpperTex, PillowUpperRect);
        if (pillowLowerTex != null) PasteRGBAspectFill(pillowLowerTex, PillowLowerRect);
        if (blanketTex != null) PasteRGBAspectFill(blanketTex, BlanketRect);
        if (mattressTex != null) PasteRGBAspectFill(mattressTex, MattressRect);

        // Store the result back into the texture
        result.SetPixels(resultPixels);
        result.Apply();
        return result;
    }

    /// <summary>
    /// Main entry point to discover, register, and set up all custom beds.
    /// Looks for Config.json files in the supplied imageFolder (recursively) and creates a prefab for each.
    /// </summary>
    /// <param name="imageFolder">Root directory to search for bed folders/configs</param>
    public static void RegisterCustomBeds(string imageFolder)
    {
        Debug.Log($"[BedPrefabLoader] RegisterCustomBeds called, scanning: {imageFolder}");

        // Check if the folder exists
        if (!Directory.Exists(imageFolder))
        {
            Debug.LogWarning($"[BedPrefabLoader] Beds folder does not exist: {imageFolder}");
            return;
        }

        // Find all Config.json files (each should define a bed)
        var configFiles = Directory.GetFiles(imageFolder, "Config.json", SearchOption.AllDirectories);
        Debug.Log($"[BedPrefabLoader] Found {configFiles.Length} Config.json files in {imageFolder} (and subfolders)");

        int index = 1;
        foreach (var configPath in configFiles)
        {
            string folder = Path.GetDirectoryName(configPath);

            // --- Load Bed Config ---
            BedConfig config = null;
            try
            {
                config = JsonConvert.DeserializeObject<BedConfig>(File.ReadAllText(configPath));
            }
            catch
            {
                Debug.LogWarning($"[BedPrefabLoader] Failed to parse config: {configPath}");
                continue;
            }

            Debug.Log($"[BedPrefabLoader] Processing config: {configPath}");

            // If techType not specified, auto-generate a unique name
            string bedName = config.techType ?? $"CustomBed_{index}";
            string displayName = config.displayName ?? config.name ?? bedName;
            string desc = config.description ?? "A bed with a custom design.";

            // Determine which vanilla bed to clone as a base
            TechType baseBedType = TechType.Bed1;
            if (!string.IsNullOrEmpty(config.bedType))
            {
                if (System.Enum.TryParse<TechType>(config.bedType, out var parsedType))
                    baseBedType = parsedType;
                else
                    Debug.LogWarning($"[BedPrefabLoader] Unknown bedType '{config.bedType}', fallback to Bed1.");
            }

            Debug.Log($"[BedPrefabLoader] Registering prefab: {bedName} (Display: {displayName}, Base: {baseBedType})");

            // Setup prefab info, icon, and prefab clone
            var prefabInfo = PrefabInfo.WithTechType(bedName, displayName, desc);
            prefabInfo.WithIcon(SpriteManager.Get(baseBedType));
            var prefab = new CustomPrefab(prefabInfo);
            var bedClone = new CloneTemplate(prefabInfo, baseBedType);

            // Attach code to modify the prefab's appearance/materials when it's created
            bedClone.ModifyPrefab += gameObject =>
            {
                Debug.Log($"[BedPrefabLoader] Customizing cloned vanilla bed prefab for: {bedName}");

                // Helper: loads a texture from the config by filename (returns null if not supplied or not found)
                Texture2D LoadPart(string texName) =>
                    !string.IsNullOrEmpty(texName)
                    ? LoadTextureFromFile(Path.Combine(folder, texName))
                    : null;

                // Load all provided textures for this bed
                Texture2D pillowUpperTex = LoadPart(config.pillowUpperTexture);
                Texture2D pillowLowerTex = LoadPart(config.pillowLowerTexture);
                Texture2D blanketTex = LoadPart(config.blanketTexture);
                Texture2D mattressTex = LoadPart(config.mattressTexture);

                // Find the vanilla material/texture (for alpha and for default RGB)
                Texture2D baseTex = null;
                var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    foreach (var mat in renderer.materials)
                    {
                        var tex = mat.GetTexture("_MainTex") as Texture2D;
                        if (tex != null && mat.name.StartsWith("matress_pillow")) // Only select the shared bed-part material!
                        {
                            baseTex = tex;
                            break;
                        }
                    }
                    if (baseTex != null) break;
                }
                if (baseTex == null)
                {
                    Debug.LogWarning($"[BedPrefabLoader] Could not find base texture for {bedName}, skipping texture assignment.");
                    return;
                }

                // Workaround: Lesbare Kopie erzeugen, falls nötig
                if (!baseTex.isReadable)
                    baseTex = MakeReadable(baseTex);

                // Compose the final bed texture from all provided parts!
                Texture2D combined = CombineBedParts(
                    baseTex,
                    pillowUpperTex,
                    pillowLowerTex,
                    blanketTex,
                    mattressTex
                );
                if (combined == null)
                {
                    Debug.LogWarning($"[BedPrefabLoader] Failed to generate combined texture for {bedName}.");
                    return;
                }

                // Apply the new composite texture ONLY to the shared soft-part material
                foreach (var renderer in renderers)
                {
                    foreach (var mat in renderer.materials)
                    {
                        if (mat.name.StartsWith("matress_pillow"))
                        {
                            // Clone material so beds don't share material instances
                            var newMat = new Material(mat);
                            newMat.SetTexture("_MainTex", combined);
                            mat.CopyPropertiesFromMaterial(newMat); // Overwrite properties in-place
                            Debug.Log($"[BedPrefabLoader] Applied combined texture to material: {mat.name} ({renderer.name})");
                        }
                    }
                }
            };

            // Attach the customized clone as the GameObject source for this prefab
            prefab.SetGameObject(bedClone);

            // --- Parse recipe ingredients from config ---
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
            // Fallback: default recipe if none specified
            if (recipe.Count == 0)
            {
                Debug.Log($"[BedPrefabLoader] No ingredients specified in config, using default recipe for {bedName}");
                recipe.Add(new Ingredient(TechType.FiberMesh, 2));
                recipe.Add(new Ingredient(TechType.Titanium, 1));
            }

            // --- Register the prefab with Nautilus ---
            TechType unlockTechType = TechType.Bed1;
            if (!string.IsNullOrEmpty(config.unlockTechType) && System.Enum.TryParse<TechType>(config.unlockTechType, out var parsedUnlock))
            {
                unlockTechType = parsedUnlock;
            }
            prefab.SetRecipe(new RecipeData(recipe.ToArray()));
            prefab.SetUnlock(unlockTechType);
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);
            prefab.Register();
            // Localization for PDA/tooltip
            LanguageHandler.SetLanguageLine(bedName, displayName);
            LanguageHandler.SetLanguageLine($"{bedName}_Description", desc);
            LanguageHandler.SetLanguageLine($"Tooltip_{bedName}", desc);
            Debug.Log($"[BedPrefabLoader] Registered bed prefab: {bedName}");

            index++;
        }
    }

    /// <summary>
    /// Loads a PNG file from disk into a Texture2D. Returns null if file not found.
    /// </summary>
    private static Texture2D LoadTextureFromFile(string path)
    {
        if (!File.Exists(path)) return null;
        Debug.Log($"[BedPrefabLoader] Loading texture from file: {path}");
        byte[] fileData = File.ReadAllBytes(path);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(fileData);
        Debug.Log($"[BedPrefabLoader] Loaded {tex.width}x{tex.height} texture from {path}");
        return tex;
    }

    /// <summary>
    /// Erstellt eine lesbare Kopie einer nicht-lesbaren Texture2D.
    /// </summary>
    private static Texture2D MakeReadable(Texture2D source)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(source, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }
}