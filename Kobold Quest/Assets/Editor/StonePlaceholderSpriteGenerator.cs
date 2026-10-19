using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Generates a simple grey circular stone placeholder and a compatible item prefab.
/// The PlayerTransform reference is intentionally left unset on the prefab asset:
/// a prefab asset cannot reference a Player instance in a scene.
/// </summary>
public static class StonePlaceholderSpriteGenerator
{
    private const int TextureSize = 64;
    private const float PixelsPerUnit = 64f;
    private const float ColliderSize = 0.8f;

    private static readonly Color Fill = new Color32(145, 148, 153, 255);
    private static readonly Color Outline = new Color32(75, 78, 83, 255);
    private static readonly Color Transparent = new Color(0f, 0f, 0f, 0f);

    private const string SpriteFolder = "Assets/Sprites/Materials/Stone";
    private const string SpritePath = SpriteFolder + "/Stone_Placeholder.png";
    private const string PrefabFolder = "Assets/Prefabs/Materials/Stone";
    private const string PrefabPath = PrefabFolder + "/Stone.prefab";

    [MenuItem("Tools/Kobold Quest/Generate Stone Placeholder Sprite")]
    public static void GenerateSpriteOnly()
    {
        GenerateSprite();
    }

    [MenuItem("Tools/Kobold Quest/Generate Stone Placeholder and Prefab")]
    public static void GenerateSpriteAndPrefab()
    {
        GenerateSprite();
        CreatePrefab();
    }

    private static void GenerateSprite()
    {
        EnsureFolder("Assets/Sprites");
        EnsureFolder("Assets/Sprites/Materials");
        EnsureFolder(SpriteFolder);

        var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        var pixels = new Color[TextureSize * TextureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Transparent;

        Vector2 center = new Vector2((TextureSize - 1) / 2f, (TextureSize - 1) / 2f);
        const float radius = 27f;
        const float outlineWidth = 3f;

        for (int y = 0; y < TextureSize; y++)
        for (int x = 0; x < TextureSize; x++)
        {
            float distance = Vector2.Distance(new Vector2(x, y), center);
            if (distance > radius)
                continue;

            pixels[y * TextureSize + x] =
                distance >= radius - outlineWidth ? Outline : Fill;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(SpritePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.Refresh();

        var importer = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.SaveAndReimport();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated stone placeholder sprite: " + SpritePath);
    }

    private static void CreatePrefab()
    {
        EnsureFolder("Assets/Prefabs/Materials");
        EnsureFolder(PrefabFolder);

        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
        {
            Debug.LogWarning("Stone prefab already exists; leaving it unchanged: " + PrefabPath);
            return;
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        if (sprite == null)
        {
            Debug.LogError("Stone sprite was not found. Generate the placeholder sprite first.");
            return;
        }

        GameObject item = new GameObject("Stone");
        try
        {
            item.tag = "Item";
            item.transform.localScale = Vector3.one;

            SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.drawMode = SpriteDrawMode.Simple;
            renderer.sortingOrder = 0;

            Rigidbody2D body = item.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.freezeRotation = true;

            BoxCollider2D collider = item.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(ColliderSize, ColliderSize);
            collider.isTrigger = false;

            MaterialItem materialItem = item.AddComponent<MaterialItem>();
            SerializedObject materialSettings = new SerializedObject(materialItem);
            materialSettings.FindProperty("itemID").enumValueIndex = (int)ItemID.Stone;
            materialSettings.FindProperty("materialType").enumValueIndex = (int)MaterialType.SurfaceStone;
            materialSettings.FindProperty("volumeCubicMetres").floatValue =
                MaterialItem.DefaultStoneVolumeCubicMetres;
            materialSettings.FindProperty("densityKgPerCubicMetre").floatValue =
                MaterialItem.DefaultStoneDensityKgPerCubicMetre;
            materialSettings.ApplyModifiedPropertiesWithoutUndo();

            pickupobject pickup = item.AddComponent<pickupobject>();
            SerializedObject pickupSettings = new SerializedObject(pickup);
            // PlayerTransform must be assigned to the actual Player instance after
            // placing this prefab in a scene, not to the Player prefab asset.
            pickupSettings.FindProperty("PlayerTransform").objectReferenceValue = null;
            pickupSettings.FindProperty("ToolTransform").objectReferenceValue = item.transform;
            pickupSettings.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(item, PrefabPath);
        }
        finally
        {
            Object.DestroyImmediate(item);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated stone placeholder prefab: " + PrefabPath +
            ". Assign PlayerTransform to the Player instance in the scene before testing pickup.");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string name = Path.GetFileName(path);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
