using UnityEngine;
using UnityEditor;
using UmStretch.MaskMapGenerator;

namespace UmStretch.MaskMapSeparator
{
    public class MaskMapSeparator : EditorWindow
    {
        private static MaskMapSeparator _window;
        private static Vector2 _minWindowSize = new(450, 300);

        private static Texture2D _inputTexture;
        // Metallic, AO, Detail, Smoothness
        private static Texture2D[] _outputTextures = new Texture2D[4];

        private static string _outputName;
        private static string _saveLocation = Config.defaultSaveLocation;

        [MenuItem("Tools/um-stretch/Mask Map Separator")]
        public static void OpenWindow()
        {
            _window = GetWindow<MaskMapSeparator>("Mask Map Separator");

            _window.minSize = _minWindowSize;
            _window.maxSize = _minWindowSize;
            _window.maxSize = Vector2.one * 10000;
        }

        void OnGUI()
        {
            _window ??= GetWindow<MaskMapSeparator>("Mask Map Separator");

            DrawInputTexture();
        }

        private void DrawInputTexture()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Input Texture", EditorStyles.centeredGreyMiniLabel);
            _inputTexture = (Texture2D)EditorGUILayout.ObjectField(_inputTexture, typeof(Texture2D), false, GUILayout.Height(96), GUILayout.Width(96));
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawOutputTextures()
        {
            
        }
    }
}