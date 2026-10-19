using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Generates temporary wood placeholder sprites and compatible item prefabs.
/// No existing pickup-system scripts are modified.
/// </summary>
public static class WoodPlaceholderSpriteGenerator
{
    private const int TextureSize = 64;
    private const float PixelsPerUnit = 64f;
    private const float ColliderSize = 0.8f;

    private static readonly Color Fill = new Color32(201, 154, 96, 255);
    private static readonly Color Outline = new Color32(105, 69, 37, 255);
    private static readonly Color Detail = new Color32(231, 190, 135, 255);
    private static readonly Color Transparent = new Color(0f, 0f, 0f, 0f);

    [MenuItem("Tools/Kobold Quest/Generate Wood Placeholder Sprites")]
    public static void GenerateSpritesOnly()
    {
        GenerateSprites();
    }

    [MenuItem("Tools/Kobold Quest/Generate Wood Placeholders and Prefabs")]
    public static void GenerateSpritesAndPrefabs()
    {
        GenerateSprites();
        GeneratePrefabs();
    }

    private static void GenerateSprites()
    {
        const string folder = "Assets/Sprites/Materials/Wood";
        EnsureFolder("Assets/Sprites");
        EnsureFolder("Assets/Sprites/Materials");
        EnsureFolder(folder);

        SaveShape(folder + "/Stick_Placeholder.png", 8, ShapeStyle.Stick);
        SaveShape(folder + "/Log_Placeholder.png", 6, ShapeStyle.Log);
        SaveShape(folder + "/Plank_Placeholder.png", 0, ShapeStyle.Plank);

        AssetDatabase.Refresh();
        ConfigureSprite(folder + "/Stick_Placeholder.png");
        ConfigureSprite(folder + "/Log_Placeholder.png");
        ConfigureSprite(folder + "/Plank_Placeholder.png");
        AssetDatabase.SaveAssets();
    }

    private static void GeneratePrefabs()
    {
        const string spriteFolder = "Assets/Sprites/Materials/Wood";
        const string prefabFolder = "Assets/Prefabs/Materials/Wood";
        const string playerPrefabPath = "Assets/Prefabs/Player.prefab";

        EnsureFolder("Assets/Prefabs/Materials");
        EnsureFolder(prefabFolder);

        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);
        if (playerPrefab == null)
        {
            Debug.LogError("Could not load " + playerPrefabPath +
                ". Prefabs were not generated because pickupobject needs the existing player reference.");
            return;
        }

        CreateWoodPrefab("Stick", ItemID.Stick,
            AssetDatabase.LoadAssetAtPath<Sprite>(spriteFolder + "/Stick_Placeholder.png"),
            prefabFolder + "/Stick.prefab", playerPrefab.transform);
        CreateWoodPrefab("Log", ItemID.Log,
            AssetDatabase.LoadAssetAtPath<Sprite>(spriteFolder + "/Log_Placeholder.png"),
            prefabFolder + "/Log.prefab", playerPrefab.transform);
        CreateWoodPrefab("Plank", ItemID.Plank,
            AssetDatabase.LoadAssetAtPath<Sprite>(spriteFolder + "/Plank_Placeholder.png"),
            prefabFolder + "/Plank.prefab", playerPrefab.transform);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Generated wood placeholder prefabs. Existing prefabs were left unchanged; verify pickup behaviour in Play mode.");
    }

    private static void CreateWoodPrefab(
        string itemName,
        ItemID itemID,
        Sprite sprite,
        string prefabPath,
        Transform playerTransform)
    {
        if (sprite == null)
        {
            Debug.LogError("Could not find placeholder sprite for " + itemName +
                ". Generate sprites first.");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.LogWarning("Prefab already exists; leaving it unchanged: " + prefabPath);
            return;
        }

        GameObject item = new GameObject(itemName);
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
            materialSettings.FindProperty("itemID").enumValueIndex = (int)itemID;
            materialSettings.FindProperty("materialType").enumValueIndex = (int)MaterialType.SurfaceWood;
            // Keep volume unset until proper sprite dimensions are established.
            materialSettings.FindProperty("volumeCubicMetres").floatValue = 0f;
            materialSettings.FindProperty("densityKgPerCubicMetre").floatValue =
                MaterialItem.DefaultWoodDensityKgPerCubicMetre;
            materialSettings.ApplyModifiedPropertiesWithoutUndo();

            // Match the existing pickup component and player reference pattern.
            pickupobject pickup = item.AddComponent<pickupobject>();
            SerializedObject pickupSettings = new SerializedObject(pickup);
            pickupSettings.FindProperty("PlayerTransform").objectReferenceValue = playerTransform;
            pickupSettings.FindProperty("ToolTransform").objectReferenceValue = item.transform;
            pickupSettings.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(item, prefabPath);
        }
        finally
        {
            Object.DestroyImmediate(item);
        }
    }

    private enum ShapeStyle { Stick, Log, Plank }

    private static void SaveShape(string path, int sides, ShapeStyle style)
    {
        var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        var pixels = new Color[TextureSize * TextureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Transparent;

        Vector2 center = new Vector2((TextureSize - 1) / 2f, (TextureSize - 1) / 2f);
        float radius = 27f;

        for (int y = 0; y < TextureSize; y++)
        for (int x = 0; x < TextureSize; x++)
        {
            Vector2 point = new Vector2(x, y);
            bool inside;
            bool border;

            if (style == ShapeStyle.Plank)
            {
                float distance = Vector2.Distance(point, center);
                inside = distance <= radius;
                border = distance >= radius - 3f;
            }
            else
            {
                inside = IsInsidePolygon(point, center, radius, sides);
                border = inside && !IsInsidePolygon(point, center, radius - 3f, sides);
            }

            if (!inside) continue;
            Color color = border ? Outline : Fill;

            if (style == ShapeStyle.Log)
            {
                float distance = Vector2.Distance(point, center);
                if ((distance < 14f && distance > 10f) || distance < 7f) color = Detail;
            }
            else if (style == ShapeStyle.Stick)
            {
                if (Mathf.Abs(point.y - center.y) < 2f &&
                    point.x > center.x - 13f && point.x < center.x + 13f && !border)
                    color = Detail;
            }
            else if (style == ShapeStyle.Plank)
            {
                if (Mathf.Abs(point.y - center.y) < 1.5f &&
                    Mathf.Abs(point.x - center.x) < 15f && !border)
                    color = Detail;
            }

            pixels[y * TextureSize + x] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
    }

    private static bool IsInsidePolygon(Vector2 point, Vector2 center, float radius, int sides)
    {
        float angleOffset = Mathf.PI / sides;
        Vector2[] vertices = new Vector2[sides];
        for (int i = 0; i < sides; i++)
        {
            float angle = (2f * Mathf.PI * i / sides) + angleOffset;
            vertices[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        bool inside = false;
        for (int i = 0, j = sides - 1; i < sides; j = i++)
        {
            bool crosses = ((vertices[i].y > point.y) != (vertices[j].y > point.y)) &&
                (point.x < (vertices[j].x - vertices[i].x) *
                (point.y - vertices[i].y) / (vertices[j].y - vertices[i].y) + vertices[i].x);
            if (crosses) inside = !inside;
        }
        return inside;
    }

    private static void ConfigureSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.SaveAndReimport();
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string name = Path.GetFileName(path);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
