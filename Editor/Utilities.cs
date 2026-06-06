using UnityEngine;
using UnityEditor;

public static class Utilities
{
    // Ensure texture is readable.
    public static Texture2D MakeReadable(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogWarning("Input texture is null.");
            return null;
        }

        string texPath = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(texPath))
        {
            Debug.LogWarning("Failed to find texture path.");          
            return null;
        }

        var importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("Failed to get texture importer.");
            return null;
        }

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
        }

        return texture;
    }
}