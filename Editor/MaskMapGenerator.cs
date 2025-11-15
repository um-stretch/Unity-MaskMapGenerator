using UnityEngine;
using UnityEditor;
using System.IO;

public class MaskMapGenerator : EditorWindow
{
    private static MaskMapGenerator window;
    private static Vector2 _minWindowSize = new Vector2(315, 420);

    // Metallic, AO, Detail, Smoothness
    private static Texture2D[] _inputTextures = new Texture2D[4];
    private static float[] _fallbackValues = new float [4];

    // Defaults
    private static string _textureName = "NewMaskMap";
    private static string _saveLocation = "Assets/";

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
        GUILayout.Button("?");
        GUILayout.EndHorizontal();
        _textureName = EditorGUILayout.TextField(_textureName, GUILayout.Width(window.position.width * 0.66f));

        // Save location
        GUILayout.Label(new GUIContent("Save Location", "Select a folder in the Project window, or browse for a save location."), EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        _saveLocation = EditorGUILayout.TextField(_saveLocation);

        if(GUILayout.Button(new GUIContent("...", "Browse"), GUILayout.Width(24)))
        {
            // string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            // string absolutePath = EditorUtility.OpenFolderPanel("Save Location", "Assets", "");
            // projectRoot = projectRoot.Replace("\\", "/");

            // absolutePath = absolutePath.Substring(projectRoot.Length + 1);

            // if (absolutePath.StartsWith(projectRoot))
            // _saveLocation = absolutePath;

            // Debug.Log(absolutePath);
            // Debug.Log(_saveLocation);

            // Repaint();
        }
        GUILayout.EndHorizontal();

        GUILayout.Button(new GUIContent("Generate Mask Map", "Generate a mask map, saved at the above location."), GUILayout.Height(48));        
    }

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

    // private void GenerateMaskMap(Texture2D metallicMap, Texture2D occlusionMap, Texture2D detailMap, Texture2D smoothnessMap)
    // {
    //     var firstNonNullRef = metallicMap ?? occlusionMap ?? detailMap ?? smoothnessMap;
    //     if (firstNonNullRef == null)
    //         return;

    //     int resolution = firstNonNullRef.width;

    //     Color[] mPixels = metallicMap != null ? metallicMap.GetPixels() : new Color[resolution * resolution];
    //     Color[] oPixels = occlusionMap != null ? occlusionMap.GetPixels() : Enumerable.Repeat(Color.white, resolution * resolution).ToArray();
    //     Color[] dPixels = detailMap != null ? detailMap.GetPixels() : Enumerable.Repeat(Color.white, resolution * resolution).ToArray();
    //     Color[] sPixels = smoothnessMap != null ? smoothnessMap.GetPixels() : Enumerable.Repeat(Color.white, resolution * resolution).ToArray();

    //     Texture2D maskMap = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
    //     Color[] maskPixels = new Color[resolution * resolution];

    //     if (config.enableMultithreading)
    //     {
    //         System.Threading.Tasks.Parallel.For(0, resolution * resolution, i =>
    //         {
    //             float m = mPixels[i].grayscale;
    //             float o = oPixels[i].grayscale;
    //             float d = dPixels[i].grayscale;
    //             float s = sPixels[i].grayscale;

    //             maskPixels[i] = new Color(m, o, d, s);
    //         });
    //     }
    //     else
    //     {
    //         for (int i = 0; i < resolution * resolution; i++)
    //         {
    //             float m = mPixels[i].grayscale;
    //             float o = oPixels[i].grayscale;
    //             float d = dPixels[i].grayscale;
    //             float s = sPixels[i].grayscale;

    //             maskPixels[i] = new Color(m, o, d, s);
    //         }
    //     }

    //     maskMap.SetPixels(maskPixels);
    //     maskMap.Apply();

    //     byte[] maskMapBytes = maskMap.EncodeToPNG();
    //     string path = $"{_destinationPath}/{_assetname}_MaskMap.png";
    //     File.WriteAllBytes(path, maskMapBytes);
    //     AssetDatabase.Refresh();

    //     LogContext("Generate maskMap...OK");
    // }

}
