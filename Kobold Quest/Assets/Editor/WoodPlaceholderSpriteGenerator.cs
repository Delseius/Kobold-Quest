using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Creates temporary one-cell placeholder sprites for the wood item types.
/// Run from Tools > Kobold Quest > Generate Wood Placeholder Sprites.
/// Generated sprites are 64x64 pixels at 64 pixels per unit, so their
/// full texture bounds represent one Unity world unit. Shapes are inset
/// from the texture edges to leave a small visual margin.
/// </summary>
public static class WoodPlaceholderSpriteGenerator
{
    private const int TextureSize = 64;
    private const float PixelsPerUnit = 64f;

    private static readonly Color Fill = new Color32(201, 154, 96, 255);
    private static readonly Color Outline = new Color32(105, 69, 37, 255);
    private static readonly Color Detail = new Color32(231, 190, 135, 255);
    private static readonly Color Transparent = new Color(0f, 0f, 0f, 0f);

    [MenuItem("Tools/Kobold Quest/Generate Wood Placeholder Sprites")]
    public static void Generate()
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
        Debug.Log("Generated Stick, Log, and Plank placeholder sprites.");
    }

    private enum ShapeStyle
    {
        Stick,
        Log,
        Plank
    }

    private static void SaveShape(string path, int sides, ShapeStyle style)
    {
        var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        var pixels = new Color[TextureSize * TextureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Transparent;

        Vector2 center = new Vector2((TextureSize - 1) / 2f, (TextureSize - 1) / 2f);
        float radius = 27f;

        for (int y = 0; y < TextureSize; y++)
        {
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

                if (!inside)
                    continue;

                Color color = border ? Outline : Fill;

                // Simple readable grain marks; these are purely decorative.
                if (style == ShapeStyle.Log)
                {
                    float d = Vector2.Distance(point, center);
                    if (d < 14f && d > 10f)
                        color = Detail;
                    if (d < 7f)
                        color = Detail;
                }
                else if (style == ShapeStyle.Stick)
                {
                    if (Mathf.Abs(point.y - center.y) < 2f &&
                        point.x > center.x - 13f && point.x < center.x + 13f &&
                        !border)
                        color = Detail;
                }
                else if (style == ShapeStyle.Plank)
                {
                    if (Mathf.Abs(point.y - center.y) < 1.5f &&
                        Mathf.Abs(point.x - center.x) < 15f &&
                        !border)
                        color = Detail;
                }

                pixels[y * TextureSize + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
    }

    private static bool IsInsidePolygon(Vector2 point, Vector2 center, float radius, int sides)
    {
        // Flat-topped polygons with a slight rotation keep their silhouettes distinct.
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
                (point.y - vertices[i].y) / (vertices[j].y - vertices[i].y) +
                vertices[i].x);

            if (crosses)
                inside = !inside;
        }

        return inside;
    }

    private static void ConfigureSprite(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

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
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string name = Path.GetFileName(path);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
