using UnityEngine;
using UnityEditor;

public class MaskMapGenerator : EditorWindow
{
    private static MaskMapGenerator window;
    private static Vector2 _minWindowSize = new Vector2(315, 420);

    private static Texture2D _metallicTexture;
    private static Texture2D _aoTexture;
    private static Texture2D _detailMaskTexture;
    private static Texture2D _smoothnessTexture;

    private static string _textureName = "NewMaskMap";
    private static string _generationLocation = "Assets/";

    [MenuItem("Tools/Mask Map Generator")]
    public static void OpenWindow()
    {
        window = GetWindow<MaskMapGenerator>("Mask Map Generator");

        window.minSize = _minWindowSize;
        window.maxSize = _minWindowSize;
        window.maxSize = Vector3.one * 10000;
    }

    static float value;
    void OnGUI()
    {
        window ??= GetWindow<MaskMapGenerator>("Mask Map Generator");

        // Textures
        TextureField("Metallic", ref _metallicTexture);
        TextureField("Ambient Occlusion", ref _aoTexture);
        TextureField("Detail Mask", ref _detailMaskTexture);
        TextureField("Smoothness", ref _smoothnessTexture);

        // Name
        GUILayout.FlexibleSpace();
        GUILayout.Label(new GUIContent("Name"), EditorStyles.boldLabel);
        _textureName = EditorGUILayout.TextField(_textureName, GUILayout.Width(window.position.width * 0.66f));

        // Save location
        GUILayout.Label(new GUIContent("Save Location", "Select a folder in the Project window, or browse for a save location."), EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        Object obj = Selection.activeObject;
        if (Selection.objects.Length > 0 && obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if(AssetDatabase.IsValidFolder(path))
            {
                _generationLocation = path;
            }
        }        
        _generationLocation = EditorGUILayout.TextField(_generationLocation);
        GUILayout.EndHorizontal();

        GUILayout.Button(new GUIContent("Generate Mask Map", "Generate a mask map, saved at the above location."), GUILayout.Height(48));

        Repaint();
    }

    private static void TextureField(string label, ref Texture2D texture)
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label(label, EditorStyles.boldLabel);
        if (texture == null)
        {
            GUILayout.FlexibleSpace();
            value = EditorGUILayout.Slider(value, 0, 1);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Height(64), GUILayout.Width(64));
        GUILayout.EndHorizontal();

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
    }
}
