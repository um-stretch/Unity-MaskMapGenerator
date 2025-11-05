using UnityEngine;
using UnityEditor;

public class MaskMapGenerator : EditorWindow
{
    private static MaskMapGenerator window;
    private static Vector2 _minWindowSize = new Vector2(315, 420);

    // Metallic, AO, Detail, Smoothness
    private static Texture2D[] _inputTextures = new Texture2D[4];
    private static float[] _fallbackValues = new float [4];

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

    private static void DrawTextureField(string label, int textureIndex)
    {
        Texture2D texture = _inputTextures[textureIndex];
        float fallbackValue = _fallbackValues[textureIndex];

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
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
}
