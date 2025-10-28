using UnityEngine;
using UnityEditor;

public class MaskMapGenerator : EditorWindow
{
    private static MaskMapGenerator window;

    [MenuItem("Tools/Mask Map Generator")]
    public static void OpenWindow()
    {
        window = GetWindow<MaskMapGenerator>("Mask Map Generator");
        window.minSize = new Vector2(275, 300);
    }
}
