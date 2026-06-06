using UnityEngine;
using UnityEditor;
using System.IO;

namespace UmStretch.MaskMap
{
    public static class Utilities
    {
        /// <summary>
        /// Ensure texture is marked readable.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D MakeReadable(Texture2D texture)
        {
            if (texture == null)
            {
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

        /// <summary>
        /// Encode texture to PNG and save it to disk.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="saveLocation"></param>
        /// <param name="name"></param>
        public static void SaveToPng(Texture2D texture, string saveLocation, string name)
        {
            name = name.Replace(".png", "");

            byte[] texBytes = texture.EncodeToPNG();
            string path = Path.Combine(saveLocation, name + ".png");
            File.WriteAllBytes(path, texBytes);
            AssetDatabase.Refresh();
        }
    }
}