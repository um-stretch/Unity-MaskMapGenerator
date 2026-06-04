using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace UmStretch.MaskMapGenerator
{
    public class MaskMapGenerator : EditorWindow
    {
        private static MaskMapGenerator window;
        private static Vector2 _minWindowSize = new Vector2(315, 420);

        // Metallic, AO, Detail, Smoothness
        private static Texture2D[] _inputTextures = new Texture2D[4];
        private static float[] _fallbackValues = new float[4];

        private static string _textureName = "NewMaskMap";
        private static string _saveLocation = "Assets/";
        private static int _resolution = 1024;

        [MenuItem("Tools/Mask Map Generator")]
        public static void OpenWindow()
        {
            window = GetWindow<MaskMapGenerator>("Mask Map Generator");

            window.minSize = _minWindowSize;
            window.maxSize = _minWindowSize;
            window.maxSize = Vector3.one * 10000;
        }

        void OnGUI()
        {
            window ??= GetWindow<MaskMapGenerator>("Mask Map Generator");

            // Textures
            DrawTextureField("Metallic", 0);
            DrawTextureField("Ambient Occlusion", 1);
            DrawTextureField("Detail Mask", 2);
            DrawTextureField("Smoothness", 3);

            // Name
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name"), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // View source
            if (GUILayout.Button(new GUIContent("?", "View source.")))
            {
                Application.OpenURL("https://github.com/um-stretch/Unity-MaskMapGenerator");
            }
            GUILayout.EndHorizontal();

            _textureName = EditorGUILayout.TextField(_textureName, GUILayout.Width(window.position.width * 0.66f));

            // Save location
            GUILayout.Label(new GUIContent("Save Location"), EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            _saveLocation = EditorGUILayout.TextField(_saveLocation);

            if (GUILayout.Button(new GUIContent("...", "Browse"), GUILayout.Width(24)))
            {
                string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                projectRoot = projectRoot.Replace("\\", "/");

                string absolutePath = EditorUtility.OpenFolderPanel("Save Location", "Assets", "");

                if (!String.IsNullOrEmpty(absolutePath) && absolutePath.StartsWith(projectRoot))
                {
                    string savePath = absolutePath.Substring(projectRoot.Length + 1);
                    _saveLocation = savePath;
                }
                else
                {
                    _saveLocation = "Assets/";
                }
            }
            GUILayout.EndHorizontal();

            // Generate mask map
            if (GUILayout.Button(new GUIContent("Generate Mask Map", "Generate a mask map, saved at the above location."), GUILayout.Height(48)))
            {
                VerifyInputTextures();

                GenerateMaskMap();
            }
        }

        // Allow input textures, fall back to float range otherwise.
        private static void DrawTextureField(string label, int textureIndex)
        {
            Texture2D texture = _inputTextures[textureIndex];
            float fallbackValue = _fallbackValues[textureIndex];

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label(label, EditorStyles.boldLabel);
            if (texture == null)
            {
                GUILayout.FlexibleSpace();
                _fallbackValues[textureIndex] = EditorGUILayout.Slider(fallbackValue, 0, 1);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            _inputTextures[textureIndex] = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
            GUILayout.EndHorizontal();

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }

        // Ensure input textures are readable.
        private static void VerifyInputTextures()
        {
            for (int i = 0; i < _inputTextures.Length; i++)
            {
                Texture2D tex = _inputTextures[i];
                if (tex == null)
                    continue;

                string texPath = AssetDatabase.GetAssetPath(tex);
                if (string.IsNullOrEmpty(texPath))
                    continue;

                var importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
                if (importer == null)
                    continue;

                if (!importer.isReadable)
                {
                    importer.isReadable = true;
                    AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        private static void GenerateMaskMap()
        {
            double sTime = EditorApplication.timeSinceStartup;
            Texture2D firstNonNullRef = _inputTextures[0] ?? _inputTextures[1] ?? _inputTextures[2] ?? _inputTextures[3];
            _resolution = firstNonNullRef == null ? Config.defaultResolution : firstNonNullRef.width;

            Color[] mPixels = GetInputPixels(0);
            Color[] oPixels = GetInputPixels(1);
            Color[] dPixels = GetInputPixels(2);
            Color[] sPixels = GetInputPixels(3);

            Texture2D maskMap = new Texture2D(_resolution, _resolution, TextureFormat.RGBA32, false);
            Color[] maskPixels = new Color[_resolution * _resolution];

            if (Config.useMultithreading)
            {
                System.Threading.Tasks.Parallel.For(0, _resolution * _resolution, i =>
                {
                    float m = mPixels[i].grayscale;
                    float o = oPixels[i].grayscale;
                    float d = dPixels[i].grayscale;
                    float s = sPixels[i].grayscale;

                    maskPixels[i] = new Color(m, o, d, s);
                });
            }
            else
            {
                for (int i = 0; i < _resolution * _resolution; i++)
                {
                    float m = mPixels[i].grayscale;
                    float o = oPixels[i].grayscale;
                    float d = dPixels[i].grayscale;
                    float s = sPixels[i].grayscale;

                    maskPixels[i] = new Color(m, o, d, s);
                }
            }

            maskMap.SetPixels(maskPixels);
            maskMap.Apply();

            byte[] maskMapBytes = maskMap.EncodeToPNG();
            string path = $"{_saveLocation}/{_textureName}.png";
            File.WriteAllBytes(path, maskMapBytes);
            AssetDatabase.Refresh();

            Debug.Log(EditorApplication.timeSinceStartup - sTime);
        }

        // Use input texture if available, otherwise use fallback value to create greyscale texture.
        private static Color[] GetInputPixels(int index)
        {
            return _inputTextures[index] != null ? _inputTextures[index].GetPixels() : Enumerable.Repeat(Color.white * _fallbackValues[index], _resolution * _resolution).ToArray();
        }
    }
}